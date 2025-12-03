using Microsoft.Xna.Framework;
using System.ComponentModel;
using System.Reflection;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
namespace AutoReforge
{
	class Config : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[DefaultValue(10)]
		[Range(1, 100)]
		[Increment(1)]
		public int ReforgePerSec { get; set; }

		[DefaultValue(true)]
		public bool HideDefaultReforgeMenu { get; set; }

		[DefaultValue(false)]
		public bool Lock { get; set; }

		[DefaultValue(typeof(Vector2), "35, 340")]
		[Range(0, 1920f)]
		public Vector2 UIPosition { get; set; }
    }
}
