using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;


//#if UNITY_ADS
//using UnityEngine.Advertisements;
//#endif

public class AdManager : MonoBehaviour, IUnityAdsInitializationListener
{
    public static AdManager Instance = null;

    private event Action OnSuccessAction = null;
    private event Action OnAfterSuccessAction = null;

    public enum AdType
    {
        VIDEO = 0,
        REWARDED = 1,
        BANNER = 2
    }

    // Update these if placed in a new project.
    private static readonly string[] placements = { "video", "rewardedVideo", "banner" };
    private static readonly string gameId = "3661087";

    private static bool testMode = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Advertisement.AddListener(this);
        // Advertisement.Initialize(gameId, testMode);
        //while (!Advertisement.IsReady(p))
        //    yield return null;
    }

    // TODO: Move this out of here, to remove connection between menu, profiledatabase and this.
    private void OnEnable()
    {
        Menu.main.OnBeforeLoadingGameScene += HideBanner;
        //Menu.main.OnBeforeLoadingGameScene += EnableBanner;
        Menu.main.OnBeforeLoadingMenuScene += HideBanner;
        Menu.main.OnBeforeLoadingMapEditorScene += HideBanner;
    }

    private void OnDisable()
    {
        Menu.main.OnBeforeLoadingGameScene -= HideBanner;
        //Menu.main.OnBeforeLoadingGameScene -= EnableBanner;
        Menu.main.OnBeforeLoadingMenuScene -= HideBanner;
        Menu.main.OnBeforeLoadingMapEditorScene -= HideBanner;
    }

    private static void EnableBanner()
    {
        if (ProfileDataBase.main.GetIsChallengeMode())
        {
            AdManager.Instance.ShowAd(AdManager.AdType.BANNER);
        }
    }

    public void SetRewardAction(Action action)
    {
        OnSuccessAction = action;
    }

    public void SetAfterRewardAction(Action action)
    {
        OnAfterSuccessAction = action;
    }

    public void ShowAd(AdType type)
    {
        if (type == AdType.BANNER)
        {
            Advertisement.Banner.SetPosition(BannerPosition.TOP_CENTER);
            Advertisement.Banner.Show(placements[(int)type]);
        }
        else if ((type == AdType.VIDEO) || (type == AdType.REWARDED && OnSuccessAction != null))
        {
            Advertisement.Show(placements[(int)type]);
        }
    }

    public void HideBanner()
    {
        Advertisement.Banner.Hide();
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (placementId == placements[(int)AdType.REWARDED])
        {
            if (showResult == ShowResult.Finished)
            {
#if UNITY_EDITOR
                Debug.Log("Finished ad! :)");
#endif
                // Reward!
                OnSuccessAction?.Invoke();
                OnSuccessAction = null;
                OnAfterSuccessAction?.Invoke();
                OnAfterSuccessAction = null;
            }
            else if (showResult == ShowResult.Failed)
            {
                // Fail!
            }
            else if (showResult == ShowResult.Skipped)
            {
                // Skipped!
            }
        }
    }

    public void OnUnityAdsDidError(string message)
    {
#if UNITY_EDITOR
        Debug.Log("Unity Ad Error!!!");
#endif
    }

    public void OnUnityAdsDidStart(string placementId)
    {
#if UNITY_EDITOR
        Debug.Log("Unity Ad Started!!!");
#endif
    }

    public void OnUnityAdsReady(string placementId)
    {
#if UNITY_EDITOR
        Debug.Log("Unity Ad Ready!!!");
#endif
    }

    public void OnInitializationComplete()
    {
        throw new NotImplementedException();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        throw new NotImplementedException();
    }
}
