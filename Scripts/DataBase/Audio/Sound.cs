using System;
using AudioSystem;
using UnityEngine;

namespace DataBase
{
    [Serializable]
    public class Sound
    {
        public string Name;
        public SoundType SoundType;
        public AudioClip AudioClip;
    }
}