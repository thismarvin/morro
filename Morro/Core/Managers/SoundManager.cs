using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    public enum SoundType
    {
        None
    }

    static class SoundManager
    {
        public static float Volume { get; set; }

        public static void Initialize()
        {
            Volume = 0.5f;
        }

        public static void IncrementVolume(float increment)
        {
            Volume = ((int)Math.Round(Volume * 100) + increment) / 100;
            Volume = Volume > 1 ? 0 : Volume;
        }

        public static void Play(SoundType sound)
        {
            switch (sound)
            {
                case SoundType.None:
                    break;
            }
        }
    }
}
