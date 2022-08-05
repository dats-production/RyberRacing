using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Analytics;
using Bootstrap.Interfaces;
using Cysharp.Threading.Tasks;
using Localization;
using UniRx;
using Debug = UnityEngine.Debug;

namespace Bootstrap
{
    public class GameLoadingService : IGameLoadingService
    {
        private readonly List<GameLoadingStep> _steps;
        private readonly float _loadingWeight = 0;
        private float _currentLoading = 0;
        private Subject<float> _loadingProgress;
        private IAnalyticsServiceWrapper _analytics;
        
        public GameLoadingService(List<GameLoadingStep> steps, IAnalyticsServiceWrapper analytics)
        {
            _steps = steps;
            _analytics = analytics;
            _loadingWeight = steps.Where(x => x.LoadingPolicy != LoadingPolicy.Forgotten).Sum(x => x.StepWeight);
        }

        public async UniTask<bool> LoadGame(Subject<float> loadingProgress)
        {
            _loadingProgress = loadingProgress;
            var stopwatch = Stopwatch.StartNew();
            
            _steps.Sort((step1, step2) =>
            {
                if (step1.LoadingPolicy < step2.LoadingPolicy) return -1;
                if (step1.LoadingPolicy > step2.LoadingPolicy) return 1; 
                return 0;
            });
            
            foreach (var step in _steps)
            {
                switch (step.LoadingPolicy)
                {
                    case LoadingPolicy.Async:
                    case LoadingPolicy.Forgotten:
                        step.LoadStep(OnStepLoaded).Forget();
                        break;
                    case LoadingPolicy.Sync:
                        await step.LoadStep(OnStepLoaded);
                        break;
                }
            }

            await UniTask.WaitUntil(() => _currentLoading / _loadingWeight >= 1);
            stopwatch.Stop();
            _analytics.TrackEvent("gameLoaded", "loadingTime", stopwatch.ElapsedMilliseconds);
            return true;
        }

        private void OnStepLoaded(GameLoadingStep step)
        {
            if (step.LoadingPolicy == LoadingPolicy.Forgotten) return;
            
            _currentLoading += step.StepWeight;
            var progressValue = _currentLoading / _loadingWeight;
            _loadingProgress.OnNext(progressValue);
            Log($"Game loading progress : {progressValue.ToString(CultureInfo.CurrentCulture)}");
        }

        public static void Log(string message) => UnityEngine.Debug.Log($"<b><color=green>{message}</color></b>");
    }
}