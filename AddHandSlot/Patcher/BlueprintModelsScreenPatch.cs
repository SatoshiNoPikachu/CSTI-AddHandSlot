// using System.Linq;
// using HarmonyLib;
//
// namespace AddHandSlot.Patcher;
//
// [HarmonyPatch]
// public static class BlueprintModelsScreenPatch
// {
//     // [HarmonyPostfix, HarmonyPatch(typeof(BlueprintModelsScreen), "UpdateLockedBlueprints")]
//     // public static void UpdateLockedBlueprints_Postfix(BlueprintModelsScreen __instance)
//     // {
//     //     var ctrl = LineCtrl.GetLine(LineType.Blueprint);
//     //     if (ctrl is null || ctrl.Status != LineStatus.DoubleLine) return;
//     //
//     //     var line = ctrl.Line;
//     //     var args = ctrl.Args;
//     //     var views = __instance.LockedBlueprintsPreviews.Where(card => card.gameObject.activeSelf).ToList();
//     //     for (var index = 0; index < views.Count; index++)
//     //     {
//     //         var view = views[index];
//     //         view.transform.localPosition =
//     //             line.LayoutOriginPos + line.LayoutDirection * (line.Spacing * (int)(index / 2)) +
//     //             args.Padding * (index % 2) + args.Margin;
//     //     }
//     // }
// }