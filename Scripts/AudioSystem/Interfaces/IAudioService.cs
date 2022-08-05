using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AudioSystem.Interfaces
{
    public interface IAudioService
    {
        void PlayMusic(SceneType sceneType);
        void PlayAmbient(SceneType sceneType);
        void TogglePauseMusic(bool isPaused);
        void TogglePauseAmbient(bool isPaused);
        void SetMusicVolume(float volume);
        void SetSfxVolume(float volume);
        void SetUiVolume(float volume);
        UniTask PlaySound(AudioClip audioClip, SoundType soundType, float volume = 1, bool loop = false,
            CancellationToken cancellationToken = default);
        UniTask PlaySoundAtPoint(AudioClip audioClip, Vector3 position, float volume = 1, bool loop = false,
            CancellationToken cancellationToken = default);
    }
}