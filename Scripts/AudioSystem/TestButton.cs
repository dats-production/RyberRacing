using System.Threading;
using DataBase;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace AudioSystem
{
    public class TestButton : MonoBehaviour
    {
        private IAudioConfig _audioConfig;
        private AudioService _audioService;
        [SerializeField] private Slider _slider1;
        [SerializeField] private Slider _slider2;
        private bool isMusicPaused;
        private bool isAmbientPaused;
        
        CancellationTokenSource cancellationTokenSource1 = new();
        CancellationTokenSource cancellationTokenSource2 = new();
        
        [Inject]
        public void Construct(IAudioConfig audioConfig, AudioService audioService)
        {
            _audioConfig = audioConfig;
            _audioService = audioService;
            //_slider1.onValueChanged.AsObservable().Subscribe((x) => _audioService.SetMusicVolume(x)).AddTo(this);
            //_slider2.onValueChanged.AsObservable().Subscribe((x) => _audioService.SetSfxVolume(x)).AddTo(this);
        }

        public void PlayMusic(SceneType sceneType)
        {
            _audioService.PlayMusic(sceneType);
        }
        
        public void PlayAmbient(SceneType sceneType)
        {
            _audioService.PlayAmbient(sceneType);
        }
        
        public void PauseMusic()
        {
            _audioService.TogglePauseAmbient(true);
        }
        
        public void PauseAmbient()
        {
            _audioService.TogglePauseAmbient(false);
        }

        public async void PlayeSfx1()
        {
            cancellationTokenSource1 = new();
            var audioClip = _audioConfig.GetAudioClip("B1", out var soundType);
            await _audioService.PlaySound(audioClip, soundType, cancellationToken : cancellationTokenSource1.Token);
        }
        public async void PlaySfxAtPoint()
        {
            cancellationTokenSource2 = new();
            var audioClip = _audioConfig.GetAudioClip("B2", out var soundType);
            await _audioService.PlaySound(audioClip, soundType, cancellationToken : cancellationTokenSource2.Token);
        }
        public void PauseSound1()
        {
            cancellationTokenSource1.Cancel();
        }
        public void PauseSound2()
        {
            cancellationTokenSource2.Cancel();
        }
    }
}