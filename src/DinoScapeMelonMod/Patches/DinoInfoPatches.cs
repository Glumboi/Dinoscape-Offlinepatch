using Il2Cpp;
using Il2CppPlayFab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace DinoScapeOffline.Patches
{
    [HarmonyPatch(typeof(DinoInfo))]
    class DinoInfoPatches
    {
        [HarmonyPatch("CheckIfOwned")]
        [HarmonyPrefix]
        private static bool Prefix_CheckIfOwned(ref DinoInfo __instance, ref bool __result)
        {
            __result = true;
            __instance.owned = true;
            return false;
        }
    }
}
