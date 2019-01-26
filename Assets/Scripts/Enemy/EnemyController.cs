using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private Target _currentTarget;

    [SerializeField]
    private float _distanceToTarget = 0.75f;

    private float _coolDownTime;
    private float _nextTargetTime;

    public float ObjectDamage = 20f;
    public float PlayerDamage = 10f;
    public float DamageCooldown = 2f;
    public float AnimationWalkSpeedFactor = 1.0f;

    [HideInInspector]
    public GameController GameController;

    // Start is called before the first frame update
    void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        MessageBus.Subscribe<TargetDestroyed>(OnTargetDestroyed);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentTarget == null)
        {
            _nextTargetTime -= Time.deltaTime;
            if (_nextTargetTime <= 0)
            {
                var target = GetNextTarget();
                SetTarget(target);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            //var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hit;
            //if(Physics.Raycast(ray, out hit, 1000))
            //{
            //    //_navMeshAgent.SetDestination(hit.point);
            //    var target = hit.transform.gameObject.GetComponent<Target>();
            //    var components = hit.transform.gameObject.GetComponents(typeof(MonoBehaviour));

            //    foreach(var c in components)
            //    {
            //        Debug.Log("HittedComp: " + c.GetType());
            //    }

            //    if(target != null)
            //    {
            //        SetTarget(target);
            //    }
            //}
        }
        UpdateAnimator();
        UpdateTarget();
    }

    public void SetTarget(Target target)
    {
        if (target == null || target.TargetPosition == null) return;

        _currentTarget = target;
        _navMeshAgent.SetDestination(target.TargetPosition.position);
    }

    private Target GetNextTarget()
    {
        if (GameController.Walls.Length <= 0) return null;

        var targetIndex = Random.Range(0, GameController.Walls.Length);
        var target = GameController.Walls[targetIndex];
        return target;
    }

    private void UpdateAnimator()
    {
        var animationWalkSpeed = _navMeshAgent.velocity.magnitude * AnimationWalkSpeedFactor;
        _animator.SetFloat("WalkSpeed", animationWalkSpeed);
    }

    private void UpdateTarget()
    {
        _coolDownTime -= Time.deltaTime;

        if(_currentTarget != null)
        {
            Debug.DrawLine(transform.position, _currentTarget.TargetPosition.position, Color.red);
            

            var takeDamage = _currentTarget as ITakeDamage;
            if (takeDamage != null)
            {
                var distance = (_currentTarget.TargetPosition.position - transform.position).sqrMagnitude;
                if (distance <= _distanceToTarget * _distanceToTarget)
                {
                    ApplyDamage(takeDamage);
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
        else if(takeDamage.GetDamageType() == DamageType.Person)
        {
            takeDamage.TakeDamage(PlayerDamage);
        }
        _coolDownTime = DamageCooldown;
    }

    private void OnTargetDestroyed(TargetDestroyed msg)
    {
        if(msg.Target == _currentTarget)
        {
            _currentTarget = null;
            _nextTargetTime = 0.1f;
        }
    }
}
