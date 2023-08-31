using DresserMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheGeneralsTraining
{
    public static class Dresser
    {
        public static readonly Attire rescueBro = new Attire("TGT_RescueBro", ResourcesController.AssetsFolder)
        {
            wearer = nameof(RescueBro),
            clothes = new Dictionary<string, string>()
            {
                { "sprite", "RescueBroFlexier.png" }
            }
        };
        public static readonly Attire desperabro = new Attire("TGT_Desperabro", ResourcesController.AssetsFolder)
        {
            wearer = nameof(Desperabro),
            clothes = new Dictionary<string, string>()
            {
                { "player.hud.avatar", "avatar_desperabro.png" }
            }
        };

        public static readonly Attire broffy = new Attire("TGT_Broffy", ResourcesController.AssetsFolder)
		{
			wearer = "Broffy",
			clothes = new Dictionary<string, string>
			{
				{
					"sprite",
					"broffy_anim.png"
				}
			}
		};

        public static void Intitialize()
        {
            AddToWardrobe(rescueBro);
            AddToWardrobe(desperabro);
			AddToWardrobe(broffy);
        }

        public static void AddToWardrobe(Attire attire)
        {
            if (!StorageRoom.wardrobes.ContainsKey(attire.wearer))
            {
                StorageRoom.wardrobes.Add(attire.wearer, new Wardrobe(attire.wearer));
            }
            StorageRoom.wardrobes[attire.wearer].AddAttire(attire);
        }
    }
}
