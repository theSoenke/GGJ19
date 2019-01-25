using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Target _currentTarget;

    [SerializeField]
    private float _distanceToTarget = 0.75f;

    private float _coolDownTime;

    public float ObjectDamage = 20f;
    public float PlayerDamage = 10f;
    public float DamageCooldown = 2f;

    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 1000))
            {
                //_navMeshAgent.SetDestination(hit.point);
                var target = hit.transform.gameObject.GetComponent<Target>();
                var components = hit.transform.gameObject.GetComponents(typeof(MonoBehaviour));

                foreach(var c in components)
                {
                    Debug.Log("HittedComp: " + c.GetType());
                }

                if(target != null)
                {
                    SetTarget(target);
                }
            }
        }

        UpdateTarget();
    }

    public void SetTarget(Target target)
    {
        _currentTarget = target;
        _navMeshAgent.SetDestination(target.TargetPosition.position);
    }

    private void UpdateTarget()
    {
        _coolDownTime -= Time.deltaTime;

        if(_currentTarget != null)
        {
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
}
