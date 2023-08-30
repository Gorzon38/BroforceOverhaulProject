using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerroristC4Programs.Extensions;

namespace TerroristC4Programs.Patches.Snake
{
    [HarmonyPatch(typeof(AlienFaceHugger), "Start")]
    static class SnakeAreTerroristSide_Patch
    {
        static void Postfix(AlienFaceHugger __instance)
        {
            if (__instance.IsSnake())
                __instance.playerNum = (__instance.firingPlayerNum = -1);
        }
    }
}
