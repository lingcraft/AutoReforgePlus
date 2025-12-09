using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutoReforge;

class AutoReforgeGoblin : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.GoblinTinkerer;
    }

    public override bool PreChatButtonClicked(NPC npc, bool firstButton)
    {
        if (!firstButton)
        {
            Main.npcChatText = "";
            AutoReforge.Instance.isOpenReforgeMenu = true;
        }
        if (GetConfig().HideDefaultReforgeMenu)
        {
            return firstButton;
        }
        return true;
    }
}