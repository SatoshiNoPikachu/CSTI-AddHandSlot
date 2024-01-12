using HarmonyLib;

namespace AddHandSlot.Patcher;

[Harmony]
public static class GameLoadPatch
{
    [HarmonyPostfix, HarmonyPatch(typeof(GameLoad), "LoadMainGameData")]
    public static void LoadMainGameData_Postfix()
    {
        PerkCtrl.ModifyAddHandSlotPerkNum();

        StatCtrl.ModifyEncumbranceLimit();
    }
}