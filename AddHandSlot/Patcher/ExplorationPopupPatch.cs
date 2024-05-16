using HarmonyLib;

namespace AddHandSlot.Patcher;

[HarmonyPatch(typeof(ExplorationPopup))]
public static class ExplorationPopupPatch
{
    [HarmonyPostfix, HarmonyPatch("SetSlots")]
    public static void SetSlots_Postfix()
    {
        LineCtrl.ModifyExplorationSlotNum();
    }
}