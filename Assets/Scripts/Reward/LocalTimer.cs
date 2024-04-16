using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalTimer : MonoBehaviour, ITimeHolder
{
    public Action OnRecieveTimeAction { get; set; }
    public Action OnError { get; set; }

    public void Initialize()
    {
        OnRecieveTimeAction?.Invoke();
    }

    public void StopInitialize()
    {
    }

    public DateTime? GetCurrentUTCTime()
    {
        return DateTime.UtcNow;
    }

    public DateTime? GetCurrentLocalTime()
    {
        return DateTime.Now;
    }
}
