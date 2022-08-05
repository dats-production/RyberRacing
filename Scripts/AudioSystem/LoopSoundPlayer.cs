using System.Threading;
using UnityEngine;
using Zenject;

namespace AudioSystem
{
    public class LoopSoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip _audioClip;
        [SerializeField] private bool atPoint;

        private AudioService _audioService;
        private CancellationTokenSource _cancellationTokenSource = new();

        private bool _isApplicationQuit;
        
        [Inject]
        public void Construct(AudioService audioService)
        {
            _audioService = audioService;
        }

        private async void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            if (atPoint)
                await _audioService.PlaySoundAtPoint(_audioClip, transform.position, loop: true, cancellationToken: _cancellationTokenSource.Token);
            else
                await _audioService.PlaySound(_audioClip, SoundType.Sfx, loop: true, cancellationToken: _cancellationTokenSource.Token);
        }

        private void OnDisable()
        {
            if (_isApplicationQuit) return; // is needed to avoid application quit error
            _cancellationTokenSource.Cancel();
        }
        
        private void OnDestroy()
        {
            if (_isApplicationQuit) return; // is needed to avoid application quit error
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private void OnApplicationQuit()
        {
            _isApplicationQuit = true;
        }
    }
}