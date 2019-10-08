using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace komal.sdk {
    public class iOSPush : SDKBase, IPush
    {
        //////////////////////////////////////////////////////////////
        //// Push 功能的开关
        //////////////////////////////////////////////////////////////
        private bool m_isPushToggleOn = false;
        public override void OnInit(){
            // 从配置表初始化相关推送参数
            m_isPushToggleOn = Config.ID.GetToggle("Push");
            // 清空通知
            CleanNotification();
        }

        public override void OnPause(){
            NotificationAllMessage();
        }

        public override void OnResume(){
            CleanNotification();
        }

        public override void OnDestroy(){
            NotificationAllMessage();
        }

        private void NotificationAllMessage(){
            if(!m_isPushToggleOn){ return; }
            Config.ID.push.ForEach(it=>{
                if(it.Toggle){
                    System.DateTime date = System.DateTime.Parse(it.Date);
                    NotificationMessage(it.Text, date, it.Type == "Day");
                }
            });
        }

        public void NotificationMessage(string message, System.DateTime newDate, bool isRepeatDay)
        {
            if(!m_isPushToggleOn){ return; }
            //推送时间需要大于当前时间
            if (newDate > System.DateTime.Now)
            {
                UnityEngine.iOS.LocalNotification localNotification = new UnityEngine.iOS.LocalNotification();
                localNotification.fireDate = newDate;
                localNotification.alertBody = message;
                localNotification.applicationIconBadgeNumber = 1;
                localNotification.hasAction = true;
                if (isRepeatDay)
                {
                    //是否每天定期循环
                    localNotification.repeatCalendar = UnityEngine.iOS.CalendarIdentifier.ChineseCalendar;//中国日历
                    localNotification.repeatInterval = UnityEngine.iOS.CalendarUnit.Day;//每日推送
                }
                localNotification.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
                UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.Sound);
                //以特定的类型推送
                UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(localNotification);
            }
        }
        
        // 清除推送消息
        void CleanNotification()
        {
            UnityEngine.iOS.LocalNotification ln = new UnityEngine.iOS.LocalNotification();
            ln.applicationIconBadgeNumber = -1;
            UnityEngine.iOS.NotificationServices.PresentLocalNotificationNow(ln);
            UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
            UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
        }
    }
}