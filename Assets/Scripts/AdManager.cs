using System;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

// Example script showing how to invoke the Google Mobile Ads Unity plugin.
public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    BannerView bannerView;
    InterstitialAd interstitial;
    NativeExpressAdView nativeExpressAdView;
    RewardBasedVideoAd rewardBasedVideo;

    bool
    //Banner Flag
    bannerLoaded,
    //Inter Flag
    interstitialLoaded,
    interstitialClosed,
    //Reward Flag
    rewardBasedVideoLoaded,
    rewardBasedVideoRewarded,
    rewardBasedVideoClosed,
    //Failed Flag
    bannerFailed,
    interstitialFailed,
    rewardBasedVideoFailed;

    void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
#if UNITY_EDITOR
        Debug.Log("Nope.");
#elif UNITY_ANDROID
        string appId = "_APP_ID_";
#elif UNITY_IOS
        string appId = "_APP_ID_";
#endif

#if UNITY_EDITOR
        Debug.Log("Nope.");
#else
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);

        // Get singleton reward based video ad reference.
        rewardBasedVideo = RewardBasedVideoAd.Instance;

        // RewardBasedVideoAd is a singleton, so handlers should only be registered once.
        rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
        rewardBasedVideo.OnAdFailedToLoad += HandleOnRewardBasedVideoFailedToLoad;
        rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
        rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
#endif
    }

    public void LoadAllAds()
    {
        RequestBanner();
        RequestInterstitial();
        RequestRewardBasedVideo();
    }

    // Returns an ad request with custom ad targeting.
    //AdRequest CreateAdRequest()
    //{
    //return new AdRequest.Builder()
    //.AddTestDevice(AdRequest.TestDeviceSimulator)
    //.AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
    //.AddKeyword("game")
    //.SetGender(Gender.Male)
    //.SetBirthday(new DateTime(1985, 1, 1))
    //.TagForChildDirectedTreatment(false)
    //.AddExtra("color_bg", "9B30FF")
    //.Build();
    //return new AdRequest.Builder().Build();
    //}

    public void RequestBanner()
    {
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "_APP_ID_";
#elif UNITY_IOS
        string adUnitId = "_APP_ID_";
#endif

        // Clean up banner ad before creating a new one.
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

        bannerView.OnAdLoaded += HandleOnBannerLoaded;
        bannerView.OnAdFailedToLoad += HandleOnBannerFailedToLoad;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);
        HideBanner();
    }

    public void RequestInterstitial()
    {
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "_APP_ID_";
#elif UNITY_IOS
        string adUnitId = "_APP_ID_";
#endif

        // Clean up interstitial ad before creating a new one.
        if (interstitial != null)
        {
            interstitial.Destroy();
        }

        // Create an interstitial.
        interstitial = new InterstitialAd(adUnitId);

        // Register for ad events.
        interstitial.OnAdLoaded += HandleInterstitialLoaded;
        interstitial.OnAdFailedToLoad += HandleOnInterstitialFailedToLoad;
        interstitial.OnAdClosed += HandleInterstitialClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the interstitial with the request.
        interstitial.LoadAd(request);
    }

    public void RequestRewardBasedVideo()
    {
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = "_APP_ID_";
#elif UNITY_IOS
        string adUnitId = "_APP_ID_";
#endif

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the rewarded video ad with the request.
        rewardBasedVideo.LoadAd(request, adUnitId);
    }

    public void ShowBanner()
    {
        bannerView.Show();
    }

    public void HideBanner()
    {
        bannerView.Hide();
    }

    public void ShowInterstitial()
    {
        interstitial.Show();
    }

    public void ShowRewardBasedVideo()
    {
        rewardBasedVideo.Show();
    }

    #region handler
    void HandleOnBannerLoaded(object sender, EventArgs args)
    {
        bannerLoaded = true;
    }

    void HandleInterstitialLoaded(object sender, EventArgs args)
    {
        interstitialLoaded = true;
    }

    void HandleInterstitialClosed(object sender, EventArgs args)
    {
        interstitialClosed = true;
    }

    void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        rewardBasedVideoLoaded = true;
    }

    void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        rewardBasedVideoRewarded = true;
    }

    void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        rewardBasedVideoClosed = true;
    }

    void HandleOnBannerFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        bannerFailed = true;
    }

    void HandleOnInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        interstitialFailed = true;
    }

    void HandleOnRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        rewardBasedVideoFailed = true;
    }
    #endregion

    void Update()
    {
        //Banner
        if (bannerLoaded)
        {
            bannerLoaded = false;
            bannerFailed = false;
        }
        //Inter
        if (interstitialLoaded)
        {
            interstitialLoaded = false;
            interstitialFailed = false;
            UiManager.Instance.InterstitialReady = true;
        }
        if (interstitialClosed)
        {
            interstitialClosed = false;
            UiManager.Instance.InterstitialReady = false;
            RequestInterstitial();
        }
        //Reward
        if (rewardBasedVideoLoaded)
        {
            rewardBasedVideoLoaded = false;
            rewardBasedVideoFailed = false;
            UiManager.Instance.RewardBasedVideoReady = true;
        }
        if (rewardBasedVideoRewarded)
        {
            rewardBasedVideoRewarded = false;
            Score.Instance.IsRewarded();
        }
        if (rewardBasedVideoClosed)
        {
            rewardBasedVideoClosed = false;
            UiManager.Instance.RewardBasedVideoReady = false;
            RequestRewardBasedVideo();
        }
    }

    public void IfAnyFailed()
    {
        if (bannerFailed)
        {
            bannerFailed = false;
            RequestBanner();
        }
        if (interstitialFailed)
        {
            interstitialFailed = false;
            RequestInterstitial();
        }
        if (rewardBasedVideoFailed)
        {
            rewardBasedVideoFailed = false;
            RequestRewardBasedVideo();
        }
    }
}

