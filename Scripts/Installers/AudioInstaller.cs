using AudioSystem;
using DataBase;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace Installers
{
    public class AudioInstaller : MonoInstaller
    {
        [SerializeField] private AudioConfig audioConfig;
        [SerializeField] private AudioSource musicSource1;
        [SerializeField] private AudioSource musicSource2;
        [SerializeField] private AudioSource ambientSource1;
        [SerializeField] private AudioSource ambientSource2;
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioSourcesPool audioSourcesPool;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AudioService>().FromNew().AsSingle()
                .WithArguments(musicSource1, musicSource2, ambientSource1, ambientSource2, audioMixer, audioSourcesPool);
            Container.Bind<IAudioConfig>().FromInstance(audioConfig).AsSingle();
        }
    }
}
