using System;
using System.Collections;
using System.Collections.Generic;
using ByteBrewSDK;
using UnityEngine;
using com.adjust.sdk;
using LionStudios.Suite.Analytics;
using LionStudios.Suite.Debugging;


public class SDKEventManager : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnGameStart()
    {
        
        string adjustAppToken = "";
#if UNITY_IOS
        /* Mandatory - set your iOS app token here */
        adjustAppToken = "";
#elif UNITY_ANDROID
        /* Mandatory - set your Android app token here */
        adjustAppToken = "64aqwg8wap34";
#endif
        
        AdjustEnvironment adjustEnvironment = AdjustEnvironment.Production;

        AdjustConfig config = new AdjustConfig(adjustAppToken,
        adjustEnvironment);

        config.setLogLevel(AdjustLogLevel.Info); // AdjustLogLevel.Suppress to disable logs
        config.setSendInBackground(true);
        new GameObject("Adjust").AddComponent<Adjust>(); // do not remove or rename
        Adjust.start(config);
        

        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {
            // AppLovin SDK is initialized, start loading ads
        };

        MaxSdk.SetSdkKey("WEp-We0GWicWitdQLFFSd26N5T7C9Umk49kCTFc_A_W2SKph9ClzJq32L6wcWXyhrAmTT0HAZB8DFe5Gvt8Vvw");
        MaxSdk.SetUserId(SystemInfo.deviceUniqueIdentifier);
        MaxSdk.SetVerboseLogging(true);
        MaxSdk.InitializeSdk();
            
        
        LionAnalytics.GameStart();
    }
    

    //---------------- E V E N T S -----------------------------------------------//

    public void LevelStarted(int _Level, int _Playtime)
    {
        if(_Level > 300) return;
        LionAnalytics.LevelStart(_Level, 1,null);
        ByteBrew.NewCustomEvent("LevelStart", string.Format("LevelNum={0};TimeInApp={1};", _Level, _Playtime));
    }

    public void LevelCompleted(int _Level, int _Playtime)
    {
        if(_Level > 300) return;
        //LionAnalytics.LevelComplete(_Level, _AttemptNumber, _Score, null);
        ByteBrew.NewCustomEvent("LevelComplete", string.Format("LevelNum={0};TimeInApp={1};", _Level, _Playtime));
    }

    public void LevelFailed(int _Level, int _Playtime)
    {
        if(_Level > 300) return;
        //LionAnalytics.LevelFail(_Level, _AttemptNumber,null);
        ByteBrew.NewCustomEvent("LevelFail", string.Format("LevelNum={0};TimeInApp={1};", _Level, _Playtime));
    }

    public void LevelRestarted(int _Level, int _Playtime)
    {
        if (_Level > 300) return;
        //LionAnalytics.LevelRestart(_Level, _AttemptNumber, null);
        ByteBrew.NewCustomEvent("LevelRestart", string.Format("LevelNum={0};TimeInApp={1};", _Level, _Playtime));

    }
    
    
    //---------------- A D S -----------------------------------------------//
/*
    public ShowAdRequest InterstitialHook(int _Level)
    {
        return AdsManager.InterstitialHook(_Level);
    }

    public void ShowInterstitial(ShowAdRequest _Request)
    {
        AdsManager.ShowInterstitial(_Request);
        //StartCoroutine(ShowDelay(_Request));
    }

    private IEnumerator ShowDelay(ShowAdRequest _Request)
    {
        yield return new WaitForSeconds(0.5f);
        AdsManager.ShowInterstitial(_Request);
    }

    public void ShowBanner()
    {
        AdsManager.ShowBanner();
    }

    public void HideBanner()
    {
        AdsManager.HideBanner();
    }

    public ShowAdRequest RewardedHook(int _Level, MaxSdkBase.Reward _Reward)
    {
        return AdsManager.RewardedHook(_Level, _Reward);
    }

    public void ShowRewardedVideo(ShowAdRequest _Request)
    {
        StartCoroutine(ShowDelayRV(_Request));
    }

    private IEnumerator ShowDelayRV(ShowAdRequest _Request)
    {
        yield return new WaitForSeconds(0.1f);
        AdsManager.ShowRewardedVideo(_Request);
    }
    */
}
