using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageZone : MonoBehaviour
{
    [SerializeField]
    private float _fieldRadius;

    [SerializeField]
    private float _baseDamage;

    [SerializeField]
    private AnimationCurve _damageFalloff;

    [SerializeField]
    private float _damageTickDelay;


    [SerializeField]
    private GameController _gameController;


    private float _lastDamageTickTime;



    // Start is called before the first frame update
    void Start()
    {
        _lastDamageTickTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time  > _lastDamageTickTime + _damageTickDelay ) 
        {
            PerformDamageTick();
            _lastDamageTickTime = Time.time;
        }
    }


    private void PerformDamageTick()
    {
        _gameController.believer += CalculateDamage();
    }

    private float CalculateDamage() 
    {
        var enemyList = _gameController.Enemies;
        var enemyCount = enemyList.Count;
        var result = 0f;
        foreach(var enemy in enemyList) 
        {
            var distance = Vector3.Distance(enemy.transform.position, transform.position);

            if(distance < _fieldRadius) {
                var crowdFactor = _damageFalloff.Evaluate((float)enemyCount);
                result += _baseDamage * crowdFactor;
            }
        }
        return result;
    }
}
