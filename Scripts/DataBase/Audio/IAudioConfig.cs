using AudioSystem;
using UnityEngine;

namespace DataBase
{
    public interface IAudioConfig
    {
        AudioClip MenuMusicClip { get; }
        AudioClip RacingMusicClip { get; }
        AudioClip MenuAmbientClip { get; }
        AudioClip RacingAmbientClip { get; }       
        float CrossFadeTime { get; }
        float FadeOutTime { get; }
        AudioClip GetAudioClip(string audioClipName, out SoundType soundType);
    }
}