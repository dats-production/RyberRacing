using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

namespace Analytics
{
    public class UnityAnalyticsService : IAnalyticsServiceWrapper
    {
        private bool duplicateToLog;
        
        public async UniTask Initialize(AnalyticsOptions analyticsOptions)
        {
            duplicateToLog = analyticsOptions.duplicateToLog;
            var options = new InitializationOptions();
            options.SetEnvironmentName(analyticsOptions.EnviromentName);
            await UnityServices.InitializeAsync(options);
            try
            {
                List<string> consentIdentifiers = await AnalyticsService.Instance.CheckForRequiredConsents();

                var isOptInConsentRequired = false;

                if (consentIdentifiers.Count > 0)
                {
                    var consentIdentifier = consentIdentifiers[0];
                    isOptInConsentRequired = consentIdentifier == "pipl";
                    if (isOptInConsentRequired)
                    {
                        AnalyticsService.Instance.ProvideOptInConsent(consentIdentifier, false);
                    }
                }
            }
            
#if UNITY_EDITOR
            catch (ConsentCheckException)
            {
#elif !UNITY_EDITOR
            catch (ConsentCheckException e)
            {
                  Debug.LogException(e);
#endif
            }
        }

        public void TrackEvent(string eventName)
        {
            AnalyticsService.Instance.CustomData(eventName, null);
            AnalyticsService.Instance.Flush();
            Log(eventName, null);
        }

        public void TrackEvent(string eventName, string valueName, float value)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                { valueName, value }
            };
            TrackEvent(eventName, parameters);
        }

        public void TrackEvent(string eventName, Dictionary<string, object> parameters)
        {
            AnalyticsService.Instance.CustomData(eventName, parameters); 
            AnalyticsService.Instance.Flush();
            Log(eventName, parameters);
        }

        private void Log(string eventName, Dictionary<string, object> parameters)
        {
            if (!duplicateToLog) return;
            
            var paramsText = string.Empty;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    paramsText += $"{parameter.Key} {parameter.Value} \n";
                }
            }

            Debug.Log($"<i><color=grey>Custom analytics event: <color=white><b>{eventName}</b></color> with parameters:</color></i> \n {paramsText}");
        }
    }
}
