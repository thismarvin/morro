using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Morro.Utilities
{
    public enum FontType
    {
        Probity,
        Sparge,
    }

    class BitmapFont : MonoObject, IDisposable
    {
        public FontType FontType { get; private set; }
        public Vector2 Scale { get; private set; }
        public string Text { get; private set; }

        private readonly List<Sprite> sprites;
        private readonly List<string> words;
        private string[] splitWords;
        private readonly Timer timer;
        private readonly Quad debugBounds;
        private string spriteTypeFont;
        private readonly int maximumCharacterCount;
        private float textWidth;
        private float textHeight;
        private float extraHorizontalWidth;
        private float verticalPadding;
        private bool showAll;
        private bool compact;

        public float Rotation { get; private set; }

        private Outline outline;

        public BitmapFont(float x, float y, string dialogue, FontType type) : this(x, y, dialogue, new Vector2(1, 1), type, Color.White)
        {

        }

        public BitmapFont(float x, float y, string dialogue, Vector2 scale, FontType type, Color color) : base(x, y, 1, 1)
        {
            words = new List<string>();
            sprites = new List<Sprite>();
            timer = new Timer(0);            
            FontType = type;
            Text = dialogue;
            maximumCharacterCount = dialogue.Length * 20;
            compact = true;
            showAll = true;
            Scale = scale;

            ParseFontType();

            SetupOutlineEffect();

          


            debugBounds = new Quad(X, Y, 1, 1, 2, PICO8.GrassGreen, VertexInformation.Dynamic);




            BreakUpWords();
            CreateText();
            SetColor(color);
        }

        public override void SetLocation(float x, float y)
        {
            if (X == x && Y == y)
                return;

            base.SetLocation(x, y);
            CreateText();
        }

        // I'm not sure if I should disable these methods, but they actually don't do anythin for this class.
        public override void SetDimensions(int width, int height)
        {
            //base.SetDimensions(width, height);
        }

        public override void SetBounds(float x, float y, int width, int height)
        {
            //base.SetBounds(x, y, width, height);
        }

        public override void SetColor(Color color)
        {
            if (Color == color)
                return;

            base.SetColor(color);
            foreach (Sprite s in sprites)
            {
                s.SetColor(Color);
            }
        }

        public void SetRotation(float rotation)
        {
            if (Rotation == rotation)
                return;

            Rotation = rotation;

            CreateText();

            for (int i = 0; i < sprites.Count; i++)
            {
                sprites[i].Rotation = Rotation;
            }
        }

        public void SetCompact(bool compact)
        {
            if (this.compact == compact)
                return;

            this.compact = compact;
            CreateText();
        }

        public void SetScale(float xScale, float yScale)
        {
            Scale = new Vector2(xScale, yScale);
            CreateText();
        }

        public void SetText(string text)
        {
            if (Text == text)
                return;

            Text = text;
            BreakUpWords();
            CreateText();
        }

        private void ParseFontType()
        {
            switch (FontType)
            {
                case FontType.Probity:
                    spriteTypeFont = "Probity";
                    break;
                case FontType.Sparge:
                    spriteTypeFont = "Sparge";
                    break;
            }
        }

        private void SetupOutlineEffect()
        {
            outline = null;
        }

        private void BreakUpWords()
        {
            words.Clear();
            splitWords = Regex.Split(Text, "[ ]+");
            foreach (string s in splitWords)
            {
                words.Add(s);
            }
        }

        private void TextSetup()
        {
            // Maybe make this a file?
            switch (FontType)
            {
                case FontType.Probity:
                    textWidth = 8;
                    textHeight = 8;
                    extraHorizontalWidth = 1;
                    verticalPadding = 2;
                    break;
                case FontType.Sparge:
                    textWidth = 16;
                    textHeight = 16;
                    extraHorizontalWidth = 3;
                    verticalPadding = 4;
                    break;
            }

            textWidth *= Scale.X;
            extraHorizontalWidth *= Scale.X;
            textHeight *= Scale.Y;
            verticalPadding *= Scale.Y;
        }

        private void CreateText()
        {
            sprites.Clear();
            TextSetup();

            int dialougeIndex = 0;
            int lineIndex = 0;
            float wordLength = 0;
            int y = 0;

            Matrix transform;
            Vector2 result;

            foreach (string s in words)
            {
                if (s.Length + lineIndex + 1 > maximumCharacterCount)
                {
                    wordLength = 0;
                    lineIndex = 1;
                    y++;
                }

                for (int i = 0; i < s.Length; i++)
                {
                    sprites.Add(new Sprite((int)Position.X + wordLength, (int)Position.Y + ((textHeight + verticalPadding) * y), spriteTypeFont));

                    if (outline == null)
                        outline = new Outline(sprites[0].SpriteSheet);

                    sprites.Last().Effect = outline.Effect;

                    transform =
                            Matrix.CreateTranslation(-Center.X, -Center.Y, 0) *
                            Matrix.CreateRotationZ(Rotation) *
                            Matrix.CreateTranslation(Center.X, Center.Y, 0) *
                            Matrix.Identity;

                    result = Vector2.Transform(sprites.Last().Position, transform);

                    sprites.Last().SetLocation(result.X, result.Y);
                    sprites.Last().SetFrame(Text.Substring(dialougeIndex, 1).ToCharArray()[0], 16);
                    sprites.Last().SetColor(Color);
                    sprites.Last().Scale = Scale;
                    sprites.Last().Rotation = Rotation;
                    sprites.Last().Show = showAll;

                    if (compact)
                    {
                        if (s[i] == 'I' || s[i] == 'i' || s[i] == '!' || s[i] == 'l' || s[i] == '.' || s[i] == ',' || s[i] == '\'' || s[i] == ':' || s[i] == ';')
                        {
                            switch (FontType)
                            {
                                case FontType.Probity:
                                    wordLength += 3 * Scale.X;
                                    break;
                                case FontType.Sparge:
                                    wordLength += 5 * Scale.X;
                                    break;
                            }
                        }
                        else if (s[i] == 't')
                        {
                            switch (FontType)
                            {
                                case FontType.Probity:
                                    wordLength += 6 * Scale.X;
                                    break;
                                case FontType.Sparge:
                                    wordLength += 10 * Scale.X;
                                    break;
                            }
                        }
                        else if (s[i] == 'f')
                        {
                            switch (FontType)
                            {
                                case FontType.Probity:
                                    wordLength += 7 * Scale.X;
                                    break;
                                case FontType.Sparge:
                                    wordLength += 12 * Scale.X;
                                    break;
                            }
                        }
                        else
                        {
                            wordLength += textWidth - extraHorizontalWidth;
                        }
                    }
                    else
                    {
                        wordLength += textWidth - extraHorizontalWidth;
                    }

                    dialougeIndex++;
                    lineIndex++;
                }

                // Acounts for space between words.
                dialougeIndex++;
                lineIndex++;
                wordLength += textWidth;
            }

            base.SetBounds(X, Y, (int)Math.Ceiling(y == 0 ? wordLength - textWidth + extraHorizontalWidth : maximumCharacterCount * textWidth), (int)Math.Ceiling(textHeight * (y + 1)));
            debugBounds.SetBounds(X, Y, Width, Height);
            debugBounds.SetRotationOffset(Width / 2, Height / 2);
            debugBounds.SetRotation(Rotation);
        }

        public void Update()
        {
            if (timer.Done() && !showAll)
            {
                for (int i = sprites.Count - 1; i >= 0; i--)
                {
                    if (i == 0)
                    {
                        showAll = true;
                    }
                    if (!sprites[i].Show)
                    {
                        sprites[i].Show = true;
                        break;
                    }

                }
                timer.Reset();
            }
        }

        public void Draw(SpriteBatch spriteBatch, CameraType cameraType)
        {
            //foreach (Sprite sprite in sprites)
            //{
            //    sprite.Draw(spriteBatch, cameraType);
            //}

            Batcher.DrawSprites(spriteBatch, sprites, cameraType);

            if (DebugManager.ShowBoundingBoxes)
            {
                debugBounds.Draw(spriteBatch, cameraType);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    outline.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~BitmapFont()
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
