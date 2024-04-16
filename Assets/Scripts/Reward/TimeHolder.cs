using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[RequireComponent(typeof(LocalTimer))]
[RequireComponent(typeof(WorldTimeAPITimer))]
public class TimeHolder : MonoBehaviour
{
    private LocalTimer localTime;
    private WorldTimeAPITimer worldTime;

    private ITimeHolder[] onlineTimerHolders;

    private event Action OnSuccess;

    // Increase if we have more /\ options.
    private int onlineOptionCount { get { return onlineTimerHolders.Length; } set { } }
    //private int onlineOptionsReturned = 0;

    private ITimeHolder primaryTimeHolder;

    public bool makeRequestAtStart = true;

    private void Awake()
    {
        worldTime = GetComponent<WorldTimeAPITimer>();
        localTime = GetComponent<LocalTimer>();

        // Add new timers here.
        onlineTimerHolders = new[] { worldTime };

        primaryTimeHolder = localTime;

        if (makeRequestAtStart)
        {
            TryOnlineInitialize();
        }
    }

    private void OnEnable()
    {
        foreach (ITimeHolder holder in onlineTimerHolders)
        {
            holder.OnRecieveTimeAction += delegate { OnSuccess?.Invoke(); };
        }
        //Menu.main.OnBeforeLoadingMenuScene += TryOnlineInitialize;
    }

    private void OnDisable()
    {
        foreach (ITimeHolder holder in onlineTimerHolders)
        {
            holder.OnRecieveTimeAction -= delegate { OnSuccess?.Invoke(); };
        }
        //Menu.main.OnBeforeLoadingMenuScene -= TryOnlineInitialize;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
            return;

        //TryOnlineInitialize();
    }

    private void TryOnlineInitialize()
    {
        //onlineOptionsReturned = 0;
        foreach (ITimeHolder holder in onlineTimerHolders)
        {
            holder.Initialize();
        }
    }

    public int GetSecondsLeftFromDay()
    {
        if (!primaryTimeHolder.GetCurrentUTCTime().HasValue)
            return -1;

        return Mathf.CeilToInt((float)(new TimeSpan(days: 1, 0, 0, 0) - primaryTimeHolder.GetCurrentUTCTime().Value.TimeOfDay).TotalSeconds);
    }

    public DateTime? GetCurrentUTCTime()
    {
        return primaryTimeHolder.GetCurrentUTCTime();
    }

    public void SubscribeActionToOnSuccess(Action action)
    {
#if UNITY_EDITOR
        Debug.Log("Subscribed to OnRecieveTime");
#endif
        OnSuccess += action;
    }

    public void UnSubscribeActionToOnSuccess(Action action)
    {
#if UNITY_EDITOR
        Debug.Log("Unsubscribed from OnRecieveTime");
#endif
        OnSuccess -= action;
    }

    // Used if initialization of online services are turned off.
    private IEnumerator CallDelayedInitializeOnPrimary()
    {
        yield return new WaitForSeconds(1.5f);
        if (primaryTimeHolder == null)
            yield break;

        primaryTimeHolder.Initialize();
    }
}

public interface ITimeHolder
{
    Action OnRecieveTimeAction { get; set; }
    Action OnError { get; set; }

    void Initialize();
    void StopInitialize();

    DateTime? GetCurrentUTCTime();
    DateTime? GetCurrentLocalTime();
}
