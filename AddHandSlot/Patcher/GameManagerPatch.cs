using AddHandSlot.Line;
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
        
        foreach (var ctrl in LineCtrl.GetLines())
        {
            ctrl.CheckStatus();
        }
    }

    [HarmonyPostfix, HarmonyPatch("ChangeStatValue")]
    public static IEnumerator ChangeStatValue_Postfix(IEnumerator result, InGameStat _Stat)
    {
        while (result.MoveNext()) yield return result.Current;

        StatCtrl.OnStatValueChange(_Stat);
    }

    [HarmonyPostfix, HarmonyPatch("ChangeEnvironment"), HarmonyPriority(Priority.Last)]
    public static IEnumerator ChangeEnvironment_Postfix(IEnumerator result)
    {
        while (result.MoveNext()) yield return result.Current;
    
        LineCtrl.OnChangeEnvironment();
    }
}