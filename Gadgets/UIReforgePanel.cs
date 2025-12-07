using AutoReforge;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.ModLoader;

namespace GadgetBox.GadgetUI
{
	internal class UIReforgePanel : UIPanel
	{
		private readonly Func<Item> _reforgeItem;
		private readonly Func<int> _reforgePrice;
		private Vector2 offset;
		private bool dragging;
		public UIReforgePanel(Func<Item> reforgeItem, Func<int> reforgePrice)
		{
			_reforgeItem = reforgeItem;
			_reforgePrice = reforgePrice;
			Recalculate();
		}

		public override void Recalculate()
		{
			Width.Set(Math.Max(FontAssets.MouseText.Value.MeasureString(Language.GetTextValue("LegacyInterface.20")).X + 90, 320), 0);
			base.Recalculate();
		}

		public override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (ContainsPoint(Main.MouseScreen))
			{
				Main.LocalPlayer.mouseInterface = true;
			}
			if (dragging)
			{
				Left.Set(Main.MouseScreen.X - offset.X, 0f);
				Top.Set(Main.MouseScreen.Y - offset.Y, 0f);
				Recalculate();
			}

			base.DrawSelf(spriteBatch);
			CalculatedStyle style = GetDimensions();
			string priceText;
			Vector2 priceOffset = new Vector2(style.X + 68, style.Y + 28);
			if (!_reforgeItem().IsAir)
			{
				priceOffset += new Vector2(46, -20);
				priceText = Language.GetTextValue("LegacyInterface.46");
				float xOffset = FontAssets.MouseText.Value.MeasureString(priceText).X - 20;
				DrawMoney(spriteBatch, "", priceOffset.X + xOffset + 45, priceOffset.Y - 42, Utils.CoinsSplit(Math.Max(_reforgePrice(), 1)), true);
				DrawSavings(spriteBatch, priceOffset.X, priceOffset.Y - 14, true, IsChinese() ? new(25, 0) : Vector2.Zero);
			}
			else
			{
				priceText = Language.GetTextValue("LegacyInterface.20");
			}

			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, priceText, priceOffset, new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), 0f, Vector2.Zero, Vector2.One, -1f, 2f);
		}

		public override void LeftMouseDown(UIMouseEvent evt)
		{
			DragStart(evt);
			base.LeftMouseDown(evt);
		}

		public override void LeftMouseUp(UIMouseEvent evt)
		{
			DragEnd(evt);
			base.LeftMouseUp(evt);
		}

		private void DragStart(UIMouseEvent evt)
		{
			CalculatedStyle innerDimensions = GetInnerDimensions();
			if (evt.Target == this && !GetConfig().Lock)
			{
				offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
				dragging = true;
			}
		}

		private void DragEnd(UIMouseEvent evt)
		{
			if (evt.Target == this)
			{
				dragging = false;
				SavePosition(Left.Pixels, Top.Pixels);
			}
		}
	}
}