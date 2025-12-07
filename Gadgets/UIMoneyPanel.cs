using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using GadgetBox;
using System;
using AutoReforge;

namespace GadgetBox.GadgetUI
{
	class UIMoneyPanel : UIPanel
	{
		public bool Visible { get; internal set; }
		Color textColor = Color.White;
		string text = GetText("MoneyUI.Reserve.Label");
		public UICounterButton[] CounterButtons { get; private set; } = new UICounterButton[4];
		public override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (!Visible)
			{
				return;
			}
			base.DrawSelf(spriteBatch);

		}
		public override void DrawChildren(SpriteBatch spriteBatch)
		{
			if (!Visible)
			{
				return;
			}

			var pos = GetInnerDimensions().Position() - new Vector2(FontAssets.MouseText.Value.MeasureString(text).X, 4);
			if (IsChinese())
			{
				pos -= new Vector2(38, 0);
			}
			Utils.DrawBorderString(spriteBatch, text, pos, Colors.AlphaDarken(textColor));
			base.DrawChildren(spriteBatch);
		}
		public int GetMoneyValue()
		{
			return CounterButtons[0].Counter * Item.copper + CounterButtons[1].Counter * Item.silver
				+ CounterButtons[2].Counter * Item.gold + CounterButtons[3].Counter * Item.platinum;
		}
		public override void OnInitialize()
		{
			MinWidth.Set(150, 0);
			MinHeight.Set(40, 0);
			Width.Set(150, 0);
			Height.Set(40, 0);
			int padding = 24;
			Player player = Main.CurrentPlayer;
			UICounterButton platinumButton = new UICounterButton(TextureAssets.Coin[3]);
			platinumButton.Left.Set(26, 0);
			UICounterButton goldButton = new UICounterButton(TextureAssets.Coin[2]);
			goldButton.Left.Set(platinumButton.Left.Pixels + padding, 0);
			UICounterButton silverButton = new UICounterButton(TextureAssets.Coin[1]);
			silverButton.Left.Set(goldButton.Left.Pixels + padding, 0);
			//xd
			UICounterButton copperButton = new UICounterButton(TextureAssets.Coin[0]);
			copperButton.Left.Set(silverButton.Left.Pixels + padding, 0);

			CounterButtons[3] = platinumButton;
			CounterButtons[2] = goldButton;
			CounterButtons[1] = silverButton;
			CounterButtons[0] = copperButton;

			Append(platinumButton);
			Append(goldButton);
			Append(silverButton);
			Append(copperButton);
		}
	}
	class UICounterButton : UIImageButton
	{
		public int Counter { get; private set; }
		private UIText counterText;
		public UICounterButton(Asset<Texture2D> texture, int defaultValue = 0) : base(texture)
		{
			OnScrollWheel += (a, b) => ChangeCounterOnScrool(a.ScrollWheelValue);
			Counter = defaultValue;
			counterText = new UIText(Counter.ToString(), .75f);
			counterText.Top.Set(5, 0);
			Append(counterText);
		}
		public override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (IsMouseHovering)
			{
				Main.hoverItemName = GetText("MoneyUI.Reserve.Tooltip"); ;
			}
			base.DrawSelf(spriteBatch);
		}
		private void ChangeCounterOnScrool(int scroll)
		{
			int value = Counter + (scroll > 0 ? 1 : -1);
			value = value < 0 ? 99 : value > 99 ? 0 : value;
			SetCounter(value);
		}
		public void SetCounter(int value)
		{
			Counter = value;
			counterText.SetText(Counter.ToString());
		}
	}
}
