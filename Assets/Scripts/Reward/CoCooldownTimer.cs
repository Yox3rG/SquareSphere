using System;
using System.Collections;
using System.Reflection;
using UnityEngine;



public class CoCooldownTimer : MonoBehaviour
{

    public event Action OnFinished;
    public event Action OnNextStep;

    private float allTimeLeft = 0f;

    private float stepSizeInSeconds = 1f;
    private float timeLeft = 0f;

    private IEnumerator coroutine;

    public float TimeLeft { get { return timeLeft; } set { } }
    public bool isRunning { get; private set; } = false;
    public bool isInitialized { get; private set; } = false;

    private void Awake()
    {
        coroutine = RunTimer();
    }

    private IEnumerator RunTimer()
    {
        var wait = new WaitForSecondsRealtime(stepSizeInSeconds);
        while (true)
        {
            yield return wait;

            timeLeft -= stepSizeInSeconds;
            OnNextStep?.Invoke();

            if (timeLeft <= 0f)
            {
                Stop();
                timeLeft = 0f;
                OnFinished?.Invoke();
            }
        }
    }

    public void Initialize(float timeLeft, float stepSizeInSeconds, Action OnFinished, Action OnNextStep = null)
    {
        if (timeLeft <= 0f)
            return;

        Stop();

        this.allTimeLeft = timeLeft;
        this.stepSizeInSeconds = stepSizeInSeconds;

        this.OnFinished = OnFinished;
        this.OnNextStep = OnNextStep;

        isInitialized = true;
    }

    public void Begin()
    {
        if (!isInitialized)
            return;

        timeLeft = allTimeLeft;
        isRunning = true;

        StartCoroutine(coroutine);
    }

    public void Stop()
    {
        isRunning = false;
        StopCoroutine(coroutine);
    }

    public void StopAndInvalidate()
    {
        Stop();
        isInitialized = false;
    }

    // These are not in working conditions. Review everything before and after uncommenting.
    //public void Pause()
    //{
    //    isRunning = false;
    //}

    //public void UnPause()
    //{
    //    if (!isInitialized)
    //        return;

    //    isRunning = true;
    //}
}