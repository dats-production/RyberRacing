using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Analytics
{
    public interface IAnalyticsServiceWrapper
    {
        UniTask Initialize(AnalyticsOptions analyticsOptions);
        void TrackEvent(string eventName);
        
        void TrackEvent(string eventName, string valueName, float value);
        void TrackEvent(string eventName, Dictionary<string, object> parameters);
    }
}
