using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.ComponentModel;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace GadgetBox.GadgetUI
{
    internal class UIFancyButton : UIHoverText
    {
        private Asset<Texture2D> _texture;
        private Asset<Texture2D> _hoverTexture;
        private bool _isClicking;
        private float _clickScale;
        public bool Visible { get; internal set; } = true;
		public float Rotation { get; internal set; }
        internal event Func<bool> CanClick;
        private bool _isMenuButton = false;

		public UIFancyButton(Asset<Texture2D> texture, Asset<Texture2D> hoverTexture, float clickScale = 0.85f) : base()
        {
            _texture = texture;
            _hoverTexture = hoverTexture;
            _clickScale = clickScale;
			Width.Set(_texture.Width(), 0f);
            Height.Set(_texture.Height(), 0f);
        }

        public UIFancyButton(Asset<Texture2D> texture, float clickScale = 1f) : this(texture, texture, clickScale)
        {
	        _isMenuButton = true;
        }

		public void SetImage(Asset<Texture2D> texture)
		{
			_texture = texture;
			_hoverTexture = texture;
			Width.Set(_texture.Width(), 0f);
			Height.Set(_texture.Height(), 0f);
		}

		public override void LeftMouseDown(UIMouseEvent evt)
        {
            if (!Visible)
            {
                return;
            }

            _isClicking = true;
            if (CanClick?.Invoke() ?? true)
            {
                base.LeftMouseDown(evt);
            }
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            if (!Visible)
            {
                return;
            }

            if (_isClicking)
            {
                _isClicking = false;
            }

            base.LeftMouseUp(evt);
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            if (!Visible)
            {
                return;
            }

            if (_isClicking)
            {
                _isClicking = false;
            }

            base.MouseOut(evt);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            if (!Visible)
            {
                return;
            }

            base.MouseOver(evt);
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (!Visible)
            {
                return;
            }

            if (!_isMenuButton)
            {
	            Asset<Texture2D> texture = IsMouseHovering ? _hoverTexture : _texture;
	            float scale = _isClicking ? _clickScale : 1f;
	            Vector2 origin = texture.Size() * 0.5f * scale;
	            spriteBatch.Draw(texture.Value, GetDimensions().Position() + origin, null, Color.White, Rotation,
		            origin, scale, SpriteEffects.None, 0);
            }
            else
            {
				spriteBatch.Draw(_texture.Value, GetDimensions().Position(), Color.White * (IsMouseHovering ? 1f : 0.4f));
			}
            base.DrawSelf(spriteBatch);
		}
    }
}