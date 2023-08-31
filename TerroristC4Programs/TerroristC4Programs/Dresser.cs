using DresserMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerroristC4Programs
{
    public static class Dresser
    {
        public static readonly Attire mookTrooper = new Attire("TC4P_MookTrooper", ResourcesController.AssetsFolder)
        {
            wearer = nameof(MookTrooper),
            clothes = new Dictionary<string, string>()
            {
                { "decapitatedMaterial", "MookTrooper_decapitated.png" }
            }
        };
        public static readonly Attire mookStrong = new Attire("TC4P_StrongTrooper", ResourcesController.AssetsFolder)
        {
            wearer = Wearers.MOOK_STRONG,
            clothes = new Dictionary<string, string>()
            {
                { "decapitatedMaterial", "MookStrong_decapitated.png" }
            }
        };

        public static void Intitialize()
        {
            AddToWardrobe(mookTrooper);
        }

        public static void AddToWardrobe(Attire attire)
        {
            if (!StorageRoom.wardrobes.ContainsKey(attire.wearer))
            {
                StorageRoom.wardrobes.Add(attire.wearer, new Wardrobe(attire.wearer));
            }
            StorageRoom.wardrobes[attire.wearer].AddAttire(attire);
        }

        public static string GetDecapitationFileName(Mook mook)
        {
            if (mook as AlienMosquito || mook as DolphLundrenSoldier || mook as MookCaptainCutscene || mook as MookDog || mook as MookHellBoomer || mook as SatanMiniboss || mook as Warlock)
                return string.Empty;

            return Wearers.GetWearerName(mook) + "_decapitated.png";
        }

        public static string GetSkinnedFileName(Mook mook)
        {
            if (mook.skinnedPrefab == null)
                return string.Empty;

            if (mook as MookJetpack)
                return nameof(MookJetpack) + "_skinless.png";
            if (mook as UndeadTrooper)
                return nameof(UndeadTrooper) + "_skinless.png";
            if (mook as MookTrooper || mook as MookSuicide || mook as MookRiotShield || mook as ScoutMook)
                return nameof(Mook) + "_skinless.png";

            return string.Empty;
        }
    }
}
