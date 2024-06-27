using AddHandSlot.Line;
using HarmonyLib;

namespace AddHandSlot.Patcher;

[HarmonyPatch(typeof(CardLine))]
public static class CardLinePatch
{
    [HarmonyPrefix, HarmonyPatch("GetPointerIndex")]
    public static bool GetPointerIndex_Prefix(CardLine __instance)
    {
        return LineCtrl.IsRunGetPointerIndex(__instance);
    }
    
    [HarmonyPostfix, HarmonyPatch("GetPointerIndex")]
    public static void GetPointerIndex_Postfix(CardLine __instance, ref int __result, InGameCardBase _Card)
    {
        __result = LineCtrl.GetPointerIndex(__instance, _Card, __result);
    }
}