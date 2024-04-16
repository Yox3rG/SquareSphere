

using System;
using UnityEngine;
using UnityEngine.UI;

public class DailyCooldownTimer : MonoBehaviour
{
    private CoCooldownTimer cooldown;

    public DailyRewardHandler dailyHandler;

    public TimeHolder timeHolder;
    public Text[] targets;

    private void Awake()
    {
        if(cooldown == null)
            cooldown = gameObject.AddComponent<CoCooldownTimer>();

        SetPlaceHolderText("");
    }

    private void Start()
    {
        InitializeAndStartTimer();
    }

    private void OnEnable()
    {
        Menu.main.OnBeforeLoadingMenuScene += OnReturnToMenu;
        Menu.main.OnBeforeLoadingGameScene += OnLeavingMenu;
    }

    private void OnDisable()
    {
        Menu.main.OnBeforeLoadingMenuScene -= OnReturnToMenu;
        Menu.main.OnBeforeLoadingGameScene -= OnLeavingMenu;
    }

    private void InitializeAndStartTimer()
    {
#if UNITY_EDITOR
        //Debug.LogError("InitializeAndStart, seconds left: " + timeHolder.GetSecondsLeftFromDay());
#endif
        cooldown.Initialize(timeHolder.GetSecondsLeftFromDay(), 1f, EndOfCooldownAction, UpdateTexts);
        cooldown.Begin();
    }

    private void OnReturnToMenu()
    {
        dailyHandler.CheckAndPopUpIfNextDay();
        InitializeAndStartTimer();
    }

    private void OnLeavingMenu()
    {
        cooldown.StopAndInvalidate();
        SetPlaceHolderText("");
    }

    private void EndOfCooldownAction()
    {
        dailyHandler.CheckAndPopUpIfNextDay();
        InitializeAndStartTimer();
    }

    private void UpdateTexts()
    {
        TimeSpan timespan = TimeSpan.FromSeconds(cooldown.TimeLeft + .2);
        foreach(Text t in targets)
        {
            t.text = timespan.ToString(@"hh\:mm\:ss");
        }
    }

    private void SetPlaceHolderText(string placeHolder)
    {
        foreach (Text t in targets)
        {
            t.text = placeHolder;
        }
    }
}