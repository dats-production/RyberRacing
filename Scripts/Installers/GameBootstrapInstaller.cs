using System.Collections.Generic;
using Analytics;
using Bootstrap;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameBootstrapInstaller : MonoInstaller
    {
        [SerializeField] private GameObject splashScreen;
        [SerializeField] private List<GameLoadingStep> loadingSteps;
        
        public override void InstallBindings()
        {
#if UNITY_EDITOR ||  UNITY_STANDALONE
            Container.Bind<IAnalyticsServiceWrapper>().To<UnityAnalyticsService>().AsSingle().NonLazy();
#else
            //other platforms implementations
#endif
            foreach (var step in loadingSteps)
                Container.QueueForInject(step);
            
            Container.BindInterfacesAndSelfTo<List<GameLoadingStep>>().FromInstance(loadingSteps);
            Container.BindInterfacesAndSelfTo<GameLoadingService>().FromNew().AsTransient();
            Container.BindInterfacesAndSelfTo<GameBootstrap>().AsSingle().WithArguments(splashScreen);
        }
    }
}
