using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DailyRewardData : ISerializationCallbackReceiver
{
    public int allDayCount;

    private static readonly DateTime defaultLastDateTime = ProfileDataBase.officialLaunchDay;
    private DateTime lastRewardDateTime;
    public string str_lastRewardDateTime;

    public DateTime LastRewardDateTime
    {
        get
        {
            return lastRewardDateTime;
        }
        set
        {
            lastRewardDateTime = value;
        }
    }

    public DailyRewardData()
    {
        allDayCount = 0;
        lastRewardDateTime = defaultLastDateTime;
    }

    public void OnBeforeSerialize()
    {
        str_lastRewardDateTime = lastRewardDateTime.ToString();
    }

    public void OnAfterDeserialize()
    {
        if(!DateTime.TryParse(str_lastRewardDateTime, out lastRewardDateTime))
        {
            lastRewardDateTime = defaultLastDateTime;
        }
    }
}
