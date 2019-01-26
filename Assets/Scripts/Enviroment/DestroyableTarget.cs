using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableTarget : Target, ITakeDamage
{
    [SerializeField]
    private float Health;

    [SerializeField]
    private DamageType DamageType;

    public bool TakeDamage(float value)
    {
        var result = false;
        Health -= value;
        if(Health <= 0)
        {
            result = true;
            Destroy(gameObject);
            MessageBus.Push(new TargetDestroyed(this));
        }

        return result;
    }

    public DamageType GetDamageType()
    {
        return DamageType;
    }
}
