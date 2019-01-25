using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableTarget : Target, ITakeDamage
{
    public float Health;

    public bool TakeDamage(float value)
    {
        var result = false;
        Health -= value;
        if(Health <= 0)
        {
            result = true;
            Destroy(gameObject);
        }

        return result;
    }
}
