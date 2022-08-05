using System;
using System.Threading;
using AudioSystem.Interfaces;
using Cysharp.Threading.Tasks;
using DataBase;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace AudioSystem
{
    public class AudioService : IAudioService
    {
        [Inject] private IAudioConfig _audioConfig;
        
        private readonly AudioSource _musicSource1;
        private readonly AudioSource _musicSource2;
        private readonly AudioSource _ambientSource1;
        private readonly AudioSource _ambientSource2;
        private readonly AudioMixer _audioMixer;
        private readonly AudioSourcesPool _audioSourcesPool;

        private readonly AudioMixerGroup _sfxAudioMixerGroup;
        private readonly AudioMixerGroup _uiAudioMixerGroup;
        private bool _isPlayingMusicSource1;
        private bool _isPlayingAmbientSource1;
        
        [Inject]
        private AudioService(AudioSource musicSource1, AudioSource musicSource2,
        AudioSource ambientSource1, AudioSource ambientSource2, AudioMixer audioMixer, AudioSourcesPool audioSourcesPool)
        {
            _musicSource1 = musicSource1;
            _musicSource2 = musicSource2;
            _ambientSource1 = ambientSource1;
            _ambientSource2 = ambientSource2;
            _audioMixer = audioMixer;
            _audioSourcesPool = audioSourcesPool;
            
            foreach (var group in _audioMixer.FindMatchingGroups(""))
            {
                switch (group.name)
                {
                    case "Music":
                        _musicSource1.outputAudioMixerGroup = group;
                        _musicSource2.outputAudioMixerGroup = group;
                        
                        break;
                    case "Sfx":
                        _ambientSource1.outputAudioMixerGroup = group;
                        _ambientSource2.outputAudioMixerGroup = group;
                        _sfxAudioMixerGroup = group;
                        break;
                    case "Ui":
                        _uiAudioMixerGroup = group;
                        break;
                }
            }
        }

        public void PlayMusic(SceneType sceneType)
        {
            switch (sceneType)
            {
                case SceneType.Menu:
                    SwapClip(_audioConfig.MenuMusicClip, _isPlayingMusicSource1, _musicSource1, _musicSource2);
                    break;
                case SceneType.Racing:
                    SwapClip(_audioConfig.RacingMusicClip, _isPlayingMusicSource1, _musicSource1, _musicSource2);
                    break;
            }
            
            _isPlayingMusicSource1 = !_isPlayingMusicSource1;
        }
        
        public void PlayAmbient(SceneType sceneType)
        {
            switch (sceneType)
            {
                case SceneType.Menu:
                    SwapClip(_audioConfig.MenuAmbientClip, _isPlayingAmbientSource1, _ambientSource1, _ambientSource2);
                    break;
                case SceneType.Racing:
                    SwapClip(_audioConfig.RacingAmbientClip, _isPlayingAmbientSource1, _ambientSource1, _ambientSource2);
                    break;
            }

            _isPlayingAmbientSource1 = !_isPlayingAmbientSource1;
        }       

        public void TogglePauseMusic(bool isPaused) => 
            TogglePauseAudioSource(_isPlayingMusicSource1, _musicSource1, _musicSource2, isPaused);

        public void TogglePauseAmbient(bool isPaused) => 
            TogglePauseAudioSource(_isPlayingAmbientSource1, _ambientSource1, _ambientSource2, isPaused);
        
        public void SetMusicVolume(float volume) =>
            _audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);

        public void SetSfxVolume(float volume) =>
            _audioMixer.SetFloat("SfxVolume", Mathf.Log10(volume) * 20);
        
        public void SetUiVolume(float volume) =>
            _audioMixer.SetFloat("UiVolume", Mathf.Log10(volume) * 20);

        public async UniTask PlaySound(AudioClip audioClip, SoundType soundType, float volume = 1, bool loop = false,
            CancellationToken cancellationToken = default)
        {
            var soundAudioSource = _audioSourcesPool.GetAudioSource();
            soundAudioSource.outputAudioMixerGroup = soundType switch
            {
                SoundType.Sfx => _sfxAudioMixerGroup,
                SoundType.Ui => _uiAudioMixerGroup,
                _ => throw new ArgumentOutOfRangeException(nameof(soundType), soundType, null)
            };
            soundAudioSource.loop = loop;
            soundAudioSource.clip = audioClip;
            soundAudioSource.volume = volume;
            soundAudioSource.Play();
            
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(soundAudioSource.clip.length),
                    cancellationToken: cancellationToken);
            }
            catch
            {
                if (cancellationToken.IsCancellationRequested) soundAudioSource.Stop();
            }
        }

        public async UniTask PlaySoundAtPoint(AudioClip audioClip, Vector3 position,
            float volume = 1, bool loop = false, CancellationToken cancellationToken = default)
        {
            var sfsAudioSourceAtPoint = _audioSourcesPool.GetAudioSourceAtPoint(position);
            sfsAudioSourceAtPoint.outputAudioMixerGroup = _sfxAudioMixerGroup;
            sfsAudioSourceAtPoint.loop = loop;
            sfsAudioSourceAtPoint.clip = audioClip;
            sfsAudioSourceAtPoint.volume = volume;
            sfsAudioSourceAtPoint.Play();
            
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(sfsAudioSourceAtPoint.clip.length),
                    cancellationToken: cancellationToken);
            }
            catch
            {
                if (cancellationToken.IsCancellationRequested) sfsAudioSourceAtPoint.Stop();
            }
        }
        
        private async void SwapClip(AudioClip newClip, bool isPlayingAudioSource1, AudioSource audioSource1, AudioSource audioSource2)
        {
            float timeElapsed = 0;
            var timeToFade = _audioConfig.CrossFadeTime;
        
            if (isPlayingAudioSource1)
            {
                audioSource2.clip = newClip;
                audioSource2.Play();

                while (timeElapsed < timeToFade)
                {
                    audioSource2.volume = Mathf.Lerp(0, 1, timeElapsed / timeToFade);
                    audioSource1.volume = Mathf.Lerp(1, 0, timeElapsed / timeToFade);
                    timeElapsed += Time.deltaTime;
                    await UniTask.NextFrame();
                }
                audioSource1.Stop();
            }
            else
            {
                audioSource1.clip = newClip;
                audioSource1.Play();
            
                while (timeElapsed < timeToFade)
                {
                    audioSource1.volume = Mathf.Lerp(0, 1, timeElapsed / timeToFade);
                    audioSource2.volume = Mathf.Lerp(1, 0, timeElapsed / timeToFade);
                    timeElapsed += Time.deltaTime;
                    await UniTask.NextFrame();
                }   
                audioSource2.Stop();
            }
        }
        
        private void TogglePauseAudioSource(bool isPlayingAudioSource1, AudioSource audioSource1, 
            AudioSource audioSource2, bool isPaused) =>
            FadeOut(isPlayingAudioSource1 ? audioSource1 : audioSource2, isPaused);

        private async void FadeOut(AudioSource audioSource, bool isPaused)
        {
            if (!audioSource.isPlaying == isPaused) return;
            
            float timeElapsed = 0;
            var timeToFade = _audioConfig.FadeOutTime;
            
            if (!isPaused) audioSource.UnPause();
        
            while (timeElapsed < timeToFade)
            {
                audioSource.volume = isPaused 
                    ? Mathf.Lerp(1, 0, timeElapsed / timeToFade) 
                    : Mathf.Lerp(0, 1, timeElapsed / timeToFade);

                timeElapsed += Time.deltaTime;
                await UniTask.NextFrame();
            }
            
            if (isPaused) audioSource.Pause();
        }
    }
}
