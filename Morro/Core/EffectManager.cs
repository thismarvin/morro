using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    public enum EffectType
    {
        None,
        Blur,
        ChromaticAberration,
        Dither,
        DropShadow,
        Grayscale,
        Invert,
        Outline,
        Palette,
        Quantize,
    }

    class EffectManager
    {
        public static Effect Blur { get; private set; }
        public static Effect ChromaticAberration { get; private set; }
        public static Effect Dither { get; private set; }
        public static Effect DropShadow { get; private set; }
        public static Effect Grayscale { get; private set; }
        public static Effect Invert { get; private set; }
        public static Effect Outline { get; private set; }
        public static Effect Palette { get; private set; }
        public static Effect Quantize { get; private set; }


        public static Effect PostProcessing { get; private set; }

        public static Effect Transition { get; private set; }

        public static void LoadContent(ContentManager Content)
        {
            Blur = Content.Load<Effect>("Assets/Effects/Blur");
            ChromaticAberration = Content.Load<Effect>("Assets/Effects/ChromaticAberration");
            Dither = Content.Load<Effect>("Assets/Effects/Dither");
            DropShadow = Content.Load<Effect>("Assets/Effects/DropShadow");
            Grayscale = Content.Load<Effect>("Assets/Effects/Grayscale");
            Invert = Content.Load<Effect>("Assets/Effects/Invert");
            Outline = Content.Load<Effect>("Assets/Effects/Outline");
            Palette = Content.Load<Effect>("Assets/Effects/Palette");
            Quantize = Content.Load<Effect>("Assets/Effects/Quantize");


            Transition = Content.Load<Effect>("Assets/Effects/Transition");

            PostProcessing = Content.Load<Effect>("Assets/Effects/PostProcessing");            
        }

        public static void UnloadContent()
        {
            Blur.Dispose();
            ChromaticAberration.Dispose();
            Dither.Dispose();
            DropShadow.Dispose();
            Grayscale.Dispose();
            Invert.Dispose();
            Outline.Dispose();
            Palette.Dispose();
            Quantize.Dispose();

            PostProcessing.Dispose();
            Transition.Dispose();
        }
    }
}
