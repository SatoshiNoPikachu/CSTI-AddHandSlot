// using System.Collections.Generic;
// using HarmonyLib;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace AddHandSlot.Patcher;
//
// [Harmony]
// public class OtherPatch
// {
//     public static readonly Dictionary<GameObject, int> ChildDictionary = new();
//
//     [HarmonyPrefix, HarmonyPatch(typeof(HorizontalLayoutGroup), nameof(HorizontalLayoutGroup.SetLayoutVertical))]
//     public static void SetLayoutVertical(HorizontalLayoutGroup __instance)
//     {
//         ChildDictionary.Clear();
//         __instance.SetLayoutHorizontal();
//     }
//
//     [HarmonyPrefix,
//      HarmonyPatch(typeof(LayoutGroup), "SetChildAlongAxisWithScale", typeof(RectTransform)
//          , typeof(int), typeof(float), typeof(float))]
//     public static void SetChildAlongAxisWithScale(LayoutGroup __instance, RectTransform rect,
//         int axis, ref float pos, float scaleFactor)
//     {
//         if (GraphicsManager.Instance == null) return;
//         if (GraphicsManager.Instance.BlueprintModelsPopup == null) return;
//         if (GraphicsManager.Instance.BlueprintModelsPopup.LockedBlueprintsParent == null) return;
//         if (__instance != GraphicsManager.Instance.BlueprintModelsPopup.LockedBlueprintsParent.gameObject
//                 .GetComponent<HorizontalLayoutGroup>()) return;
//         if (axis == 0)
//         {
//             var horizontalLayoutGroup = (HorizontalLayoutGroup)__instance;
//             var size = rect.sizeDelta[axis] * scaleFactor + horizontalLayoutGroup.spacing;
//             var _index = (int)(pos / size);
//             ChildDictionary[rect.gameObject] = _index;
//             pos = (_index / 2) * size;
//         }
//         else
//         {
//             var i = ChildDictionary.TryGetValue(rect.gameObject, out var v) ? v : 0;
//             pos += i % 2 == 1 ? 157 : -168;
//         }
//     }
// }