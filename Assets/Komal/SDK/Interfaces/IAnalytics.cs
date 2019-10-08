/* Brief: Interface for Analytics
 * Author: Komal
 * Date: "2019-07-23"
 */

namespace komal.sdk
{    
    public interface IAnalytics {
        void SendLaunchEvent();
        void SendTerminateEvent();
        void SendBeginePageEvent(string pageName);
        void SendEndPageEvent(string pageName);
    }
}
