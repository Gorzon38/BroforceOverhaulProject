using DresserMod;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerroristC4Programs.Patches
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
