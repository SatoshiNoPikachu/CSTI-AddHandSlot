using HarmonyLib;

namespace AddHandSlot.Patcher;

[Harmony]
public static class InspectionPopupPatch
{
    [HarmonyPostfix, HarmonyPatch(typeof(InspectionPopup), "Setup", typeof(InGameCardBase))]
    public static void Setup_Postfix(InspectionPopup __instance)
    {
        if (__instance != GraphicsManager.Instance.InventoryInspectionPopup) return;

        var ctrl = LineCtrl.GetLine(LineType.Inventory);
        ctrl?.CheckStatus();
    }
}