using System;
using System.Collections.Generic;

namespace komal.sdk {
    public static class MSG_GAMECENTER {
        public const string GAMECENTER_LOGIN_SUCCESS = "GAMECENTER_LOGIN_SUCCESS";
    }
    public class SDKManager: puremvc.Singleton<SDKManager>, ILifeCycle, IDebugLog, IAD,  IGameCenter, IIAP, IAnalytics, IPush
    {
        private List<SDKBase> m_SDKList = new List<SDKBase>();
        private bool m_isInitialized = false;
        private IDebugLog m_ProxyDebugLog = null;
        private IAD m_ProxyAD = null;
        private IGameCenter m_ProxyGameCenter = null;
        private IIAP m_ProxyIAP = null;
        private List<IAnalytics> m_AnalyticList = new List<IAnalytics>();
        private IPush m_ProxyPush = null; 
        private puremvc.IFacade facade => puremvc.Facade.getInstance();

        ////////////////////////////////////////////////////////////////
        // 调试接口 IDebugLog 
        ////////////////////////////////////////////////////////////////
        public string GetLogFileFullPath(){ return m_ProxyDebugLog.GetLogFileFullPath(); }
        public string GetRunTimeLogText() { return m_ProxyDebugLog.GetRunTimeLogText(); }


        ////////////////////////////////////////////////////////////////
        // 广告接口 IAD
        ////////////////////////////////////////////////////////////////
        public void ShowBanner(){ m_ProxyAD.ShowBanner(); }
        public void HideBanner(){ m_ProxyAD.HideBanner(); }
        public void ShowInterstitial(System.Action<ADResult.InterstitialResult> callback){ m_ProxyAD.ShowInterstitial(callback); }
        public void ShowRewardedVideo(System.Action<ADResult.RewardedVideoResult> callback){ m_ProxyAD.ShowRewardedVideo(callback); }
        public bool IsDisplayingAD(){ return m_ProxyAD.IsDisplayingAD(); }
        public bool IsRewardedVideoAvailable(){ return m_ProxyAD.IsRewardedVideoAvailable(); }
        public bool IsInterstitialAvailable(){ return m_ProxyAD.IsInterstitialAvailable(); }
        public void ValidateIntegration() { m_ProxyAD.ValidateIntegration(); }

        ////////////////////////////////////////////////////////////////
        // 游戏中心接口 IGameCenter
        ////////////////////////////////////////////////////////////////
        public void Login(Action<bool, string> callback = null) {
            m_ProxyGameCenter.Login((success, info)=>{
                // 派发登陆成功消息
                if(callback != null){ callback(success, info); }
                if(success){
                    this.facade.SendNotification(MSG_GAMECENTER.GAMECENTER_LOGIN_SUCCESS, info);
                }
            }); 
        }
        public bool IsAuthenticated() { return m_ProxyGameCenter.IsAuthenticated(); }
        public void OpenGameCenter() { m_ProxyGameCenter.OpenGameCenter(); }
        public void RecordRank(string key, int value, System.Action<bool> callback = null) { m_ProxyGameCenter.RecordRank(key, value, callback); }
        public void RecordAchievement(string key, double value, System.Action<bool> callback = null) { m_ProxyGameCenter.RecordAchievement(key, value, callback); }
        public void FiveStars(){ m_ProxyGameCenter.FiveStars(); }

        ////////////////////////////////////////////////////////////////
        // 内置购买接口 IIAP
        ////////////////////////////////////////////////////////////////
        public void Purchase(string productKey){ m_ProxyIAP.Purchase(productKey); }
        public bool IsPurchased(string productKey) { return m_ProxyIAP.IsPurchased(productKey); }
        public void RemoveAds() { m_ProxyIAP.RemoveAds(); }
        public bool IsAdsRemoved() { return m_ProxyIAP.IsAdsRemoved(); }
        public void RestorePurchases(){ m_ProxyIAP.RestorePurchases(); }
        public bool IsSupportRestorePurchases(){ return m_ProxyIAP.IsSupportRestorePurchases(); }
        public bool IsPurchasing(){ return m_ProxyIAP.IsPurchasing(); }

        ////////////////////////////////////////////////////////////////
        // 统计分析接口 IAnalytics
        ////////////////////////////////////////////////////////////////
        public void SendLaunchEvent() { m_AnalyticList.ForEach(analytic => analytic.SendLaunchEvent()); }
        public void SendTerminateEvent() { m_AnalyticList.ForEach(analytic => analytic.SendTerminateEvent()); }
        public void SendBeginePageEvent(string pageName) { m_AnalyticList.ForEach(analytic => analytic.SendBeginePageEvent(pageName)); }
        public void SendEndPageEvent(string pageName) { m_AnalyticList.ForEach(analytic => analytic.SendEndPageEvent(pageName)); }

        //////////////////////////////////////////////////////////////
        //// 推送接口
        //////////////////////////////////////////////////////////////
        public void NotificationMessage(string message, DateTime newDate, bool isRepeatDay) {if(m_ProxyPush != null){ m_ProxyPush.NotificationMessage(message, newDate, isRepeatDay); }}

        ////////////////////////////////////////////////////////////////
        // 单例接口 Singleton
        ////////////////////////////////////////////////////////////////
        public override void OnSingletonInit(){
            OnInit();
        }

        ////////////////////////////////////////////////////////////////
        // 生命周期接口 ILifeCycle
        ////////////////////////////////////////////////////////////////
        public void OnInit()
        {
            // load all sdk
            if(!m_isInitialized){
                m_isInitialized = true;

                // 日志记录模块
                var debugLogSDK = new DebugLogSDK();
                m_SDKList.Add(debugLogSDK);
                this.m_ProxyDebugLog = debugLogSDK;

                // 广告模块
#if (UNITY_ANDROID || UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
                var ironSourceSDK = new IronSourceSDK();
                m_SDKList.Add(ironSourceSDK);
                m_ProxyAD = ironSourceSDK;
#else
                var adSimulatorSDK = new ADSimulatorSDK();
                m_SDKList.Add(adSimulatorSDK);
                m_ProxyAD = adSimulatorSDK;
#endif

#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
            // iOS GameCenter SDK
                var _iOSGameCenter = new iOSGameCenter();
                m_SDKList.Add(_iOSGameCenter);
                this.m_ProxyGameCenter = _iOSGameCenter;
#elif UNITY_ANDROID && !UNITY_EDITOR
                // android GameCenter SDK
                var _androidGameCenter = new AndroidGameCenter();
                m_SDKList.Add(_androidGameCenter);
                this.m_ProxyGameCenter = _androidGameCenter;
#else
                // simulator GameCenter SDK
                var _simulatorGameCenter = new SimulatorGameCenter();
                m_SDKList.Add(_simulatorGameCenter);
                this.m_ProxyGameCenter = _simulatorGameCenter;
#endif

                // IAP
                var iapSDK = new IAPSDK();
                m_SDKList.Add(iapSDK);
                this.m_ProxyIAP = iapSDK;

                // IAnalytics
#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
                // AppsFlyer SDK
                var appsFlyerValue = Config.ID.GetValue("AppsFlyerKey");
                if(!(appsFlyerValue == null || appsFlyerValue == "")){
                    var appsFlyerAnalyticsSDK = new AppsFlyerAnalyticsSDK();
                    m_SDKList.Add(appsFlyerAnalyticsSDK);
                    m_AnalyticList.Add(appsFlyerAnalyticsSDK);
                }
#endif

                // 推送
#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
                var pushSDK = new iOSPush();
                m_SDKList.Add(pushSDK);
                m_ProxyPush = pushSDK;
#endif

                m_SDKList.ForEach(sdk=>{
                    sdk.OnInit();
                });
            }
        }

        public void OnPause() { m_SDKList.ForEach(sdk=>sdk.OnPause()); }
        public void OnResume() { m_SDKList.ForEach(sdk=>sdk.OnResume()); }
        public void OnDestroy() { m_SDKList.ForEach(sdk=>sdk.OnDestroy()); }
        public void OnUpdate(){ m_SDKList.ForEach(sdk=>sdk.OnUpdate()); }
    }
}
