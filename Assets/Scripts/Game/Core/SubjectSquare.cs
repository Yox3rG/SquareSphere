using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubjectSquare : MonoBehaviour
{
    public static event Action<Vector2, byte> OnDeath;

    public void OnDeathFunction()
    {
        OnDeath?.Invoke(transform.position, GetComponent<Destroyable>().ColorIndex);
    }
}