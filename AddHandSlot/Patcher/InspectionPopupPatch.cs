using AddHandSlot.Line;
using HarmonyLib;

namespace AddHandSlot.Patcher;

[HarmonyPatch(typeof(InspectionPopup))]
public static class InspectionPopupPatch
{
    [HarmonyPostfix, HarmonyPatch("Setup", typeof(InGameCardBase))]
    public static void Setup_Postfix(InspectionPopup __instance)
    {
        if (__instance != GraphicsManager.Instance.InventoryInspectionPopup) return;

        var ctrl = LineCtrl.GetLine(LineType.Inventory);
        ctrl?.CheckStatus();
    }
}