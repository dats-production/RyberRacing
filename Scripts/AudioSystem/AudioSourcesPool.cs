using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AudioSystem
{
    public class AudioSourcesPool : MonoBehaviour
    {
        [SerializeField] private int initialPoolSize;
        private readonly List<AudioSource> _audioSources = new();
        private readonly List<AudioSource> _audioSourcesAtPoint = new();

        private void Awake()
        {
            for (var i = 0; i < initialPoolSize; i++)
            {
                AddAudioSource();
                AddAudioSourceAtPoint(Vector3.zero);
            }
        }

        public AudioSource GetAudioSource()
        {
            return _audioSources.FirstOrDefault(audioSource => !audioSource.isPlaying) ?? AddAudioSource();
        }

        public AudioSource GetAudioSourceAtPoint(Vector3 position)
        {
            var audioSourceAtPoint = _audioSourcesAtPoint.FirstOrDefault(audioSource => !audioSource.isPlaying) ?? AddAudioSourceAtPoint(position);
            audioSourceAtPoint.transform.position = position;
            return audioSourceAtPoint;
        }
        
        private AudioSource AddAudioSource()
        {
            var newAudioSource = gameObject.AddComponent<AudioSource>();
            _audioSources.Add(newAudioSource);
            return newAudioSource;
        } 
        
        private AudioSource AddAudioSourceAtPoint(Vector3 position)
        {
            var soundAtPointGo = new GameObject("Sound At Point")
            {
                transform =
                {
                    parent = transform,
                    position = position
                }
            };

            var newAudioSourceAtPoint = soundAtPointGo.AddComponent<AudioSource>();
            newAudioSourceAtPoint.spatialBlend = 1;
            _audioSourcesAtPoint.Add(newAudioSourceAtPoint);
            return newAudioSourceAtPoint;
        }
    }
}