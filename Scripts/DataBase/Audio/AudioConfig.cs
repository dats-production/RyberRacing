using AudioSystem;
using UnityEngine;

namespace DataBase
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "Game/Config/AudioConfig", order = 0)]
    public class AudioConfig : ScriptableObject, IAudioConfig
    {
        [SerializeField] private AudioClip menuMusicClip;
        [SerializeField] private AudioClip racingMusicClip;
        [SerializeField] private AudioClip menuAmbientClip;
        [SerializeField] private AudioClip racingAmbientClip;
        [SerializeField] private float crossFadeTime;
        [SerializeField] private float fadeOutTime;
        [SerializeField] private Sound[] sfxAudioClips;
        
        public AudioClip MenuMusicClip => menuMusicClip;
        public AudioClip RacingMusicClip => racingMusicClip;        
        public AudioClip MenuAmbientClip => menuAmbientClip;
        public AudioClip RacingAmbientClip => racingAmbientClip;
        public float CrossFadeTime => crossFadeTime;
        public float FadeOutTime => fadeOutTime;

        public AudioClip GetAudioClip(string audioClipName, out SoundType soundType)
        {
            foreach (var audioClip in sfxAudioClips)
            {
                if (audioClip.Name != audioClipName) continue;
                soundType = audioClip.SoundType;
                return audioClip.AudioClip;
            }

            Debug.LogWarning($"AudioClip with name {audioClipName} does not exist");
            soundType = default;
            return default;
        }
    }
}