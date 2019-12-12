using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Morro.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    static class SoundManager
    {
        public static float MasterVolume 
        { 
            get { return SoundEffect.MasterVolume; }
            set { SoundEffect.MasterVolume = value; }
        }

        private static AudioEmitter audioEmitter;
        private static AudioListener audioListener;
        private static SoundEffectInstance soundEffectInstance;

        internal static void Initialize()
        {
            MasterVolume = 0.75f;

            audioEmitter = new AudioEmitter();
            audioListener = new AudioListener();
        }

        /// <summary>
        /// Plays a <see cref="SoundEffect"/>.
        /// </summary>
        /// <param name="sound">The name of <see cref="SoundEffect"/> that was loaded via <see cref="AssetManager.LoadSoundEffect(string, string)"/>.</param>
        /// <param name="volume">The <see cref="SoundEffect"/>'s unique volume constrained within 0.0f and 1.0f.</param>
        public static void PlaySoundEffect(string sound, float volume)
        {
            AssetManager.GetSoundEffect(sound).Play(volume, 0, 0);
        }

        /// <summary>
        /// Plays a <see cref="SoundEffect"/> that is processed in 3D space.
        /// </summary>
        /// <param name="sound">The name of <see cref="SoundEffect"/> that was loaded via <see cref="AssetManager.LoadSoundEffect(string, string)"/>.</param>
        /// <param name="volume">The <see cref="SoundEffect"/>'s unique volume constrained within 0.0f and 1.0f.</param>
        /// <param name="theta">The vertical angle from the z-axis.</param>
        /// <param name="azimuth">The horizontal angle from the x-axis.</param>
        /// <param name="distance">How far away the <see cref="SoundEffect"/> is from the listener.</param>
        public static void PlaySoundEffect3D(string sound, float volume, float theta, float azimuth, float distance)
        {
            PlaySoundEffect3D(sound, volume, Vector3.Zero, VectorHelper.PolarToCartesian(distance, theta, azimuth));
        }

        /// <summary>
        /// Plays a <see cref="SoundEffect"/> that is processed in 3D space.
        /// </summary>
        /// <param name="sound">The name of <see cref="SoundEffect"/> that was loaded via <see cref="AssetManager.LoadSoundEffect(string, string)"/>.</param>
        /// <param name="volume">The <see cref="SoundEffect"/>'s unique volume constrained within 0.0f and 1.0f.</param>
        /// <param name="listenerPosition">The position of the audio listener.</param>
        /// <param name="emitterPosition">The position of the audio emitter.</param>
        public static void PlaySoundEffect3D(string sound, float volume, Vector3 listenerPosition, Vector3 emitterPosition)
        {
            soundEffectInstance = AssetManager.GetSoundEffect(sound).CreateInstance();

            audioListener.Position = listenerPosition;
            audioEmitter.Position = emitterPosition;

            float constrainedVolume = volume;
            constrainedVolume = constrainedVolume < 0 ? 0 : constrainedVolume;
            constrainedVolume = constrainedVolume > 1 ? 1 : constrainedVolume;

            soundEffectInstance.Volume = constrainedVolume;
            soundEffectInstance.Apply3D(audioListener, audioEmitter);
            soundEffectInstance.Play();
        }
    }
}
