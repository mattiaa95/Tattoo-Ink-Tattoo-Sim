using UnityEngine;
using Yodo1.MAS;

public class Yodo1Ads : MonoBehaviour
{
    
    bool enableBannerApiV2 = true;
    public static Yodo1Ads Instance;

    [HideInInspector()]
    public bool bVideoInk = false;
    [HideInInspector()]
    public bool bVideoTattoo = false;
    [HideInInspector()]
    public string privacyPolicyLink = "https://gist.githubusercontent.com/mattiaa95/43f479d4da16a7bb4aaf5504874b01bb/raw/d25d0afa3cf5a33d08e396fbadae1e9f04651b53/GeneralPrivacyPolicy.md";


    void Start()
    {
            Instance = this;
        Yodo1U3dMasCallback.OnSdkInitializedEvent += (success, error) =>
        {
            Debug.Log(Yodo1U3dMas.TAG + "OnSdkInitializedEvent, success:" + success + ", error: " + error.ToString());
            Debug.Log(Yodo1U3dMas.TAG + "OnSdkInitializedEvent, age:" + Yodo1U3dMas.GetUserAge());

            if (success)
            {
                InitializeInterstitialAds();
                InitializeRewardedAds();
            }

        };

        Yodo1MasUserPrivacyConfig userPrivacyConfig = new Yodo1MasUserPrivacyConfig()
            .titleBackgroundColor(Color.green)
            .titleTextColor(Color.blue)
            .contentBackgroundColor(Color.black)
            .contentTextColor(Color.white)
            .buttonBackgroundColor(Color.red)
            .buttonTextColor(Color.green);

        Yodo1AdBuildConfig config = new Yodo1AdBuildConfig()
            .enableAdaptiveBanner(true)
            .enableUserPrivacyDialog(true)
            .userPrivacyConfig(userPrivacyConfig);
        Yodo1U3dMas.SetAdBuildConfig(config);

        Yodo1U3dMas.InitializeSdk();
       
    }

    #region Interstitial Ad Methods
    private void InitializeInterstitialAds()
    {
        Yodo1U3dMasCallback.Interstitial.OnAdOpenedEvent += OnInterstitialAdOpenedEvent;
        Yodo1U3dMasCallback.Interstitial.OnAdClosedEvent += OnInterstitialAdClosedEvent;
        Yodo1U3dMasCallback.Interstitial.OnAdErrorEvent += OnInterstitialAdErorEvent;
    }

    private void OnInterstitialAdOpenedEvent()
    {
        Debug.Log(Yodo1U3dMas.TAG + "Interstitial ad opened");
    }

    private void OnInterstitialAdClosedEvent()
    {
        Debug.Log(Yodo1U3dMas.TAG + "Interstitial ad closed");
    }

    private void OnInterstitialAdErorEvent(Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "Interstitial ad error - " + adError.ToString());
    }

    public void ShowInterstitial()
    {
        if (Yodo1U3dMas.IsInterstitialAdLoaded())
        {
            Yodo1U3dMas.ShowInterstitialAd();
        }
        else
        {
            Debug.Log(Yodo1U3dMas.TAG + "Interstitial ad has not been cached.");
        }
    }
    #endregion

    #region Reward video Ad Methods
    private void InitializeRewardedAds()
    {
        Yodo1U3dMasCallback.Rewarded.OnAdOpenedEvent += OnRewardedAdOpenedEvent;
        Yodo1U3dMasCallback.Rewarded.OnAdClosedEvent += OnRewardedAdClosedEvent;
        Yodo1U3dMasCallback.Rewarded.OnAdReceivedRewardEvent += OnAdReceivedRewardEvent;
        Yodo1U3dMasCallback.Rewarded.OnAdErrorEvent += OnRewardedAdErorEvent;
    }

    private void OnRewardedAdOpenedEvent()
    {
        Debug.Log(Yodo1U3dMas.TAG + "Rewarded ad opened");
    }

    private void OnRewardedAdClosedEvent()
    {
        Debug.Log(Yodo1U3dMas.TAG + "Rewarded ad closed");
    }

    private void OnAdReceivedRewardEvent()
    {
        VideoRewarded();
    }

    private void OnRewardedAdErorEvent(Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "Rewarded ad error - " + adError.ToString());
    }

    public void ShowVideoReward()
    {
        if (Yodo1U3dMas.IsRewardedAdLoaded())
        {
            Yodo1U3dMas.ShowRewardedAd();
        }
        else
        {
            VideoRewardNotReady();
        }
    }
    #endregion


   public void VideoRewarded()
    {
        Debug.Log("VIDEO REWARDED");
        if(Application.loadedLevelName == "TattooSalon") 
        {
            if(bVideoTattoo)
                GameObject.Find("Canvas/Menus/TattooButtonsHolder").GetComponent<DecorationsMenuScript>().EndWatchingVideoDecoration();
            if(bVideoInk)
                GameObject.Find("Canvas/Menus/InkButtonsHolder").GetComponent<InkMenuScript>().EndWatchingVideoDecoration();
            bVideoTattoo = false;
            bVideoInk = false;
        }
    }

    void VideoRewardNotReady()
    {
        Debug.Log("VIDEO IS NOT READY");
        if (GameObject.Find("Canvas/PopUps/PopUpMessage") != null)
        {
            GameObject.Find("Canvas").GetComponent<MenuManager>().ShowPopUpMessageTitleText("Sorry");
            GameObject.Find("Canvas").GetComponent<MenuManager>().ShowPopUpMessageCustomMessageText("Video is currently unavailable, please try again later");
        }
    }
}
