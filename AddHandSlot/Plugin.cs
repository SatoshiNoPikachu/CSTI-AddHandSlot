using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace AddHandSlot;

[BepInPlugin("Pikachu.AddHandSlot", "Add Hand Slot", "1.4.3")]
public class Plugin : BaseUnityPlugin
{
    static Plugin Instance;
    static GameStat HandSlotNum;
    private static bool AllowInventoryDoubleLine = true;

    private ConfigEntry<int> Config_AddSlotNum;
    private ConfigEntry<bool> Config_ForceAddSlot;
    private ConfigEntry<bool> Config_EnableModifyEncumbrance;
    private ConfigEntry<int> Config_AddEncumbranceNum;

    private ConfigEntry<bool> DoubleLine_EnableLocation;
    private ConfigEntry<bool> DoubleLine_EnableBase;
    private ConfigEntry<bool> DoubleLine_EnableHand;
    private ConfigEntry<bool> DoubleLine_EnableInventory;
    private ConfigEntry<bool> DoubleLine_EnableEquipment;

    private ConfigEntry<bool> SlotScale_EnableLocation;
    private ConfigEntry<bool> SlotScale_EnableBase;
    private ConfigEntry<bool> SlotScale_EnableHand;
    private ConfigEntry<bool> SlotScale_EnableBlueprint;
    private ConfigEntry<bool> SlotScale_EnableInventory;
    private ConfigEntry<bool> SlotScale_EnableEquipment;

    private ConfigEntry<bool> StatScale_EnableBar;

    private ConfigEntry<bool> Special_EnableInventoryDynamicDoubleLine;
    private ConfigEntry<bool> Special_EnableStatusBarElongate;

    private void Awake()
    {
        if (AccessTools.TypeByName("ModLoader.ModPack") != null)
            if (IsDisable("AddHandSlot"))
                return;

        Instance = this;

        var harmony = Harmony.CreateAndPatchAll(typeof(Plugin));
        Logger.LogInfo("Plugin [Add Hand Slot] is loaded!");

        Config_AddSlotNum = Config.Bind("Config", "AddSlotNum", 6,
            new ConfigDescription("增加手牌槽位的数量", new AcceptableValueRange<int>(-6, 10000)));
        Config_ForceAddSlot = Config.Bind("Config", "ForceAddSlot", false, "强制增加手牌槽位（谨慎使用，生效后自动关闭）");
        Config_EnableModifyEncumbrance = Config.Bind("Config", "EnableModifyEncumbrance", false, "是否修改负重上限");
        Config_AddEncumbranceNum = Config.Bind("Config", "EncumbranceNum", 6000,
            new ConfigDescription("增加负重上限的值", new AcceptableValueRange<int>(-4000, 1000000)));

        DoubleLine_EnableLocation =
            Config.Bind("DoubleLine", "EnableLocation", false, "双行环境槽位（测试功能，谨慎使用，开启后将强制修改环境槽位的尺寸）");
        DoubleLine_EnableBase = Config.Bind("DoubleLine", "EnableBase", false, "双行基础槽位（测试功能，谨慎使用，开启后将强制修改基础槽位的尺寸）");
        DoubleLine_EnableHand = Config.Bind("DoubleLine", "EnableHand", false, "双行手牌槽位（测试功能，谨慎使用，开启后将强制修改手牌槽位的尺寸）");
        DoubleLine_EnableInventory =
            Config.Bind("DoubleLine", "EnableInventory", false, "双行容器槽位（测试功能，谨慎使用，开启后将强制修改容器槽位的尺寸）");
        DoubleLine_EnableEquipment =
            Config.Bind("DoubleLine", "EnableEquipment", false, "双行装备槽位（测试功能，谨慎使用，开启后将强制修改装备槽位的尺寸）");

        SlotScale_EnableLocation = Config.Bind("SlotScale", "EnableLocation", false, "修改环境槽位的尺寸");
        SlotScale_EnableBase = Config.Bind("SlotScale", "EnableBase", false, "修改基础槽位的尺寸");
        SlotScale_EnableHand = Config.Bind("SlotScale", "EnableHand", false, "修改手牌槽位的尺寸");
        SlotScale_EnableBlueprint = Config.Bind("SlotScale", "EnableBlueprint", false, "修改蓝图槽位的尺寸");
        SlotScale_EnableInventory = Config.Bind("SlotScale", "EnableInventory", false, "修改容器槽位的尺寸");
        SlotScale_EnableEquipment = Config.Bind("SlotScale", "EnableEquipment", false, "修改装备槽位的尺寸");

        StatScale_EnableBar = Config.Bind("StatScale", "EnableBar", false, "修改状态栏的尺寸");

        Special_EnableInventoryDynamicDoubleLine = Config.Bind("Special", "EnableInventoryDynamicDoubleLine", false,
            "容器动态双行槽位（需同时启用双行容器槽位生效，启用后仅当容器槽位数量大于8时才会按双行显示）");
        Special_EnableStatusBarElongate =
            Config.Bind("Special", "EnableStatusBarElongate", false, "状态条延长（仅当启用修改状态栏的尺寸时生效）");

        if (!DoubleLine_EnableHand.Value && !DoubleLine_EnableInventory.Value && !DoubleLine_EnableBase.Value &&
            !DoubleLine_EnableLocation.Value)
            harmony.Unpatch(AccessTools.Method(typeof(DynamicViewLayoutGroup), "GetElementPosition"),
                typeof(Plugin).GetMethod("DynamicViewLayoutGroup_GetElementPosition_Postfix"));
    }

    private static bool IsDisable(string mod_name)
    {
        return !ModLoader.ModLoader.ModPacks.TryGetValue(mod_name, out var pack) || pack == null ||
               !pack.EnableEntry.Value;
    }

    [HarmonyPostfix, HarmonyPatch(typeof(GameLoad), "LoadMainGameData")]
    public static void GameLoad_LoadMainGameData_Postfix()
    {
        CharacterPerk AddHandSlot = UniqueIDScriptable.GetFromID<CharacterPerk>("97dbfde7a82f4bc69421363adffeb0b5");
        if (AddHandSlot)
            AddHandSlot.StartingStatModifiers[0].ValueModifier = new Vector2(Instance.Config_AddSlotNum.Value,
                Instance.Config_AddSlotNum.Value);

        HandSlotNum = UniqueIDScriptable.GetFromID<GameStat>("3ed9754d13824a918badb45308baff0c");

        if (Instance.Config_EnableModifyEncumbrance.Value) ModifyEncumbrance();
    }

    static void ModifyEncumbrance()
    {
        GameStat Encumbrance = UniqueIDScriptable.GetFromID<GameStat>("21574a6120f4d3c4b913c69987e2ff06");
        if (Encumbrance == null || Encumbrance.Statuses?.Length != 4) return;

        int num = (int)Encumbrance.MinMaxValue.y + Instance.Config_AddEncumbranceNum.Value;
        Encumbrance.MinMaxValue.y = num;
        Encumbrance.VisibleValueRange.y = num;
        Encumbrance.Statuses[0].ValueRange = new Vector2Int((int)(num * 0.5) + 1, (int)(num * 0.75));
        Encumbrance.Statuses[1].ValueRange = new Vector2Int((int)(num * 0.75) + 1, (int)(num * 0.875));
        Encumbrance.Statuses[2].ValueRange = new Vector2Int((int)(num * 0.875) + 1, num - 1);
        Encumbrance.Statuses[3].ValueRange = new Vector2Int(num, num);
    }

    [HarmonyPrefix, HarmonyPatch(typeof(GraphicsManager), "Init")]
    public static void GraphicsManager_Init_Prefix(GraphicsManager __instance)
    {
        // 手牌
        if (Instance.DoubleLine_EnableHand.Value)
        {
            Transform ItemsViewport = __instance.ItemSlotsLine.transform.parent;

            Transform HandGroup = ItemsViewport.Find("HandGroup");
            HandGroup.localScale = new Vector3(0.5f, 0.5f, 1f);

            Transform HandGroupLogic = ItemsViewport.Find("HandGroupLogic");
            HandGroupLogic.localScale = new Vector3(0.5f, 0.5f, 1f);
        }
        else if (Instance.SlotScale_EnableHand.Value)
        {
            Transform ItemsViewport = __instance.ItemSlotsLine.transform.parent;

            Transform HandGroup = ItemsViewport.Find("HandGroup");
            HandGroup.localScale = new Vector3(0.7f, 0.7f, 1f);

            Transform HandGroupLogic = ItemsViewport.Find("HandGroupLogic");
            HandGroupLogic.localScale = new Vector3(0.7f, 0.7f, 1f);
        }

        // 环境
        if (Instance.DoubleLine_EnableLocation.Value)
        {
            Transform LocationsViewport = __instance.LocationSlotsLine.transform.parent;
            LocationsViewport.localScale = new Vector3(0.5f, 0.5f, 1f);
            LocationsViewport.GetComponent<RectTransform>().sizeDelta = new Vector2(1310f, 360f);
        }
        else if (Instance.SlotScale_EnableLocation.Value)
        {
            Transform LocationsViewport = __instance.LocationSlotsLine.transform.parent;
            LocationsViewport.localScale = new Vector3(0.7f, 0.7f, 1f);
            LocationsViewport.GetComponent<RectTransform>().sizeDelta = new Vector2(550f, 0f);
        }

        // 基础
        if (Instance.DoubleLine_EnableBase.Value)
        {
            Transform BaseViewport = __instance.BaseSlotsLine.transform.parent;
            BaseViewport.localScale = new Vector3(0.5f, 0.5f, 1f);
            BaseViewport.GetComponent<RectTransform>().sizeDelta = new Vector2(1160f, 360f);
        }
        else if (Instance.SlotScale_EnableBase.Value)
        {
            Transform BaseViewport = __instance.BaseSlotsLine.transform.parent;
            BaseViewport.localScale = new Vector3(0.7f, 0.7f, 1f);
            BaseViewport.GetComponent<RectTransform>().sizeDelta = new Vector2(380f, 0f);
        }

        // 蓝图
        if (Instance.SlotScale_EnableBlueprint.Value)
        {
            Transform BlueprintsViewport = __instance.BlueprintSlotsLine.transform.parent;
            BlueprintsViewport.localScale = new Vector3(0.7f, 0.7f, 1f);
            BlueprintsViewport.GetComponent<RectTransform>().sizeDelta = new Vector2(620f, 15f);
        }

        // 容器
        if (Instance.DoubleLine_EnableInventory.Value)
        {
            Transform InventoryViewport = __instance.InventoryInspectionPopup.transform.Find(
                "ShadowAndPopupWithTitle/Content/InspectionGroup/InventoryGroup/Inventory/InventoryViewport");
            InventoryViewport.localScale = new Vector3(0.5f, 0.5f, 1f);
            InventoryViewport.GetComponent<RectTransform>().sizeDelta = new Vector2(1175f, 360f);
        }
        else if (Instance.SlotScale_EnableInventory.Value)
        {
            Transform InventoryViewport = __instance.InventoryInspectionPopup.transform.Find(
                "ShadowAndPopupWithTitle/Content/InspectionGroup/InventoryGroup/Inventory/InventoryViewport");
            InventoryViewport.localScale = new Vector3(0.7f, 0.7f, 1f);
            InventoryViewport.GetComponent<RectTransform>().sizeDelta = new Vector2(500f, 0f);
        }

        // 装备
        if (Instance.DoubleLine_EnableEquipment.Value)
        {
            var CharacterSlotsViewport = __instance.CharacterWindow.EquipmentSlotsLine.transform.parent;
            CharacterSlotsViewport.localScale = new Vector3(0.5f, 0.5f, 1f);
            CharacterSlotsViewport.GetComponent<RectTransform>().sizeDelta = new Vector2(1400f, 400f);
        }
        else if (Instance.SlotScale_EnableEquipment.Value)
        {
            var CharacterSlotsViewport = __instance.CharacterWindow.EquipmentSlotsLine.transform.parent;
            CharacterSlotsViewport.localScale = new Vector3(0.7f, 0.7f, 1f);
            CharacterSlotsViewport.GetComponent<RectTransform>().sizeDelta = new Vector2(600f, -28.1044f);
        }

        // if (true)
        // {
        //     var blueprintsViewport = __instance.BlueprintSlotsLine.transform.parent;
        //     blueprintsViewport.localScale = new Vector3(0.5f, 0.5f, 1f);
        //     blueprintsViewport.GetComponent<RectTransform>().sizeDelta = new Vector2(1460f, 15f);
        // }

        // 拖拽
        Transform DraggingParent = GameManager.Instance.DraggingPlane.Find("DraggingParent");
        if (Instance.DoubleLine_EnableHand.Value || Instance.DoubleLine_EnableInventory.Value ||
            Instance.DoubleLine_EnableBase.Value || Instance.DoubleLine_EnableLocation.Value)
        {
            DraggingParent.localScale = new Vector3(0.5f, 0.5f, 1f);
        }
        else if (Instance.SlotScale_EnableHand.Value || Instance.SlotScale_EnableLocation.Value ||
                 Instance.SlotScale_EnableBase.Value || Instance.SlotScale_EnableBlueprint.Value
                 || Instance.SlotScale_EnableInventory.Value || Instance.SlotScale_EnableEquipment.Value)
        {
            DraggingParent.localScale = new Vector3(0.7f, 0.7f, 1f);
        }

        if (Instance.StatScale_EnableBar.Value)
        {
            var AllStatusesCanvas = GraphicsManager.Instance.ImportantStatusGraphicsParent.parent;
            AllStatusesCanvas.localScale = new Vector3(0.7f, 0.7f, 1f);
            var rect = AllStatusesCanvas.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(417f, rect.sizeDelta.y);
        }
    }

    [HarmonyPostfix, HarmonyPatch(typeof(GraphicsManager), "Init")]
    public static void GraphicsManager_Init_Postfix(GraphicsManager __instance)
    {
        if (GameLoad.Instance.Games.Count < GameLoad.Instance.CurrentGameDataIndex ||
            GameLoad.Instance.CurrentGameDataIndex < 0) return;

        GameSaveData CurrentGameData = GameLoad.Instance.Games[GameLoad.Instance.CurrentGameDataIndex].MainData;

        if (!CurrentGameData.StatsDict.TryGetValue("3ed9754d13824a918badb45308baff0c",
                out StatSaveData HandSlotNum)) return;

        int CurrentHandSlotNum = (int)HandSlotNum.BaseValue;
        CardLine ItemSlotsLine = __instance.ItemSlotsLine;
        if (CurrentHandSlotNum > 6)
            for (int i = ItemSlotsLine.Count; i < CurrentHandSlotNum; i++)
                ItemSlotsLine.AddSlot(ItemSlotsLine.Count);

        ItemSlotsLine.RefreshSlotIndices(0);
    }

    [HarmonyPrefix, HarmonyPatch(typeof(GameManager), "Awake")]
    public static void GameManager_Awake_Prefix()
    {
        CharacterPerk AddHandSlot = UniqueIDScriptable.GetFromID<CharacterPerk>("97dbfde7a82f4bc69421363adffeb0b5");
        if (AddHandSlot)
            AddHandSlot.StartingStatModifiers[0].ValueModifier = new Vector2(Instance.Config_AddSlotNum.Value,
                Instance.Config_AddSlotNum.Value);
    }

    [HarmonyPostfix, HarmonyPatch(typeof(GameManager), "Awake")]
    public static void GameManager_Awake_Postfix(GameManager __instance)
    {
        if (Instance.Config_ForceAddSlot.Value)
        {
            __instance.StatsDict[HandSlotNum].CurrentBaseValue = Instance.Config_AddSlotNum.Value + 6;
            Instance.Config_ForceAddSlot.Value = false;
        }

        ModifyHandSlotNum((int)__instance.StatsDict[HandSlotNum].SimpleCurrentValue);
    }

    [HarmonyPostfix, HarmonyPatch(typeof(GameManager), "ChangeStatValue")]
    public static IEnumerator GameManager_ChangeStatValue_Postfix(IEnumerator result, InGameStat _Stat)
    {
        yield return result;

        if (_Stat != null && _Stat.StatModel != null && _Stat.StatModel == HandSlotNum)
        {
            ModifyHandSlotNum((int)_Stat.SimpleCurrentValue);
        }
    }

    static void ModifyHandSlotNum(int currentHandSlotNum)
    {
        if (currentHandSlotNum < 0) currentHandSlotNum = 0;

        CardLine ItemSlotsLine = GraphicsManager.Instance.ItemSlotsLine;
        if (currentHandSlotNum > ItemSlotsLine.Count)
            for (int i = ItemSlotsLine.Count; i < currentHandSlotNum; i++)
                ItemSlotsLine.AddSlot(ItemSlotsLine.Count);
        else
            for (int i = ItemSlotsLine.Count; i > currentHandSlotNum; i--)
            {
                DynamicLayoutSlot slot = ItemSlotsLine.Slots[ItemSlotsLine.Count - 1];
                for (int j = slot.CardPileCount(); j > 0; j--)
                    try
                    {
                        slot.AssignedCard.SwapCard();
                    }
                    catch
                    {
                    }

                ItemSlotsLine.RemoveSlot(ItemSlotsLine.Count - 1);
            }

        if (currentHandSlotNum == 0) GraphicsManager.Instance.MinItemSlots = 0;
        GraphicsManager.Instance.MaxItemSlots = currentHandSlotNum;
    }

    [HarmonyPostfix, HarmonyPatch(typeof(GraphicsManager), "UpdateSlotsVisibility")]
    public static void GraphicsManager_UpdateSlotsVisibility_Postfix()
    {
        if (GameManager.Instance.StatsDict.ContainsKey(HandSlotNum) &&
            GameManager.Instance.StatsDict[HandSlotNum].SimpleCurrentValue == 0)
            ModifyHandSlotNum(0);
    }

    [HarmonyPostfix, HarmonyPatch(typeof(DynamicViewLayoutGroup), "GetElementPosition")]
    public static void DynamicViewLayoutGroup_GetElementPosition_Postfix(DynamicViewLayoutGroup __instance,
        int _Index, ref Vector3 __result)
    {
        Transform view;

        var pos = 1;
        Vector2 v1;
        Vector2 v2;

        if (__instance == GraphicsManager.Instance.ItemSlotsLine && Instance.DoubleLine_EnableHand.Value)
        {
            view = GraphicsManager.Instance.ItemsParent;
            pos = 2;
            v1 = new Vector2(0, -325);
            v2 = new Vector2(55, 162.5f);
        }
        else if (__instance == GraphicsManager.Instance.BaseSlotsLine && Instance.DoubleLine_EnableBase.Value)
        {
            view = GraphicsManager.Instance.BaseParent;
            pos = 2;
            v1 = new Vector2(0, -325);
            v2 = new Vector2(50, 360);
        }
        else if (__instance == GraphicsManager.Instance.LocationSlotsLine &&
                 Instance.DoubleLine_EnableLocation.Value)
        {
            view = GraphicsManager.Instance.LocationParent;
            v1 = new Vector2(0, -325);
            v2 = new Vector2(0, 180);
        }
        else if (__instance == GraphicsManager.Instance.InventoryInspectionPopup.InventorySlotsLine &&
                 Instance.DoubleLine_EnableInventory.Value && AllowInventoryDoubleLine)
        {
            view = GraphicsManager.Instance.InventoryInspectionPopup.InventorySlotsLine.RectTr;
            v1 = new Vector2(0, -325);
            v2 = new Vector2(0, 170);
        }
        else if (__instance == GraphicsManager.Instance.CharacterWindow.EquipmentSlotsLine &&
                 Instance.DoubleLine_EnableEquipment.Value)
        {
            view = GraphicsManager.Instance.CharacterWindow.EquipmentsParent;
            v1 = new Vector2(0, -325);
            v2 = new Vector2(0, 170);
        }
        // else if (__instance == GraphicsManager.Instance.BlueprintSlotsLine)
        // {
        //     view = GraphicsManager.Instance.BlueprintParent;
        //     v1 = new Vector2(0, -325);
        //     v2 = new Vector2(0, 170);
        // }
        else return;

        if (view == null || __instance.ExtraSpaces is not { Count: > 0 } spaces) return;

        var count = __instance.AllElements.Count - __instance.InactiveElements;

        if (count % 2 == 1)
        {
            if (__instance == GraphicsManager.Instance.ItemSlotsLine &&
                Instance.DoubleLine_EnableHand.Value) v2.x = 0;
            else count++;
        }

        __instance.Size = __instance.Spacing * 0.5f * count;
        if (__instance.ExtraSpaces != null)
        {
            foreach (var dynamicViewExtraSpace in __instance.ExtraSpaces)
                __instance.Size += dynamicViewExtraSpace.Space;
        }

        if (__instance.AddedSize && __instance.AddedSize.gameObject.activeSelf)
        {
            if (__instance.LayoutOrientation == RectTransform.Axis.Horizontal)
                __instance.Size += __instance.AddedSize.rect.width;
            else __instance.Size += __instance.AddedSize.rect.height;
        }

        if (__instance.LayoutOrientation == RectTransform.Axis.Horizontal)
            __instance.Size += __instance.Padding.left + __instance.Padding.right;
        else __instance.Size += __instance.Padding.top + __instance.Padding.right;

        __instance.Size = Mathf.Max(__instance.Size, __instance.MinSize);

        view.GetComponent<RectTransform>().sizeDelta = new Vector2(__instance.Size, 0f);

        var num = 0f;
        foreach (var space in spaces)
            if (space.AtIndex <= _Index)
                num += space.Space;

        __result = __instance.LayoutOriginPos / pos +
                   __instance.LayoutDirection * (__instance.Spacing * (_Index / 2) + num) + v1 * (_Index % 2) + v2;
    }

    [HarmonyPostfix, HarmonyPatch(typeof(InspectionPopup), "Setup", typeof(InGameCardBase))]
    public static void InspectionPopup_Setup_Postfix(InspectionPopup __instance)
    {
        if (__instance != GraphicsManager.Instance.InventoryInspectionPopup) return;

        var viewport = __instance.transform.Find(
            "ShadowAndPopupWithTitle/Content/InspectionGroup/InventoryGroup/Inventory/InventoryViewport");
        var line = __instance.InventorySlotsLine;

        if (!Instance.Special_EnableInventoryDynamicDoubleLine.Value)
        {
            AllowInventoryDoubleLine = false;
            viewport.localScale = new Vector3(1f, 1f, 1f);
            viewport.GetComponent<RectTransform>().sizeDelta = new Vector2(-10f, -30f);
            return;
        }

        if (!Instance.DoubleLine_EnableInventory.Value ||
            !Instance.Special_EnableInventoryDynamicDoubleLine.Value)
        {
            AllowInventoryDoubleLine = true;
            viewport.localScale = new Vector3(0.5f, 0.5f, 1f);
            viewport.GetComponent<RectTransform>().sizeDelta = new Vector2(1175f, 360f);
            line.RecalculateSize = true;
            return;
        }

        var active = line.Count - line.InactiveElements;

        if (active > 8)
        {
            AllowInventoryDoubleLine = true;
            viewport.localScale = new Vector3(0.5f, 0.5f, 1f);
            viewport.GetComponent<RectTransform>().sizeDelta = new Vector2(1175f, 360f);
        }
        else
        {
            AllowInventoryDoubleLine = false;
            viewport.localScale = new Vector3(0.7f, 0.7f, 1f);
            viewport.GetComponent<RectTransform>().sizeDelta = new Vector2(500f, 0f);
        }

        line.RecalculateSize = true;
    }

    [HarmonyPostfix, HarmonyPatch(typeof(StatStatusGraphics), "Awake")]
    public static void StatStatusGraphics_Awake_Postfix(StatStatusGraphics __instance)
    {
        if (!Instance.Special_EnableStatusBarElongate.Value) return;

        var rt = (RectTransform)__instance.transform;
        var v = rt.sizeDelta;
        rt.sizeDelta = new Vector2(400, v.y);
    }
}