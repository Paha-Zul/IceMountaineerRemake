
using System;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using UnityEngine;

public class AdFunctions : IInterstitialAdListener, IRewardedVideoAdListener
{
    private static AdFunctions inst;

    public int returnToMainMenuCounter = 1; //Start at 1 to give the user 5 playthroughs before an ad.

    public void init()
    {
        string appKey = "17439d477eae29f7eb218efb949d88287d3388adb2fcbacd";
        if (Debug.isDebugBuild)
        {
            Appodeal.setTesting(true);
            //Appodeal.setLogging(true);
        }

        Appodeal.disableNetwork("amazon_ads");
        Appodeal.disableNetwork("smaato");
        Appodeal.disableNetwork("mailru");
        Appodeal.disableNetwork("vungle");
        Appodeal.disableNetwork("facebook");
        Appodeal.disableNetwork("startapp");
        Appodeal.disableNetwork("yandex");
        Appodeal.disableNetwork("tapjoy");
        Appodeal.disableNetwork("inner-active");
        Appodeal.disableNetwork("cheetah");
        Appodeal.disableLocationPermissionCheck();
        Appodeal.cache(Appodeal.REWARDED_VIDEO);
        Appodeal.initialize(appKey, Appodeal.BANNER | Appodeal.INTERSTITIAL | Appodeal.REWARDED_VIDEO | Appodeal.NON_SKIPPABLE_VIDEO);

        Appodeal.setInterstitialCallbacks(this);
        Appodeal.setRewardedVideoCallbacks(this);

    }

    public void DisplayInterToMainMenu()
    {
        if (this.returnToMainMenuCounter % Constants.NumTimesToMainMenuForInter == 0 && Appodeal.isLoaded(Appodeal.INTERSTITIAL))
            Appodeal.show(Appodeal.INTERSTITIAL);

        this.returnToMainMenuCounter++;
    }

    public void DisplayInter()
    {
        if (Appodeal.isLoaded(Appodeal.INTERSTITIAL))
            Appodeal.show(Appodeal.INTERSTITIAL);
    }

    public void displayRewardVideo()
    {
        if (Appodeal.isLoaded(Appodeal.REWARDED_VIDEO))
            Appodeal.show(Appodeal.REWARDED_VIDEO);
    }

    public void displayBanner()
    {
        if (Appodeal.isLoaded(Appodeal.BANNER_BOTTOM))
            Appodeal.show(Appodeal.BANNER_BOTTOM);
    }

    public void hideBanner()
    {
        Appodeal.hide(Appodeal.BANNER_BOTTOM);
    }

    public static AdFunctions instance()
    {
        if (inst == null) inst = new AdFunctions();
        return inst;
    }


    void IInterstitialAdListener.onInterstitialFailedToLoad()
    {
    }

    void IInterstitialAdListener.onInterstitialShown()
    {
    }

    void IInterstitialAdListener.onInterstitialClosed()
    {
    }

    void IInterstitialAdListener.onInterstitialClicked()
    {
    }

    void IRewardedVideoAdListener.onRewardedVideoLoaded()
    {
    }

    void IRewardedVideoAdListener.onRewardedVideoFailedToLoad()
    {
    }

    void IRewardedVideoAdListener.onRewardedVideoShown()
    {
    }

    void IRewardedVideoAdListener.onRewardedVideoFinished(int amount, string name)
    {
        int coins = PlayerPrefs.GetInt(Constants.CoinPrefString);
        PlayerPrefs.SetInt(Constants.CoinPrefString, coins + Constants.VideoRewardAmount);
        MainMenu.RefreshCoinAmount();
    }

    public void onInterstitialLoaded(bool isPrecache)
    {
        throw new NotImplementedException();
    }

    public void onRewardedVideoClosed(bool finished)
    {
        throw new NotImplementedException();
    }

    //void OnApplicationFocus(bool hasFocus)
    //{
    //    isPaused = !hasFocus;
    //}
}
