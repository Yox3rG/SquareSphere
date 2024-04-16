using System;
using System.Diagnostics;
using UnityEngine;

public class GooOverseer : MonoBehaviour
{
    public static event Action OnGrow;
    public static event Action OnReproduce;
    public static event Action OnEndOfReproduction;


    private void OnEnable()
    {
        SquareDataBase.OnEndOfRound += GooActionOnEndRound;
    }

    private void OnDisable()
    {
        SquareDataBase.OnEndOfRound -= GooActionOnEndRound;
    }

    private void GooActionOnEndRound()
    {
        OnGrow?.Invoke();
        OnReproduce?.Invoke();
        OnEndOfReproduction?.Invoke();
    }
}