using GadgetBox;
using GadgetBox.GadgetUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Timers;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.Utilities;

namespace AutoReforge;

public class AutoReforge : Mod
{
    private class DisplayNameUpdater : ModSystem
    {
        public override void OnLocalizationsLoaded()
        {
            Instance.DisplayName = GetText("ModName");
        }
    }

    public static AutoReforge Instance => ModContent.GetInstance<AutoReforge>();

    public bool isInReforgeMenu;

    public bool isOpenReforgeMenu;

    public UserInterface userInterface;

    public override void Load()
    {
        if (!Main.dedServ)
        {
            userInterface = new UserInterface();
        }
    }
}