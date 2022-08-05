using Input;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Installers
{
    public class InputInstaller : MonoInstaller
    {
        [SerializeField] private InputActionAsset inputActionAsset;
        [SerializeField] private PlayerInput playerInput;
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<InputService>().FromNew().AsSingle().WithArguments(inputActionAsset, playerInput);
        }
    }
}
