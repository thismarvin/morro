﻿using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    abstract class FX : IDisposable
    {        
        public Effect Effect { get; private set; }
        public EffectType EffectType { get; private set; }        

        public FX(EffectType type)
        {
            EffectType = type;

            switch (EffectType)
            {
                case EffectType.Blur:
                    Effect = EffectManager.Blur.Clone();
                    break;
                case EffectType.ChromaticAberration:
                    Effect = EffectManager.ChromaticAberration.Clone();
                    break;
                case EffectType.Dither:
                    Effect = EffectManager.Dither.Clone();
                    break;
                case EffectType.DropShadow:
                    Effect = EffectManager.DropShadow.Clone();
                    break;
                case EffectType.Grayscale:
                    Effect = EffectManager.Grayscale.Clone();
                    break;
                case EffectType.Invert:
                    Effect = EffectManager.Invert.Clone();
                    break;
                case EffectType.Outline:
                    Effect = EffectManager.Outline.Clone();
                    break;
                case EffectType.Palette:
                    Effect = EffectManager.Palette.Clone();
                    break;
                case EffectType.Quantize:
                    Effect = EffectManager.Quantize.Clone();
                    break;
            }
        }

        public void Reset()
        {
            Initialize();
        }

        protected abstract void Initialize();    

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Effect.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
               
                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~FX()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}