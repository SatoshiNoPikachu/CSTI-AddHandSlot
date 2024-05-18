using AddHandSlot.Stat;
using HarmonyLib;

namespace AddHandSlot.Patcher;

[HarmonyPatch(typeof(StatStatusGraphics))]
public class StatStatusGraphicsPatch
{
    [HarmonyPostfix, HarmonyPatch("Awake")]
    public static void Awake_Postfix(StatStatusGraphics __instance)
    {
        StatBarCtrl.AddBar(__instance);
    }
}