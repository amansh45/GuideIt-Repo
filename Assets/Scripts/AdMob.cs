using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using TMPro;

public class AdMob : MonoBehaviour
{

    string APP_ID = "ca-app-pub-9178056825348469~3651696201";
    string BANNER_AD_ID = "ca-app-pub-3940256099942544/6300978111";
    string INTERSTITIAL_AD_ID = "ca-app-pub-3940256099942544/1033173712";
    string VIDEO_REWARD_AD_ID = "ca-app-pub-3940256099942544/5224354917";

    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardBasedVideoAd rewardBasedVideoAd;

    [SerializeField] GameObject adLabel;
    TextMeshProUGUI labelTextField;

    private void Awake()
    {
        int adMobCount = FindObjectsOfType<AdMob>().Length;
        if (adMobCount > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    public void RequestInterstitial()
    {
        interstitialAd = new InterstitialAd(INTERSTITIAL_AD_ID);


        interstitialAd.OnAdLoaded += HandleOnAdLoaded;
        interstitialAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        interstitialAd.OnAdOpening += HandleOnAdOpened;
        interstitialAd.OnAdClosed += HandleOnAdClosed;
        interstitialAd.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        labelTextField.text = "Ad is about to load";

        AdRequest request = new AdRequest.Builder().Build();
        interstitialAd.LoadAd(request);
    }

    public void ShowInterstitialAd()
    {
        labelTextField.text = "Ad is about to show";
        if (interstitialAd.IsLoaded())
        {
            interstitialAd.Show();
        }
    }

    public void RequestRewardBasedVideo()
    {
        rewardBasedVideoAd = RewardBasedVideoAd.Instance;

        rewardBasedVideoAd.OnAdLoaded += HandleOnAdLoaded;
        rewardBasedVideoAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        rewardBasedVideoAd.OnAdOpening += HandleOnAdOpened;
        rewardBasedVideoAd.OnAdClosed += HandleOnAdClosed;
        rewardBasedVideoAd.OnAdLeavingApplication += HandleOnAdLeavingApplication;
        rewardBasedVideoAd.OnAdRewarded += HandleUserEarnedReward;

        labelTextField.text = "Ad is about to load";

        AdRequest request = new AdRequest.Builder().Build();
        rewardBasedVideoAd.LoadAd(request, VIDEO_REWARD_AD_ID);
    }

    public void ShowVideoRewardedAd()
    {
        labelTextField.text = "Ad is about to show";
        if (rewardBasedVideoAd.IsLoaded())
        {
            rewardBasedVideoAd.Show();
        }
    }

    void Start()
    {
        labelTextField = adLabel.GetComponent<TextMeshProUGUI>();
        MobileAds.Initialize(initStatus => { });
    }

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        
        labelTextField.text = "Ad is loaded";
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {

        labelTextField.text = "Failed to load ad";
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        LevelController levelControllerClass = FindObjectOfType<LevelController>();
        levelControllerClass.isPlayerEligibleForReward = true;
    }
}
