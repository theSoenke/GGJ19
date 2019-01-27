using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour, ITakeDamage
{
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private Target _currentTarget;
    private int _currentTargetLevel;
    private bool _isParty = false;

    [SerializeField]
    private float _distanceToTarget = 0.75f;

    [SerializeField]
    private float _health = 42.0f;

    private float _coolDownTime;
    private float _nextTargetTime;

    public float ObjectDamage = 20f;
    public float PlayerDamage = 10f;
    public float DamageCooldown = 2f;
    public float AnimationWalkSpeedFactor = 1.0f;
    public float TargetPlayerPriority = 0.4f;

    [HideInInspector]
    public GameController gameController;

    private bool _isDead;


    // Start is called before the first frame update
    void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _isDead = false;

        MessageBus.Subscribe<TargetDestroyedMessage>(this, OnTargetDestroyed);
        MessageBus.Subscribe<PartyMessage>(this, OnParty);
    }


    void Update()
    {
        _coolDownTime -= Time.deltaTime;

        if (!_isParty)
        {
            if (_currentTarget == null)
            {
                _nextTargetTime -= Time.deltaTime;
                if (_nextTargetTime <= 0)
                {
                    var target = gameController.GetTarget(transform.position, false, _currentTargetLevel, IsTargetReachable);
                    SetTarget(target);
                }
            }
        }

      
        UpdateAnimator();
        UpdateTarget();
    }

    public DamageType GetDamageType()
    {
        return DamageType.Person;
    }

    public bool TakeDamage(float damage)
    {
        if (_isDead) return true;

        _health -= damage;
        if(_health <= 0) {
            //TODO: effects
            _isDead = true;
            MessageBus.Push(new EnemyDeadMessage(this));
            MessageBus.UnSubscribe<TargetDestroyedMessage>(this);
            MessageBus.UnSubscribe<PartyMessage>(this);
            Destroy(gameObject);
            return true;
        }
        return false;
    }

    public void SetTarget(Target target)
    {
        if (target == null || target.TargetPosition == null) return;
        
        _currentTarget = target;
        if(!_currentTarget.IsPrimaryTarget) _currentTargetLevel = target.Level;
        _navMeshAgent.SetDestination(target.TargetPosition.position);
    }

    //public void SetDestionation(Vector3 position, float partyTime)
    //{
    //    _partyTimeLeft = partyTime;
    //    _navMeshAgent.SetDestination(position);
    //}



    private void UpdateAnimator()
    {
        var animationWalkSpeed = _navMeshAgent.velocity.magnitude * AnimationWalkSpeedFactor;
        _animator.SetFloat("WalkSpeed", animationWalkSpeed);
    }

    private void UpdateTarget()
    {
        if (_isParty) return;

        if (_currentTarget != null)
        {
            Debug.DrawLine(transform.position, _currentTarget.TargetPosition.position, Color.red);

            if (_currentTarget.CanTargetMove)
            {
                _navMeshAgent.SetDestination(_currentTarget.TargetPosition.position);
            }

            var distance = (_currentTarget.TargetPosition.position - transform.position).sqrMagnitude;
            if (distance <= _distanceToTarget * _distanceToTarget)
            {
                var takeDamage = _currentTarget as ITakeDamage;
                if(takeDamage != null)
                {
                    ApplyDamage(takeDamage);
                }
                else
                {
                    if (!_currentTarget.IsPrimaryTarget)
                    {
                        _currentTarget = null;
                        _nextTargetTime = Random.Range(0.2f, 3f);
                    }
                }
            }           
        }
    }


    private void ApplyDamage(ITakeDamage takeDamage)
    {
        if (_coolDownTime > 0) return;
        if (takeDamage.GetDamageType() == DamageType.Object)
        {
            takeDamage.TakeDamage(ObjectDamage);
        }
        else if (takeDamage.GetDamageType() == DamageType.Person)
        {
            takeDamage.TakeDamage(PlayerDamage);
        }
        _coolDownTime = DamageCooldown;
    }

    private void OnTargetDestroyed(TargetDestroyedMessage msg)
    {
        if (msg.Target == _currentTarget)
        {
            _currentTarget = null;
            _nextTargetTime = 0.1f;
        }
        else
        {
            if (_currentTarget != null && !_currentTarget.IsPrimaryTarget)
            {
                if (ShouldTargetPrimary)
                {
                    _currentTarget = gameController.GetTarget(transform.position, true, 0, IsTargetReachable);
                    if (_currentTarget != null)
                    {
                        SetTarget(_currentTarget);
                    }
                }
            }
        }
    }

    public void OnParty(PartyMessage msg)
    {
        _isParty = msg.Start;
        if (msg.Start)
        {
            var target = gameController.waveController.spawns.OrderBy(t => (t.transform.position - transform.position).sqrMagnitude).First();
            _navMeshAgent.SetDestination(target.position);
        }
        else
        {
            if (_currentTarget != null) {
                _navMeshAgent.SetDestination(_currentTarget.TargetPosition.position);
            }
        }
    }

    private bool IsTargetReachable(Target target)
    {
        var path = new NavMeshPath();
        _navMeshAgent.CalculatePath(target.TargetPosition.position, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }

    private bool ShouldTargetPrimary
    {
        get
        {
            return Random.Range(0f, 1f) <= TargetPlayerPriority;
        }
    }
}
