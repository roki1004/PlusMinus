using System;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.Advertisements;

public enum AdsType
{
    Empty,
    Undo,
    Turn,
    StageEnd
}

public class AdsManager : WeakSingletonMonoBehaviour<AdsManager>
{
    //============================================
    private RewardBasedVideoAd rewardBasedVideo;
    private InterstitialAd interstitial;
    private AdRequest interstitialRequest;
    //============================================
    //> Google Ads
    private string appID;
    private string videoAdsID;    
    private string interstitialAdsID;
    //============================================
    //> Unity Ads
    private string appID_Unity;
    private string videoAdsID_Unity;
    //============================================
    protected override void Awake()
    {
        base.Awake();       

        Init_GoogleAds();
        Init_UnityAds();
        DontDestroyOnLoad(this);
    }
    //============================================
    private void Init_GoogleAds()
    {
#if UNITY_ANDROID
        appID = "ca-app-pub-3093121901616248~9386970822"; //> plus minus (app)
        videoAdsID = "ca-app-pub-3093121901616248/4577605077"; //> video ads (video)
        //> videoAdsID = "ca-app-pub-3940256099942544/5224354917"; //> test video from Ads SDK
        interstitialAdsID = "ca-app-pub-3093121901616248/6107901487"; //> stage end (full screen)
        //> interstitialAdsID = "ca-app-pub-3940256099942544/1033173712"; //> test interstitial from Ads SDK
#elif UNITY_IPHONE
        appId = "ios app id";
#else
        appId = "another platform";
#endif
        MobileAds.Initialize(appID);

        //> 비디오 광고용.
        rewardBasedVideo = RewardBasedVideoAd.Instance;
        rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
        rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
        RequestRewardBasedVideo();

        //> 전면 광고용.
        interstitialRequest = new AdRequest.Builder().Build();
        interstitial = new InterstitialAd(interstitialAdsID);
        interstitial.OnAdClosed += HandleOnAdClosed;
        interstitial.LoadAd(interstitialRequest);
    }

    private void Init_UnityAds()
    {
#if UNITY_ANDROID
        appID_Unity = "2747662";
        videoAdsID_Unity = "turnrewardvideo";
#elif UNITY_IPHONE
        appID_Unity = "2747660";
        videoAdsID_Unity = "turnrewardvideo";
#endif

        Advertisement.Initialize(appID_Unity);
    }
    //============================================
    //> Google Ads
    private void RequestRewardBasedVideo()
    {
        AdRequest request = new AdRequest.Builder().Build();
        rewardBasedVideo.LoadAd(request, videoAdsID);
    }
    
    private void HandleRewardBasedVideoClosed(object sender, EventArgs s)
    {
        MainLogic.Instance.OffAdsPanel_Undo_And_Use_Undo();
        RequestRewardBasedVideo();
    }

    private void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        MainLogic.Instance.ShowAds_Undo();
        string type = args.Type;
        double amount = args.Amount;        
    }

    private void HandleOnAdClosed(object sender, EventArgs s)
    {
        interstitial.Destroy();
        string debugMsg = "\nInterstitial HandleRewardBasedVideoClose";
        UIObjects.Instance.debugPanel.GetComponent<DebugInfo>().debug.text += debugMsg;
    }

    //> Unity Ads
    private void HandleShowResult(ShowResult result)
    {
        if(result == ShowResult.Finished)
        {
            MainLogic.Instance.ShowAds_Turn();
        }
    }
    //============================================
    public void ShowAds(AdsType type)
    {
        string debugMsg = string.Empty; ;
        if(type == AdsType.StageEnd)
        {
            if (interstitial.IsLoaded())
            {   
                debugMsg = "\nInterstitial load complete";
                interstitial.Show();
            }
            else
                debugMsg = "\nInterstitial load not complete";
        }
        else if(type == AdsType.Undo)
        {
            if (rewardBasedVideo.IsLoaded())
            {   
                debugMsg = "\nVideo load complete";
                rewardBasedVideo.Show();                
            }
            else
                debugMsg = "\nVideo load not complete";
        }
        else if(type == AdsType.Turn)
        {
            if(Advertisement.IsReady(videoAdsID_Unity))
            {
                ShowOptions options = new ShowOptions { resultCallback = HandleShowResult };
                Advertisement.Show(videoAdsID_Unity, options);
            }
        }

        Debug.Log(debugMsg);
        UIObjects.Instance.debugPanel.GetComponent<DebugInfo>().debug.text += debugMsg;
    }
    //============================================
}
