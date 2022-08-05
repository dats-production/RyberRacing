using Analytics;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Bootstrap.LoadingSteps
{
    [CreateAssetMenu(menuName = "Game/Loading/Analytics Step")]
    public class AnalyticsLoadingStep : GameLoadingStep
    {
        private IAnalyticsServiceWrapper analyticsService;
        
        [Inject]
        private void Construct(IAnalyticsServiceWrapper analyticsService)
        {
            this.analyticsService = analyticsService;
        }
        
        protected override async UniTask LoadStepInternal()
        {
            await analyticsService.Initialize(new AnalyticsOptions("development") {duplicateToLog = true});
        }
    }
}
