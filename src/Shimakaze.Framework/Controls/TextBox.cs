using System;
using System.Text;

using FontStashSharp;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Shimakaze.LogTracer;

namespace Shimakaze.Framework.Controls
{
    public class TextBox : TextureBlock
    {
        #region Private Fields

        private TimeSpan _barTimeSpan = TimeSpan.Zero;
        private DynamicSpriteFont? _fontCache;
        private bool _ime_enable = false;
        private bool _iME_Enable;
        private (bool left, bool right) _keyLock;
        private int TextCursorPosition = 0;

        #endregion Private Fields

        #region Protected Fields

        protected const double BAR_OFF_TIME = 0.5;

        protected const double BAR_ON_TIME = 0.5;

        #endregion Protected Fields

        #region Private Properties

        private DynamicSpriteFont FontCache
        {
            get
            {
                if (_fontCache is not null)
                    return _fontCache;
                return _fontCache = FontSystem!.GetFont(FontSize);
            }
        }

        #endregion Private Properties

        #region Public Properties

        public Color Color { get; set; } = Color.Black;

        public int FontSize { get; set; }

        public FontSystem? FontSystem { get; set; }

        public StringBuilder InputContent { get; protected set; } = new();

        public int InputPosition { get; set; }

        public override bool IsFocus
        {
            get => base.IsFocus;
            set => Set_IME_Enable((base.IsFocus = value));
        }

        public int TextEndPosition { get; set; }

        public int TextStartPosition { get; set; }

        public bool UseIME { get; set; } = true;

        #endregion Public Properties

        #region Private Methods

        private void Set_IME_Enable(bool value)
        {
            if (_iME_Enable == value)
                return;

            if (_iME_Enable = value)
            {
                TextInputEXT.TextInput += OnTextInput;
                if (UseIME)
                {
                    Application.Current!.StartTextComposition();
                    _ime_enable = true;
                }
            }
            else
            {
                if (_ime_enable)
                {
                    Application.Current!.StopTextComposition();
                    _ime_enable = false;
                }
                TextInputEXT.TextInput -= OnTextInput;
            }
        }

        #endregion Private Methods

        #region Protected Methods

        protected override bool Parse(string property, string value)
        {
            if (base.Parse(property, value))
                return true;

            switch (property)
            {
                case nameof(UseIME):
                    UseIME = StringConverter.ToBoolean(value);
                    return true;

                case nameof(FontSize):
                    FontSize = StringConverter.ToInt32(value);
                    return true;

                case nameof(FontSystem):
                    FontSystem = StringConverter.ToFontSystem(value);
                    return true;

                case nameof(Color):
                    Color = StringConverter.ToColor(value);
                    return true;

                default:
                    return false;
            }
        }

        #endregion Protected Methods

        #region Public Methods

        public override void Draw(GameTime gameTime)
        {
            if (!Visible)
                return;

            base.Draw(gameTime);

            Graphics!.DrawString(FontCache!, InputContent, TruePosition, Color);

            if (Enabled && IsFocus && _barTimeSpan.TotalSeconds < BAR_ON_TIME)
            {
                var barLocation = FontCache!.MeasureString(InputContent.ToString(0, TextCursorPosition));

                Graphics!.DrawFillRectangle(new(
                    barLocation.X + TruePosition.X,
                    TruePosition.Y + 2,
                    1,
                    barLocation.Y - 4),
                    Color.White);
            }
        }

        /// <summary>
        /// Redirect a sdl text input event.
        /// </summary>
        public virtual void OnTextInput(char ch)
        {
            if (!Enabled)
                return;

            switch ((int)ch)
            {
                case 2:
                    TextCursorPosition = 0;
                    break;

                case 3:
                    TextCursorPosition = InputContent.Length;
                    break;

                case 8:
                    if (InputContent.Length > 0 && TextCursorPosition > 0)
                    {
                        InputContent = InputContent.Remove(TextCursorPosition - 1, 1);
                        TextCursorPosition--;
                    }
                    break;

                case 0x7F:
                    if (InputContent.Length != TextCursorPosition)
                        InputContent = InputContent.Remove(TextCursorPosition, 1);
                    break;

                case 27:
                case 13:
                    //InputContent.Append(Environment.NewLine);
                    //TextCursorPosition += new Vector2(-TextCursorPosition.X, 1);
                    break;

                default:
                    Logger.Detailed($"Key Down: {(int)ch,8:X} {ch}");
                    InputContent.Append(ch);
                    TextCursorPosition++;
                    break;
            }
            _barTimeSpan = TimeSpan.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;

            base.Update(gameTime);

            _barTimeSpan += gameTime.ElapsedGameTime;

            if (_barTimeSpan > TimeSpan.FromSeconds(BAR_ON_TIME + BAR_OFF_TIME))
                _barTimeSpan -= TimeSpan.FromSeconds(BAR_ON_TIME + BAR_OFF_TIME);

            if (_keyLock.left && Input.IsKeyUp(Keys.Left))
                _keyLock.left = false;
            else if (_keyLock.right && Input.IsKeyUp(Keys.Right))
                _keyLock.right = false;
            else if (!_keyLock.left && Input.IsKeyDown(Keys.Left) && TextCursorPosition > 0)
            {
                _keyLock.left = true;
                TextCursorPosition--;
            }
            else if (!_keyLock.right && Input.IsKeyDown(Keys.Right) && TextCursorPosition < InputContent.Length)
            {
                _keyLock.right = true;
                TextCursorPosition++;
            }
        }

        #endregion Public Methods
    }
}