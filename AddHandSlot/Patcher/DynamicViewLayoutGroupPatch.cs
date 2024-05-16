using HarmonyLib;
using UnityEngine;

namespace AddHandSlot.Patcher;

[HarmonyPatch(typeof(DynamicViewLayoutGroup))]
public static class DynamicViewLayoutGroupPatch
{
    [HarmonyPostfix, HarmonyPatch("GetElementPosition")]
    public static void GetElementPosition_Postfix(DynamicViewLayoutGroup __instance, int _Index, ref Vector3 __result)
    {
        if (__instance is not CardLine line) return;

        var type = LineCtrl.ResolveLineType(line);
        if (type is null) return;

        var ctrl = LineCtrl.GetLine((LineType)type);
        if (ctrl is null) return;

        if (ctrl.Status == LineStatus.DoubleLine) ctrl.DoubleLine(_Index, ref __result);
    }
}