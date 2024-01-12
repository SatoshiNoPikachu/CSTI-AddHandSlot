using HarmonyLib;

namespace AddHandSlot.Patcher;

[Harmony]
public static class GraphicsManagerPatch
{
    [HarmonyPrefix, HarmonyPatch(typeof(GraphicsManager), "Init")]
    public static void Init_Prefix(GraphicsManager __instance)
    {
        LineCtrl.Init();
        StatBarCtrl.Init();

        foreach (var ctrl in LineCtrl.GetLines())
        {
            ctrl.CheckStatus();
        }

        StatBarCtrl.UpdateStatus();
    }

    // [HarmonyPostfix, HarmonyPatch(typeof(GraphicsManager), "Init")]
    // public static void GraphicsManager_Init_Postfix(GraphicsManager __instance)
    // {
    //     if (GameLoad.Instance.Games.Count < GameLoad.Instance.CurrentGameDataIndex ||
    //         GameLoad.Instance.CurrentGameDataIndex < 0) return;
    //
    //     GameSaveData CurrentGameData = GameLoad.Instance.Games[GameLoad.Instance.CurrentGameDataIndex].MainData;
    //
    //     if (!CurrentGameData.StatsDict.TryGetValue("3ed9754d13824a918badb45308baff0c",
    //             out StatSaveData HandSlotNum)) return;
    //
    //     int CurrentHandSlotNum = (int)HandSlotNum.BaseValue;
    //     CardLine ItemSlotsLine = __instance.ItemSlotsLine;
    //     if (CurrentHandSlotNum > 6)
    //         for (int i = ItemSlotsLine.Count; i < CurrentHandSlotNum; i++)
    //             ItemSlotsLine.AddSlot(ItemSlotsLine.Count);
    //
    //     ItemSlotsLine.RefreshSlotIndices(0);
    // }

    [HarmonyPostfix, HarmonyPatch(typeof(GraphicsManager), "UpdateSlotsVisibility")]
    public static void UpdateSlotsVisibility_Postfix()
    {
        var ctrl = new StatCtrl(StatCtrl.UidHandSlotNum);
        if (ctrl.InGame is null) return;
        if (ctrl.InGame.SimpleCurrentValue == 0) LineCtrl.ModifyHandSlotNum(0);
    }
}