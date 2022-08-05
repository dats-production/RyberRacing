using System.Threading;
using AudioSystem.Interfaces;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace AudioSystem
{
    public class RandomSoundPlayer : MonoBehaviour, IRandomSoundPlayer
    {
        [SerializeField] private AudioClip[] _audioClips;
        [SerializeField] private SoundType _soundType = SoundType.Sfx;
        [SerializeField] private bool atPoint;

        private AudioService _audioService;
        private CancellationTokenSource _cancellationTokenSource = new();
        
        [Inject]
        public void Construct(AudioService audioService)
        {
            _audioService = audioService;
        }
        
        public async void Play()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var audioClip = _audioClips[Random.Range(0, _audioClips.Length)];
            if (atPoint)
                await _audioService.PlaySoundAtPoint(audioClip, transform.position, cancellationToken: _cancellationTokenSource.Token);
            else
                await _audioService.PlaySound(audioClip, _soundType, cancellationToken: _cancellationTokenSource.Token);
        }

        private void OnDisable()
        {
            _cancellationTokenSource.Cancel();
        }
        
        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}