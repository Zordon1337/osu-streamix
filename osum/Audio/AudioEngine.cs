﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using osum.Support;

namespace osum.Audio
{
    enum OsuSamples
    {
        HitNormal,
        HitWhistle,
        HitClap,
        HitFinish,
        MenuHit,
        SliderTick,
        SliderSlide,
        MenuClick,
        MenuBack
    }

    internal static class AudioEngine
    {
        static Dictionary<OsuSamples, int> loadedSamples = new Dictionary<OsuSamples, int>();
        
        internal static ISoundEffectPlayer Effect;
        internal static IBackgroundAudioPlayer Music;


        /// <summary>
        /// Initializes the audio subsystem using specific implementations for sound effects and music modules.
        /// </summary>
        /// <param name="effect">The effect player.</param>
        /// <param name="music">The music player.</param>
        internal static void Initialize(ISoundEffectPlayer effect, IBackgroundAudioPlayer music)
        {
            Effect = effect;
            Music = music;
        }

        internal static int PlaySample(OsuSamples sample)
        {
            int buffer;

            string filename = null;

            string setName = "soft";

            switch (sample)
            {
                case OsuSamples.HitClap:
                    filename = setName + "-hitclap";
                    break;
                case OsuSamples.HitFinish:
                    filename = setName + "-hitfinish";
                    break;
                case OsuSamples.HitNormal:
                    filename = setName + "-hitnormal";
                    break;
                case OsuSamples.HitWhistle:
                    filename = setName + "-hitwhistle";
                    break;
                case OsuSamples.SliderTick:
                    filename = setName + "-slidertick";
                    break;
                case OsuSamples.SliderSlide:
                    filename = setName + "-sliderslider";
                    break;
                case OsuSamples.MenuHit:
                    filename = "menuhit";
                    break;
                case OsuSamples.MenuBack:
                    filename = "menuback";
                    break;
                case OsuSamples.MenuClick:
                    filename = "menuclick";
                    break;
            }

            if (filename == null) return -1;

            if (!loadedSamples.TryGetValue(sample, out buffer))
            {
                buffer = AudioEngine.Effect.Load("Skins/Default/" + filename + ".wav");
                loadedSamples.Add(sample, buffer);
            }

            return AudioEngine.Effect.PlayBuffer(buffer);
        }
    }
}
    