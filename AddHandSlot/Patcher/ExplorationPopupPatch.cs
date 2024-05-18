using AddHandSlot.Line;
using HarmonyLib;

namespace AddHandSlot.Patcher;

[HarmonyPatch(typeof(ExplorationPopup))]
public static class ExplorationPopupPatch
{
    [HarmonyPostfix, HarmonyPatch("Setup")]
    public static void Setup_Postfix(ExplorationPopup __instance)
    {
        LineCtrl.OnExplorationPopupSetup(__instance);
    }
}