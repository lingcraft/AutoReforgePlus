using GadgetBox.GadgetUI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace AutoReforge
{
    internal class AutoReforgeSystems : ModSystem
    {
        private int lastSeenScreenWidth;
        private int lastSeenScreenHeight;
        private UserInterface userInterface;
        public override void OnModLoad()
        {
            userInterface = AutoReforge.Instance.userInterface;
        }
        public override void UpdateUI(GameTime gameTime)
        {
            if (AutoReforge.Instance.isOpenReforgeMenu && AutoReforge.Instance.isInReforgeMenu == false)
            {
                userInterface.SetState(new ReforgeMachineUI());
                AutoReforge.Instance.isInReforgeMenu = true;
            }
            if (userInterface != null)
            {
                userInterface.Update(gameTime);
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "BestModifierRoll: MyInterface",
                    delegate
                    {
                        if (Main.playerInventory && !Main.recBigList)
                        {
                            if (lastSeenScreenWidth != Main.screenWidth || lastSeenScreenHeight != Main.screenHeight ||
                                Main.hasFocus)
                            {
                                userInterface.Recalculate();
                                lastSeenScreenWidth = Main.screenWidth;
                                lastSeenScreenHeight = Main.screenHeight;
                            }

                            userInterface.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
                        }

                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}
