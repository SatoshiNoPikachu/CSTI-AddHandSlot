using AddHandSlot.Blueprint;
using HarmonyLib;

namespace AddHandSlot.Patcher;

[HarmonyPatch(typeof(BlueprintModelsScreen))]
public static class BlueprintModelsScreenPatch
{
    [HarmonyPostfix, HarmonyPatch("Awake")]
    public static void Awake_Postfix(BlueprintModelsScreen __instance)
    {
        LockedBlueprintCtrl.OnBlueprintModelsScreenAwake(__instance);
    }
}