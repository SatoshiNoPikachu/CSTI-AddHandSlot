using HarmonyLib;

namespace AddHandSlot.Patcher;

[Harmony]
public class StatStatusGraphicsPatch
{
    [HarmonyPostfix, HarmonyPatch(typeof(StatStatusGraphics), "Awake")]
    public static void Awake_Postfix(StatStatusGraphics __instance)
    {
        StatBarCtrl.AddBar(__instance);
    }
}