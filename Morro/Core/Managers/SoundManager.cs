using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    static class SoundManager
    {
        public static float MasterVolume { get; set; }

        public static void Initialize()
        {
            MasterVolume = 0.5f;
        }

        public static void PlaySoundEffect(string sound, float volume)
        {
            AssetManager.GetSoundEffect(sound).Play(MasterVolume * volume, 0, 0);
        }
    }
}
