using AddHandSlot.Line;
using HarmonyLib;
using UnityEngine;

namespace AddHandSlot.Patcher;

[HarmonyPatch(typeof(DynamicViewLayoutGroup))]
public static class DynamicViewLayoutGroupPatch
{
    [HarmonyPrefix, HarmonyPatch("UpdateList")]
    public static void UpdateList_Prefix(DynamicViewLayoutGroup __instance)
    {
        LineCtrl.OnUpdateList(__instance);
    }

    [HarmonyPostfix, HarmonyPatch("GetElementPosition")]
    public static void GetElementPosition_Postfix(DynamicViewLayoutGroup __instance, int _Index, ref Vector3 __result)
    {
        LineCtrl.OnGetElementPosition(__instance, _Index, ref __result);
    }
}