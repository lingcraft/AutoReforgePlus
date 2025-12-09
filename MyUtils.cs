global using static AutoReforge.MyUtils;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;
using Terraria.UI.Chat;

namespace AutoReforge;

public class MyUtils
{
    internal static string GetText(string str, params object[] arg)
    {
        return Language.GetTextValue($"Mods.AutoReforge.{str}", arg);
    }

    internal static Asset<Texture2D> GetTexture(string name)
    {
        return ModContent.Request<Texture2D>($"AutoReforge/Images/{name}", AssetRequestMode.ImmediateLoad);
    }

    internal static void SaveConfig()
    {
        MethodInfo saveMethodInfo = typeof(ConfigManager).GetMethod("Save", BindingFlags.Static | BindingFlags.NonPublic);
        if (saveMethodInfo != null)
        {
            saveMethodInfo.Invoke(null, new object[] { ModContent.GetInstance<Config>() });
        }
        else
        {
            AutoReforge.Instance.Logger.Warn("In-game SaveConfig failed, code update required");
        }
    }

    internal static Config GetConfig()
    {
        return ModContent.GetInstance<Config>();
    }

    internal static void SavePosition(float x, float y)
    {
        GetConfig().UIPosition = new Vector2(x, y);
        SaveConfig();
    }

    internal static bool IsChinese()
    {
        return Language.ActiveCulture == GameCulture.FromCultureName(GameCulture.CultureName.Chinese);
    }

    internal static void DrawSavings(SpriteBatch sb, float shopx, float shopy, bool horizontal = false, Vector2 coinOffset = new ())
    {
        Terraria.Player player = Main.player[Main.myPlayer];
        bool overFlowing;
        long num1 = Utils.CoinsCount(out overFlowing, player.bank.item);
        long num2 = Utils.CoinsCount(out overFlowing, player.bank2.item);
        long num3 = Utils.CoinsCount(out overFlowing, player.bank3.item);
        long num4 = Utils.CoinsCount(out overFlowing, player.bank4.item);
        long count = Utils.CoinsCombineStacks(out overFlowing, num1, num2, num3, num4);
        if (count <= 0L)
        {
            return;
        }
        Texture2D itemTexture1;
        Rectangle itemFrame1;
        Main.GetItemDrawFrame(4076, out itemTexture1, out itemFrame1);
        Texture2D itemTexture2;
        Rectangle itemFrame2;
        Main.GetItemDrawFrame(3813, out itemTexture2, out itemFrame2);
        Texture2D itemTexture3;
        Rectangle itemFrame3;
        Main.GetItemDrawFrame(346, out itemTexture3, out itemFrame3);
        Texture2D itemTexture4;
        Main.GetItemDrawFrame(87, out itemTexture4, out itemFrame3);
        // 猪猪存钱罐图标
        if (num4 > 0L)
        {
            sb.Draw(itemTexture1, Utils.CenteredRectangle(new Vector2(shopx + 92f, shopy + 45f), itemFrame1.Size() * 0.65f), new Rectangle?(), Color.White);
        }
        if (num3 > 0L)
        {
            sb.Draw(itemTexture2, Utils.CenteredRectangle(new Vector2(shopx + 92f, shopy + 45f), itemFrame2.Size() * 0.65f), new Rectangle?(), Color.White);
        }
        if (num2 > 0L)
        {
            sb.Draw(itemTexture3, Utils.CenteredRectangle(new Vector2(shopx + 80f, shopy + 50f), itemTexture3.Size() * 0.65f), new Rectangle?(), Color.White);
        }
        if (num1 > 0L)
        {
            sb.Draw(itemTexture4, Utils.CenteredRectangle(new Vector2(shopx + 70f, shopy + 54f), itemTexture4.Size() * 0.65f), new Rectangle?(), Color.White);
        }
        // 钱币
        DrawMoney(sb, Lang.inter[66].Value, shopx, shopy, Utils.CoinsSplit(count), horizontal, coinOffset);
    }

    internal static void DrawMoney(SpriteBatch sb, string text, float shopx, float shopy, int[] coinsArray, bool horizontal = false, Vector2 coinOffset = new())
    {
        // 钱币类型：花费、存款
        Utils.DrawBorderStringFourWay(sb, FontAssets.MouseText.Value, text, shopx, shopy + 40f, Color.White * ((float)Main.mouseTextColor / (float)byte.MaxValue), Color.Black, Vector2.Zero);
        if (horizontal)
        {
            for (int index = 0; index < 4; ++index)
            {
                Main.instance.LoadItem(74 - index);
                if (index == 0)
                {
                    int coins = coinsArray[3 - index];
                }
                Vector2 position = new Vector2((float)((double)shopx + (double)ChatManager.GetStringSize(FontAssets.MouseText.Value, text, Vector2.One).X + (double)(24 * index) + 45.0), shopy + 50f);
                // 钱币图标
                sb.Draw(TextureAssets.Item[74 - index].Value, position + coinOffset, new Rectangle?(), Color.White, 0.0f, TextureAssets.Item[74 - index].Value.Size() / 2f, 1f, SpriteEffects.None, 0.0f);
                // 钱币数量
                Utils.DrawBorderStringFourWay(sb, FontAssets.ItemStack.Value, coinsArray[3 - index].ToString(), position.X - 11f + coinOffset.X, position.Y + coinOffset.Y, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
            }
        }
        else
        {
            for (int index = 0; index < 4; ++index)
            {
                Main.instance.LoadItem(74 - index);
                int num = index != 0 || coinsArray[3 - index] <= 99 ? 0 : -6;
                sb.Draw(TextureAssets.Item[74 - index].Value, new Vector2(shopx + 11f + (float)(24 * index), shopy + 75f) + coinOffset, new Rectangle?(), Color.White, 0.0f, TextureAssets.Item[74 - index].Value.Size() / 2f, 1f, SpriteEffects.None, 0.0f);
                Utils.DrawBorderStringFourWay(sb, FontAssets.ItemStack.Value, coinsArray[3 - index].ToString(), shopx + (float)(24 * index) + (float)num + coinOffset.X, shopy + 75f + coinOffset.Y, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
            }
        }
    }
}