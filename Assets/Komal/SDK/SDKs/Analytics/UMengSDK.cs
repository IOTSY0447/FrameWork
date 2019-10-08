/* Brief: 友盟统计
 * Author: Komal
 * Date: "2019-07-23"
 */
#if false
using System.Collections.Generic;
using UnityEngine;
#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using komal;
using Umeng;

namespace komal.sdk
{
    public class UMengSDK : SDKBase, IAnalytics
    {
#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
        public void SendBeginePageEvent(string pageName)
        {
            this.SendEvent(AnalyticsEvent.beginPage, new Dictionary<string, string>(){
                {"page", pageName}
            });
        }

        public void SendEndPageEvent(string pageName)
        {
            this.SendEvent(AnalyticsEvent.endPage, new Dictionary<string, string>(){
                {"page", pageName}
            });
        }

        public void SendLaunchEvent()
        {
            this.SendEvent(AnalyticsEvent.launch, null);
        }

        public void SendTerminateEvent()
        {
            this.SendEvent(AnalyticsEvent.terminate, null);
        }

        public override void OnInit() {
            GA.StartWithAppKeyAndChannelId(Config.ID.GetValue("UmengKey"), "App Store");
            this.SendLaunchEvent();
        }

        public override void OnDestroy() {
            this.SendTerminateEvent();
        }
        
#else
        public void SendBeginePageEvent(string pageName) { }
        public void SendEndPageEvent(string pageName) { }
        public void SendLaunchEvent() { }
        public void SendTerminateEvent() { }
#endif
        public string GetDeviceID(){
            return GA.GetTestDeviceInfo();
        }
/*  */
        private void SendEvent(AnalyticsEvent umengEvent, Dictionary<string, string> dict = null)
        {
            GA.Event(umengEvent.ToString(), this.CreatePrama(umengEvent.ToString(), dict));
        }

        private Dictionary<string, string> CreatePrama(string name, Dictionary<string, string> dict = null)
        {
            var curPrama = new Dictionary<string, string>();
            curPrama.Add($"{name}Net", this.NetWork);
            curPrama.Add($"{name}Time", System.DateTime.Now.ToString());
            curPrama.Add("deviceId", this.GetDeviceID());
            if(dict != null){
                foreach(KeyValuePair<string,string> entry in dict){
                    curPrama.Add(entry.Key, entry.Value);
                }
            }
            return curPrama;
        }

        private string NetWork{
            get{
                switch (Application.internetReachability)
                {
                    case NetworkReachability.ReachableViaLocalAreaNetwork:
                        // Debug.Log("当前使用的是：WiFi，请放心更新！");
                        return "wifi";
                    case NetworkReachability.ReachableViaCarrierDataNetwork:
                        // Debug.Log("当前使用的是移动网络，是否继续更新？");
                        return "net";
                    default:
                        // Debug.Log("当前没有联网，请您先联网后再进行操作！");
                        return "none";
                }
            }
        }
    }
}
#endif
