//广告相关
using System.Collections.Generic;
using komal;
using komal.puremvc;
using komal.sdk;
// using Newtonsoft.Json;
using UnityEngine;
public enum adType {
    chuizi,
    _double,
    relive,
}
class adOneData {
    public int timeCdLevel = 0; //广告冷却档次，越高冷却越久
    public float cdTimeVal = 0; //记录广告冷却时间
    public adOneData (int timeCdLevel = 0, float cdTimeVal = 0) {
        this.timeCdLevel = timeCdLevel;
        this.cdTimeVal = cdTimeVal;
    }
}
public class AdProxy : Proxy {
    public static string chuiziAdData = "chuiziAdData";
    public static string doubleAdData = "doubleAdData";
    public static string reliveAdData = "reliveAdData";
    int maxTimeCdLevel = 30; //广告冷却最大等级
    float cdTime = 1; //每秒更新
    float cdTimeVal = 0; //更新间隔
    public bool isPlayingAd = false; //是否正在播放广告 //不管用
    private bool isRewarded = false;
    adOneData chuizi;
    adOneData _double;
    public float adDisTime = 30f; //广告cd
    public float adTimeVal = 0f; //广告
    // GameContrProxy GameContrProxy;
    public override void OnRegister () {
        // GameContrProxy = this.facade.RetrieveProxy (ProxyNameEnum.GameContrProxy) as GameContrProxy;
    }
    public AdProxy (string name, object obj) : base (name, obj) {
        chuizi = KomalUtil.Instance.GetItem<adOneData> (AdProxy.chuiziAdData, new adOneData ());
        _double = KomalUtil.Instance.GetItem<adOneData> (AdProxy.doubleAdData, new adOneData ());
    }
    //广告播放
    public void playAdByType (adType type, System.Action successCb, System.Action disPlayCb = null, System.Action failedCb = null) {
        switch (type) {
            case adType.chuizi:
                ShowRewardedVideo (chuizi, successCb, disPlayCb);
                break;
            case adType._double:
                ShowRewardedVideo (_double, successCb, disPlayCb);
                break;
            case adType.relive:
                ShowRewardedVideo (null, successCb, disPlayCb, failedCb);
                break;
        }
    }
    //插屏广告 不可提前关闭的广告，没有奖励，不用cd，用于复活等 
    void ShowInterstitial (System.Action disMissCb, System.Action disPlayCb = null) {
        if (isPlayingAd) {
            return;
        }
        SDKManager.Instance.ShowInterstitial ((komal.sdk.ADResult.InterstitialResult result) => {
            switch (result) {
                case komal.sdk.ADResult.InterstitialResult.UNAVAILABLE:
                    KomalUtil.Instance.ShowMessage ("Message", "Ad play failed！");
                    isPlayingAd = false;
                    break;
                case komal.sdk.ADResult.InterstitialResult.DISPLAY:
                    isPlayingAd = true;
                    if (disPlayCb != null) {
                        disPlayCb ();
                    }
                    break;
                case komal.sdk.ADResult.InterstitialResult.DISMISS:
                    disMissCb ();
                    isPlayingAd = false;
                    adTimeVal = 0;
                    break;
            }
        });
    }
    //激励广告 可提前关闭的广告//有奖励，提前关闭不给奖励
    void ShowRewardedVideo (adOneData data, System.Action rewardCb, System.Action disPlayCb = null, System.Action failedCb = null) {
        if (isPlayingAd) {
            return;
        }
        isRewarded = false;
        SDKManager.Instance.ShowRewardedVideo ((komal.sdk.ADResult.RewardedVideoResult result) => {
            switch (result) {
                case komal.sdk.ADResult.RewardedVideoResult.UNAVAILABLE:
                    KomalUtil.Instance.ShowMessage ("Message", "Ad play failed！");
                    isPlayingAd = false;
                    if (failedCb != null) {
                        failedCb ();
                    }
                    break;
                case komal.sdk.ADResult.RewardedVideoResult.DISPLAY:
                    isPlayingAd = true;
                    if (disPlayCb != null) {
                        disPlayCb ();
                    }
                    break;
                case komal.sdk.ADResult.RewardedVideoResult.DISMISS:
                    isPlayingAd = false;
                    if (isRewarded) {
                        rewardCb ();
                    }
                    adTimeVal = 0;
                    if (data != null) {
                        data.timeCdLevel = Mathf.Min (maxTimeCdLevel, data.timeCdLevel + 1);
                        data.cdTimeVal = 0;
                        localStory ();
                    }
                    break;
                case komal.sdk.ADResult.RewardedVideoResult.REWARDED:
                    isRewarded = true;
                    break;
            }
        });
    }

    //检测广告是否可以播放了
    public float getLeftTime (adType type) {
        switch (type) {
            case adType.chuizi:
                return (float) countNeetTimeByLv (chuizi.timeCdLevel) - chuizi.cdTimeVal;
            case adType._double:
                return (float) countNeetTimeByLv (_double.timeCdLevel) - _double.cdTimeVal;
            default:
                return 0;
        }
    }
    //更新
    public void Update (float dt) {
		adTimeVal += dt;
        cdTimeVal += dt;
        if (cdTimeVal > cdTime && !isPlayingAd) {
            cdTimeVal = 0;
            refreshAllAd ();
        }
    }
    //刷新
    void refreshAllAd () {
        // chuizi.cdTimeVal += 1;
        // _double.cdTimeVal += 1;
        // this.SendNotification ("adUpdate");
        // localStory ();
    }
    //根据等级计算广告cd
    //本地数据存储
    void localStory () {
        KomalUtil.Instance.SetItem (AdProxy.chuiziAdData, chuizi);
        KomalUtil.Instance.SetItem (AdProxy.doubleAdData, _double);
    }
    //根据等级计算广告cd时间
    int countNeetTimeByLv (int lv) {
        return lv * 30;
    }
    //显示banner
    public void ShowBanner () {
        if (!SDKManager.Instance.IsAdsRemoved ()) {
            SDKManager.Instance.ShowBanner ();
        }
    }
    //隐藏banner
    public void HideBanner () {
        SDKManager.Instance.HideBanner ();
    }
    //去广告
    public void RemoveAds () {
        SDKManager.Instance.RemoveAds ();
    }
    //恢复购买
    public void Restore () {
        if (SDKManager.Instance.IsSupportRestorePurchases ()) {
            SDKManager.Instance.RestorePurchases ();
        }
    }
    //点击暂停时的广告
    public void onTouchStop () {
        // KomalUtil.Instance.ShowMessage ("IAP", SDKManager.Instance.IsAdsRemoved ().ToString ());
        if (SDKManager.Instance.IsAdsRemoved ()) {
            return;
        }
        bool isAd = Random.Range (0, 2) == 1 && adTimeVal > adDisTime; //50%
        if (isAd) {
            ShowInterstitial (() => { });
        }
    }
    //升级时的广告
    public void onLevelUp () {
        // int level = GameContrProxy.level;
        // bool isAd = false;
        // if (level >= 4 && level <= 20) {
        //     isAd = (level - 1) % 3 == 0;
        // } else if (level >= 21 && level <= 50) {
        //     isAd = (level - 1) % 2 == 0;
        // } else if (level >= 51) {
        //     isAd = true;
        // }
        // if (isAd) {
        //     ShowInterstitial (() => {
        //         bool isFiveStr = GameContrProxy.level % 10 == 0;
        //         if (isFiveStr) {
        //             SDKManager.Instance.FiveStars ();
        //         }
        //     });
        // }
    }

}