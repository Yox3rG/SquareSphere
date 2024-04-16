using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardHandler : MonoBehaviour
{
    private int cycleLength
    {
        get
        {
            return btn_rewards.Length;
        }
    }

    public RewardList dailyRewardList;

    // Button count is the length of the cycle.
    public Button[] btn_rewards;
    public Text txt_CycleCount;
    public Button btn_claim;

    public GameObject popUpWindow;

    public TimeHolder timeHolder;

    private void Start()
    {
        CheckAndPopUpIfNextDay();
    }

    public void CheckAndPopUpIfNextDay()
    {
#if UNITY_EDITOR
        Debug.Log("Checking if day is new.");
#endif
        ShowDailyRewardWindow(showEvenIfOld: false);
    }

    private void ShowDailyRewardWindow(bool showEvenIfOld = true)
    {
        bool isNewRewardDay = IsNextDay();

        if (!showEvenIfOld && !isNewRewardDay)
            return;

        int allDay = GetAllRewardedDayCount();
        if (!isNewRewardDay)
            allDay--;
        int currentCycleDay = CurrentPositionInRewardCycle(allDay);

        txt_CycleCount.text = "Week " + (AllRewardCycleCount(allDay) + 1).ToString();
        btn_claim.GetComponentInChildren<Text>().text = isNewRewardDay ? "Claim!" : "Close";
        HighLightButtonsAndSetRewards(allDay, currentCycleDay, isNewRewardDay);

        popUpWindow.GetComponent<Canvas>().enabled = true;

        if (isNewRewardDay)
        {
            DateTime currentDate = timeHolder.GetCurrentUTCTime().Value;
            RewardPackage package = GetDailyRewardPackage(allDay);

            RewardPackageHandler.Open(package);
            ProfileDataBase.main.DailyRewardClaimed(currentDate, package);
        }
    }

    private RewardPackage GetDailyRewardPackage(int day)
    {
        int dayInCycle = CurrentPositionInRewardCycle(day);
        // ++ because tiers in the list start with 1.
        if (dailyRewardList.ContainsKey(++day))
        {
            return dailyRewardList.dictionary[day];
        }
        else if (dailyRewardList.ContainsKey(++dayInCycle))
        {
            return dailyRewardList.dictionary[dayInCycle];
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("No RewardPackage on this day." + day);
#endif
        }
        return null;
    }

    private int CurrentPositionInRewardCycle(int days)
    {
        return days % cycleLength;
    }

    private int AllRewardCycleCount(int days)
    {
        return days / cycleLength;
    }

    private bool IsNextDay()
    {
        if (!timeHolder.GetCurrentUTCTime().HasValue)
            return false;

        DateTime currentDate = timeHolder.GetCurrentUTCTime().Value.Date;
        DateTime launchDate = ProfileDataBase.officialLaunchDay.Date;
        if (currentDate <= launchDate)
            return false;
        if (currentDate <= ProfileDataBase.main.GetLastLogin().Date)
            return false;
        int daysTheGameIsOutFor = (currentDate - launchDate).Days;
#if UNITY_EDITOR
        Debug.Log("Game out for: " + daysTheGameIsOutFor);
#endif
        if (GetAllRewardedDayCount() > daysTheGameIsOutFor)
            return false;

        return true;
    }

    // Only supports the current week.
    private void HighLightButtonsAndSetRewards(int currentDay, int currentCycleDay, bool isNewDay = false)
    {
        int startOfWeek = currentDay - currentCycleDay;
        DailyButton.HighLightType currentType = DailyButton.HighLightType.CLAIMED;

        for (int i = 0; i < cycleLength; i++)
        {
            if (i == currentCycleDay)
                currentType = isNewDay ? DailyButton.HighLightType.CURRENT : DailyButton.HighLightType.CURRENT | DailyButton.HighLightType.CLAIMED;
            else if (i > currentCycleDay)
                currentType = DailyButton.HighLightType.LOCKED;

            btn_rewards[i].GetComponent<DailyButton>().
                SetHighLightTypeWithRewardPackage(currentType, GetDailyRewardPackage(startOfWeek + i));
        }
    }

    public void ShowDailyPopupOnClick()
    {
        ShowDailyRewardWindow();
    }

    public void ClosePopup()
    {
        popUpWindow.GetComponent<Canvas>().enabled = false;
    }

    private int GetAllRewardedDayCount()
    {
        return ProfileDataBase.main.GetDailyDayCount();
    }
}