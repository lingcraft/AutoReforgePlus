﻿using AutoReforge;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace GadgetBox.GadgetUI
{
    internal class ReforgeMachineUI : UIState
    {
        internal UIPanel reforgePanel;
        internal UIFancyButton lockButton;
		internal UIItemSlot reforgeSlot;
        internal UIFancyButton reforgeButton;
        internal UIPanel reforgeListPanel;
        internal UIList reforgeList;
        internal UIMoneyPanel moneyPanel;
        private List<int> selectedPrefixes = new List<int>();
        private int reforgePrice;
        private bool autoReforge;
        private double tickCounter;
        private double silenceCounter;
        private int lastItem = ItemID.None;
        private HashSet<int> allowedPrefixes = new HashSet<int>();
        private readonly int PrefixCountX2 = PrefixLoader.PrefixCount * 2;
        private readonly Item air = new Item();
        private Config config = MyUtils.GetConfig();

		public override void OnInitialize()
        {
            Main.recBigList = false;
            Main.playerInventory = true;
            Main.hidePlayerCraftingMenu = true;

			reforgePanel = new UIReforgePanel(() => reforgeSlot.item, () => reforgePrice);
            reforgePanel.SetPadding(4);
            reforgePanel.Left.Set(config.UIPosition.X, 0);
            reforgePanel.Top.Set(config.UIPosition.Y, 0);
            reforgePanel.MinHeight.Set(300, 0);

            var state = config.Lock ? "Lock" : "Unlock";
			lockButton = new UIFancyButton(MyUtils.GetTexture($"Button{state}"));
            lockButton.Left.Set(-24, 1);
            lockButton.Top.Set(1, 0);
			lockButton.HoverText = MyUtils.GetText($"ReforgeUI.{state}.Tooltip");
			lockButton.OnLeftMouseDown += OnLockButtonClick;
			reforgePanel.Append(lockButton);

            reforgeSlot = new UIItemSlot(0.85f);
            reforgeSlot.Left.Set(12, 0);
            reforgeSlot.Top.Set(12, 0);
            reforgeSlot.CanClick += () => Main.mouseItem.type == ItemID.None || Main.mouseItem.Prefix(-3);
            reforgeSlot.OnLeftMouseDown += (a, b) => { selectedPrefixes.Clear(); OnItemChanged(); };
            reforgePanel.Append(reforgeSlot);

            moneyPanel = new UIMoneyPanel();
            moneyPanel.Left.Set(170, 0);
            moneyPanel.Top.Set(48, 0);
            moneyPanel.BackgroundColor = Color.Transparent;
            moneyPanel.BorderColor = Color.Transparent;
            moneyPanel.Visible = false;
            reforgePanel.Append(moneyPanel);

            reforgeButton = new UIFancyButton(TextureAssets.Reforge[0], TextureAssets.Reforge[1]);
			reforgeButton.Left.Set(64, 0);
			reforgeButton.Top.Set(20, 0);
			reforgeButton.CanClick += CanReforgeItem;
            reforgeButton.OnLeftMouseDown += OnReforgeButtonClick;
            reforgeButton.HoverText = Language.GetTextValue("LegacyInterface.19");
            reforgePanel.Append(reforgeButton);

            reforgeListPanel = new UIPanel();
            reforgeListPanel.SetPadding(6);
            reforgeListPanel.Left.Set(12, 0);
            reforgeListPanel.Top.Set(80, 0);
            reforgeListPanel.Width.Set(-24, 1);
            reforgeListPanel.Height.Set(-82, 1);
            reforgeListPanel.BackgroundColor = Color.CadetBlue;
            reforgePanel.Append(reforgeListPanel);

            reforgeList = new UIList();
            reforgeList.Width.Set(-24, 1);
            reforgeList.Height.Set(0, 1);
			reforgeList.ListPadding = 2;
            reforgeListPanel.Append(reforgeList);

            var reforgeListScrollbar = new FixedUIScrollbar(AutoReforge.AutoReforge.Instance.userInterface);
            reforgeListScrollbar.SetView(100f, 1000f);
            reforgeListScrollbar.Left.Set(-20, 1f);
            reforgeListScrollbar.Top.Set(4, 0);
            reforgeListScrollbar.Height.Set(-8, 1f);
            reforgeListPanel.Append(reforgeListScrollbar);
            reforgeList.SetScrollbar(reforgeListScrollbar);
            Append(reforgePanel);
        }

        public override void OnDeactivate()
        {
            if (!reforgeSlot.item.IsAir)
            {
                var source = reforgeSlot.item.GetSource_DropAsItem();
                Main.LocalPlayer.QuickSpawnItem(source, reforgeSlot.item, reforgeSlot.item.stack);
                reforgeSlot.item.TurnToAir();
            }

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            bool closeUI = false, silent = false;
            if (Main.LocalPlayer.talkNPC == -1)
            {
                AutoReforge.AutoReforge.Instance.ReforgeMenu = false;
            }
            if (!AutoReforge.AutoReforge.Instance.ReforgeMenu)
            {
                closeUI = true;
            }

            if (closeUI)
            {
                if (!silent)
                {
                    SoundEngine.PlaySound(SoundID.MenuClose);
                }
                AutoReforge.AutoReforge.Instance.userInterface.SetState(null);
                AutoReforge.AutoReforge.Instance.isInReforgeMenu = false;
                AutoReforge.AutoReforge.Instance.ReforgeMenu = false;
                return;
            }

            reforgePrice = reforgeSlot.item.ReforgePrice();
            if (autoReforge)
            {

                tickCounter += gameTime.ElapsedGameTime.TotalMilliseconds;

                silenceCounter += .001f * gameTime.ElapsedGameTime.TotalMilliseconds;

                reforgeButton.Rotation += 10 * .001f * gameTime.ElapsedGameTime.Milliseconds;
                if (selectedPrefixes.Count == 0 || selectedPrefixes.Contains(reforgeSlot.item.prefix) || !CanReforgeItem())
                {
                    autoReforge = false;
                    tickCounter = 0;
                }
                else if (tickCounter > 1000 / MyUtils.GetConfig().ReforgePerSec)
                {

                    tickCounter = 0;
                    ReforgeItem(silenceCounter < .2f ? true : false);
                    if (silenceCounter > .2f) silenceCounter = 0;
                    if (selectedPrefixes.Contains(reforgeSlot.item.prefix))
                    {
                        autoReforge = false;
                        tickCounter = 0;
                    }
                }
            }
            else if (reforgeButton.Rotation != 0)
            {
                if (reforgeButton.Rotation > MathHelper.TwoPi)
                {
                    reforgeButton.Rotation %= MathHelper.TwoPi;
                }
                reforgeButton.Rotation = MathHelper.TwoPi - reforgeButton.Rotation <= 0.2f ? 0 : reforgeButton.Rotation + 0.2f;

            }
            else
            {
                silenceCounter = .2f;
            }
        }

        public override void DrawSelf(SpriteBatch spriteBatch)
        {
            Main.hidePlayerCraftingMenu = true;
            if (reforgePanel.ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
                Main.HoverItem.TurnToAir();
                Main.hoverItemName = "";
            }
            reforgeButton.Visible = !reforgeSlot.item.IsAir;
            moneyPanel.Visible = !reforgeSlot.item.IsAir;
        }

        private void OnLockButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
	        config.Lock = !config.Lock;
	        var newState = config.Lock ? "Lock" : "Unlock";
	        lockButton.SetImage(MyUtils.GetTexture($"Button{newState}"));
	        lockButton.HoverText = MyUtils.GetText($"ReforgeUI.{newState}.Tooltip");
		}

		private void OnReforgeButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (autoReforge)
            {
                autoReforge = false;
                tickCounter = 0;
            }
            else if (selectedPrefixes.Count > 0)
            {
                autoReforge = true;
            }
            else
            {
                ReforgeItem(false);
            }
        }

        private bool CanReforgeItem() => !reforgeSlot.item.IsAir && !selectedPrefixes.Contains(reforgeSlot.item.prefix) &&
            Main.LocalPlayer.CanAfford(reforgePrice + moneyPanel.GetMoneyValue(), -1);

        private void OnItemChanged()
        {
            reforgeList.Clear();
            if (reforgeSlot.item.IsAir)
            {
                return;
            }
            if (lastItem != reforgeSlot.item.type)
            {
                UpdateAllowedPrefixes();
                lastItem = reforgeSlot.item.type;
            }

            UpdateReforgeList();
        }

        private void UpdateReforgeList()
        {
            Item controlItem = reforgeSlot.item.Clone();

            controlItem.netDefaults(reforgeSlot.item.netID);
            controlItem = reforgeSlot.item.Clone();

            UIReforgeLabel reforgeLabel;
            List<int> tempSelected = new List<int>();
            foreach (var prefix in allowedPrefixes)
            {
                Item tempItem = controlItem.Clone();
                tempItem.ResetPrefix();
                tempItem.Prefix(prefix);
                reforgeLabel = new UIReforgeLabel(tempItem);
                reforgeLabel.OnLeftMouseDown += ChoseReforge;
                reforgeLabel.SetPadding(10);
                if (selectedPrefixes.Contains(prefix))
                {
                    reforgeLabel.selected = true;
                    tempSelected.Add(prefix);
                }
                reforgeList.Add(reforgeLabel);
            }
            selectedPrefixes = tempSelected;
        }
        private void UpdateAllowedPrefixes()
        {
            allowedPrefixes.Clear();
            if (IsAnyModDumbAndDoesntUseLoaderHooksCorrectlyLikeCalamity(reforgeSlot.item))
            {
                allowedPrefixes = GetPrefixesIfAnyModIsDumbAndDontUsesModLoaderHooksCorrectlyLikeCalamity();
                return;
            }
            //int attempts = PrefixLoader.PrefixCount;
            //Item tempItem = reforgeSlot.item.Clone();
            //tempItem.netDefaults(reforgeSlot.item.netID);
            //while (attempts > 0)
            //{
            //    tempItem.Prefix(-2);
            //    if (tempItem.prefix > 0 && allowedPrefixes.Add(tempItem.prefix))
            //    {
            //        attempts += PrefixLoader.PrefixCount;
            //        attempts = Math.Clamp(attempts, 0, PrefixCountX2);
            //    }
            //    attempts--;
            //}
            int prefixes = PrefixLoader.PrefixCount;
            for (int i = 0; i < prefixes; i++)
            {
                Item tempItem = reforgeSlot.item.Clone();
                tempItem.ResetPrefix();
                if (tempItem.CanRollPrefix(i) && tempItem.CanApplyPrefix(i))
                {
                    allowedPrefixes.Add(i);
                }
            }
        }
        private bool IsAnyModDumbAndDoesntUseLoaderHooksCorrectlyLikeCalamity(Item item)
        {
            Item tempItem = item.Clone();
            int prefixBeforePostReforge = tempItem.prefix;
            Main.reforgeItem = tempItem;
            ItemLoader.PostReforge(tempItem);
            Main.reforgeItem = new Item();
            return tempItem.prefix != prefixBeforePostReforge;
        }
        private HashSet<int> GetPrefixesIfAnyModIsDumbAndDontUsesModLoaderHooksCorrectlyLikeCalamity()
        {
            HashSet<int> prefixes = new HashSet<int>();
            int attempts = PrefixLoader.PrefixCount;
            Item tempItem = reforgeSlot.item.Clone();
            tempItem.netDefaults(reforgeSlot.item.netID);
            Main.reforgeItem = tempItem;
            while (attempts > 0)
            {
                ItemLoader.PostReforge(tempItem);
                if (tempItem.prefix > 0 && prefixes.Add(tempItem.prefix))
                {
                    attempts += PrefixLoader.PrefixCount;
                    attempts = Math.Clamp(attempts, 0, PrefixCountX2);
                }
                attempts--;
            }
            Main.reforgeItem = air;
            return prefixes;
        }

        private void ChoseReforge(UIMouseEvent evt, UIElement listeningElement)
        {
            UIReforgeLabel element = ((UIReforgeLabel)listeningElement);
            element.selected = !element.selected;
            if (!selectedPrefixes.Remove(element.shownItem.prefix))
            {
                selectedPrefixes.Add(element.shownItem.prefix);
            }
            reforgeList.UpdateOrder();
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        private void ReforgeItem(bool silent)
        {
            Main.LocalPlayer.BuyItem(reforgePrice, -1);
            GadgetMethods.PrefixItem(ref reforgeSlot.item, silent);
        }
	}
}