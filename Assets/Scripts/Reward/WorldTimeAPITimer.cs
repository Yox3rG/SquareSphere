using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WorldTimeAPITimer : MonoBehaviour, ITimeHolder
{
    public Action OnRecieveTimeAction { get; set; }
    public Action OnError { get; set; }

    private double unityTimeAtRequest;
    private DateTime timeAtRequest;
    public bool isValidTime { get; private set; } = false;

    private string url = "http://worldtimeapi.org/api/timezone/Europe/Budapest";

    private void Start()
    {
        //ConvertStringToTime("2020-06-28T14:17:33.941979+00:00", out timeAtLaunch);
        //Debug.Log(GetLocalTimeAtLaunch());
    }
    
    public void TryGetWorldTime()
    {
        StopCoroutine(GetCurrentTime());

        if (isValidTime)
        {
#if UNITY_EDITOR
            Debug.Log("Skipped Online part.");
#endif
            OnRecieveTimeAction?.Invoke();
            return;
        }


        StartCoroutine(GetCurrentTime());
    }

    public void Initialize()
    {
        TryGetWorldTime();
    }

    public void StopInitialize()
    {
        StopAllCoroutines();
    }

    public DateTime? GetCurrentUTCTime()
    {
        DateTime? date = null;
        if (isValidTime)
        {
            double secondsPastRequest = Time.time - unityTimeAtRequest;

            date = timeAtRequest.AddSeconds(secondsPastRequest);
        }
        return date;
    }

    public DateTime? GetCurrentLocalTime()
    {
        DateTime? date = null;
        if (isValidTime)
        {
            double secondsPastRequest = Time.time - unityTimeAtRequest;

            date = timeAtRequest.ToLocalTime().AddSeconds(secondsPastRequest);
        }
        return date;
    }

    public DateTime? GetUTCTimeAtRequest()
    {
        DateTime? date = null;
        if (isValidTime)
        {
            date = timeAtRequest;
        }
        return date;
    }

    public DateTime? GetLocalTimeAtRequest()
    {
        DateTime? date = null;
        if (isValidTime)
        {
            date = timeAtRequest.ToLocalTime();
        }
        return date;
    }

    private IEnumerator GetCurrentTime()
    {
        UnityWebRequest www = new UnityWebRequest(url) { downloadHandler = new DownloadHandlerBuffer() };

        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
#if UNITY_EDITOR
            Debug.Log("Error");
#endif
            OnError?.Invoke();
            yield break;
        }

        TimeInfoJson timeInfo = JsonUtility.FromJson<TimeInfoJson>(www.downloadHandler.text);
        isValidTime = ConvertStringToUTC(timeInfo.utc_datetime, out timeAtRequest);
        unityTimeAtRequest = Time.time;
#if UNITY_EDITOR
        Debug.Log("RecievedTime");
#endif

        OnRecieveTimeAction?.Invoke();
        //Debug.Log(www.downloadHandler.text);
        //Debug.Log(timeInfo.utc_datetime);
        //Debug.Log(timeAtLaunch);
    }

    private bool ConvertStringToUTC(string dateString, out DateTime date)
    {
        date = new DateTime();
        try
        {
            date = DateTime.Parse(dateString).ToUniversalTime();
        }
        catch(Exception e)
        {
#if UNITY_EDITOR
            Debug.Log("Invalid date string\n" + e.Message);
#endif
            return false;
        }

        return true;
    }
}

[Serializable]
internal class TimeInfoJson
{
    public string datetime = null;
    public string utc_datetime = null;
}