using System.Collections;
using AddHandSlot.Line;
using AddHandSlot.Stat;
using HarmonyLib;

namespace AddHandSlot.Patcher;

[HarmonyPatch(typeof(GameManager))]
public static class GameManagerPatch
{
    [HarmonyPostfix, HarmonyPatch( "Awake")]
    public static void Awake_Postfix(GameManager __instance)
    {
        LineCtrl.ForceAddHandSlot();

        LineCtrl.ModifyHandSlotNum();
    }

    [HarmonyPostfix, HarmonyPatch("ChangeStatValue")]
    public static IEnumerator ChangeStatValue_Postfix(IEnumerator result, InGameStat _Stat)
    {
        yield return result;

        var ctrl = new StatCtrl(StatCtrl.UidHandSlotNum);
        if (_Stat != null && _Stat.StatModel != null && _Stat.StatModel == ctrl.Stat)
        {
            LineCtrl.ModifyHandSlotNum((int)_Stat.SimpleCurrentValue);
        }
    }
}