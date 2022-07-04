using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;

public class AdMob_Manager : MonoBehaviour
{

    [SerializeField] Button_Manager button_manager;

    //List<string> deviceIds = new List<string>();

    public void Start()
    {

        // Google AdMob Initial
        MobileAds.Initialize(initStatus => { });

        RequestBanner();
        RequestReward();
        //この処理をしなくても表示されるが、アクションシーンから戻ってきたときのために表示処理
        bannerView.Show();
    }


    /// <summary>
    /// バナー広告
    /// </summary>
    /// 

    private string adUnitId_banner;
    private BannerView bannerView;

    private void RequestBanner()
    {
#if UNITY_ANDROID
        //adUnitId_banner = "ca-app-pub-3940256099942544/6300978111";  //　テスト
        adUnitId_banner = "ca-app-pub-1567966195595585/4197618276"; // 本番
#elif UNITY_IPHONE
        //adUnitId_banner = "ca-app-pub-3940256099942544/2934735716";  //　テスト
        adUnitId_banner = "ca-app-pub-1567966195595585/7370576529"; // 本番
#else
        string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the bottom of the screen.
        //this.bannerView = new BannerView(adUnitId_banner, AdSize.Banner, AdPosition.Bottom);

        AdSize adaptiveSize =
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        this.bannerView = new BannerView(adUnitId_banner, adaptiveSize, AdPosition.Bottom);


        /*deviceIds.Add("4f4be85c66b74e9b8d4b8e63ff278e93");

        RequestConfiguration requestConfiguration = new RequestConfiguration
        .Builder()
        .SetTestDeviceIds(deviceIds)
        .build();
        
        MobileAds.SetRequestConfiguration(requestConfiguration);
        */

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);
    }

    //アクションシーンに遷移したときに呼ばれて、バナー広告を非表示にする
    public void Hide_Banner()
    {
        bannerView.Hide();
    }


    /// <summary>
    /// リワード広告広告
    /// </summary>
    /// 

    private string adUnitId_reward;
    private RewardedAd rewardedAd;

    private void RequestReward()
    {
#if UNITY_ANDROID
        //adUnitId_reward = "ca-app-pub-3940256099942544/5224354917";  //テスト
        adUnitId_reward = "ca-app-pub-1567966195595585/3211839669";  //本番
#elif UNITY_IOS
        //adUnitId_reward = "ca-app-pub-3940256099942544/1712485313";  //テスト
        adUnitId_reward = "ca-app-pub-1567966195595585/4880144552";  //本番
#endif
        this.rewardedAd = new RewardedAd(adUnitId_reward);
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        AdRequest request = new AdRequest.Builder().Build();
        //
        this.rewardedAd.LoadAd(request);
    }


    //動画の視聴が完了したら実行される（途中で閉じられた場合は呼ばれない）
    public void HandleUserEarnedReward(object sender, Reward args)
    {
        Debug.Log("報酬獲得！");
        button_manager.Get_Coin_After_Ad();
        RequestReward();
    }

    //これを呼べば動画が流れる（例えばボタン押下時など）
    public void ShowReawrd()
    {
        if (this.rewardedAd.IsLoaded())
        {
            this.rewardedAd.Show();
        }
    }

}