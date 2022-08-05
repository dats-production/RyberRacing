using AddressableHandling;
using Zenject;

namespace Installers
{
    public class ServicesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<AddressablesDownloadingProvider>().AsSingle();
            Container.BindInterfacesAndSelfTo<Localization.Localization>().AsSingle();
        }
    }
}
