using AddHandSlot.Stat;
using HarmonyLib;

namespace AddHandSlot.Patcher;

[HarmonyPatch(typeof(GameLoad))]
public static class GameLoadPatch
{
    [HarmonyPostfix, HarmonyPatch("LoadMainGameData")]
    public static void LoadMainGameData_Postfix()
    {
        PerkCtrl.ModifyAddHandSlotPerkNum();

        StatCtrl.ModifyEncumbranceLimit();
    }
}