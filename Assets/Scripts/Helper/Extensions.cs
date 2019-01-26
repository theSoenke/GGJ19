using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions 
{
    public static T GetRandom<T>(this T[] array)
    {
        if (array.Length <= 0) return default(T);
        var index = Random.Range(0, array.Length);
        return array[index];
    }
}
