using System;
using System.Reflection;
using UnityEngine;

public class CooldownTimer : MonoBehaviour
{
    public event Action OnFinished;
    public event Action OnNextStep;

    private float allTimeLeft = 0f;

    private float stepSizeInSeconds = 1f;
    private float nextTargetStep = 0f;
    private float timeLeft = 0f;

    public float TimeLeft { get { return timeLeft; } set { } }
    public bool isRunning { get; private set; } = false;
    public bool isInitialized { get; private set; } = false;

    private void Update()
    {
        RunTimer();
    }

    private void RunTimer()
    {
        if (!isRunning)
            return;



        timeLeft -= Time.deltaTime;
        if(timeLeft <= nextTargetStep)
        {
            nextTargetStep -= stepSizeInSeconds;
            OnNextStep?.Invoke();
            
            if(timeLeft <= 0f)
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

        nextTargetStep = allTimeLeft - stepSizeInSeconds;
        timeLeft = allTimeLeft;

        isRunning = true;
    }

    public void Stop()
    {
        isRunning = false;
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
