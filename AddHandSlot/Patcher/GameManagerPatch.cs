using System.Collections;
using AddHandSlot.Line;
using AddHandSlot.ScrollView;
using AddHandSlot.Stat;
using HarmonyLib;

namespace AddHandSlot.Patcher;

[HarmonyPatch(typeof(GameManager))]
public static class GameManagerPatch
{
    [HarmonyPostfix, HarmonyPatch("Awake")]
    public static void Awake_Postfix(GameManager __instance)
    {
        LineCtrl.ForceAddHandSlot();
        LineCtrl.ModifyHandSlotNum();

        StatCtrl.ForceModifyEncumbranceLimit();
        StatCtrl.ModifyEncumbranceLimit();

        InspectionPopupScroll.Create();
    }

    [HarmonyPostfix, HarmonyPatch("ChangeStatValue")]
    public static IEnumerator ChangeStatValue_Postfix(IEnumerator result, InGameStat _Stat)
    {
        yield return result;

        StatCtrl.OnStatValueChange(_Stat);
    }
}