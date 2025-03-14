using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutoReforge
{
    class AutoReforgeGoblin : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == NPCID.GoblinTinkerer;
        }

        public override bool PreChatButtonClicked(NPC npc, bool firstButton)
        {
            if (firstButton == false && !MyUtils.GetConfig().UseDefaultReforgeMenu)
            {
                Main.npcChatText = "";
                AutoReforge.Instance.ReforgeMenu = true;
            }
            return true;
        }
    }
}
