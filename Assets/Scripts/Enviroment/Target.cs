using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public Transform TargetPosition;

    public int Priority;
    public bool IsPrimaryTarget;
    public bool CanTargetMove;
    public bool CheckReachable;

    public bool IsAvailable;

    void Awake()
    {
        if (TargetPosition == null) TargetPosition = transform;       
    }
}
