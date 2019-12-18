using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Utilities
{
    class Text : MonoObject, IDisposable
    {
        public string Content { get; private set; }

        private readonly BMFont font;
        private readonly BMFontShader shader;
        private readonly List<Sprite> sprites;

        public Text(float x, float y, string content, string fontName) : base(x, y, 1, 1)
        {
            Content = content;
            font = AssetManager.GetFont(fontName);

            sprites = new List<Sprite>();
            shader = new BMFontShader(Color.White, Color.Transparent, Color.Transparent);

            CreateText();
        }

        public void SetStyling(Color textColor, Color outlineColor, Color aaColor)
        {
            shader.SetTextColor(textColor);
            shader.SetOutlineColor(outlineColor);
            shader.SetAAColor(aaColor);

            CreateText();
        }

        private void CreateText()
        {
            sprites.Clear();

            float x = X;
            char character;
            BMFontCharacter characterData;
            Sprite sprite;
           
            for (int i = 0; i < Content.Length; i++)
            {                
                character = Content.Substring(i, 1).ToCharArray()[0];
                characterData = font.GetCharacterData(character);

                sprite = new Sprite(x + characterData.XOffset, Y + characterData.YOffset, font.FontFace + " " + (int)character)
                {
                    Effect = shader.Effect
                };
                sprites.Add(sprite);

                x += characterData.XAdvance;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            Batcher.DrawSprites(spriteBatch, sprites, camera);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    shader.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Text()
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
