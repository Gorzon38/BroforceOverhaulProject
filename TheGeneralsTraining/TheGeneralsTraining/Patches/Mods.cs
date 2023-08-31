using DresserMod;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheGeneralsTraining.Patches.Mods
{
    [HarmonyPatch(typeof(StorageRoom), "Init")]
    static class DresserMod_Patch
    {
        static void Postfix()
        {
            Dresser.Intitialize();
        }
    }
}
