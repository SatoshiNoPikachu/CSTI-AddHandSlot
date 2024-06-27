using AddHandSlot.Blueprint;
using AddHandSlot.Line;
using HarmonyLib;

namespace AddHandSlot.Patcher;

[HarmonyPatch(typeof(BlueprintModelsScreen))]
public static class BlueprintModelsScreenPatch
{
    [HarmonyPostfix, HarmonyPatch("Awake")]
    public static void Awake_Postfix(BlueprintModelsScreen __instance)
    {
        BlueprintTabCtrl.OnBlueprintModelsScreenAwake(__instance);
        LockedBlueprintCtrl.OnBlueprintModelsScreenAwake(__instance);
    }

    [HarmonyPostfix, HarmonyPatch("Show")]
    public static void Show_Postfix()
    {
        BlueprintTabCtrl.Instance.OnShow();
    }
}