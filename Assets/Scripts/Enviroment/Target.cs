using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public Transform TargetPosition;

    void Awake()
    {
        if (TargetPosition == null) TargetPosition = transform;       
    }
}
