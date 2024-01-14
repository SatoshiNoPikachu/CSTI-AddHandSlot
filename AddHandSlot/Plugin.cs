using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace AddHandSlot;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class Plugin : BaseUnityPlugin
{
    public const string PluginGuid = "Pikachu.AddHandSlot";
    public const string PluginName = "Add Hand Slot";
    public const string PluginVersion = "2.0.3";

    // public static Plugin Instance;

    private void Awake()
    {
        if (AccessTools.TypeByName("ModLoader.ModPack") != null)
            if (IsDisable("AddHandSlot"))
                return;

        // Instance = this;

        ConfigManager.Config = Config;
        InitConfig();

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        Logger.LogInfo("Plugin [Add Hand Slot] is loaded!");
    }

    private static bool IsDisable(string mod_name)
    {
        return !ModLoader.ModLoader.ModPacks.TryGetValue(mod_name, out var pack) || pack == null ||
               !pack.EnableEntry.Value;
    }

    private void InitConfig()
    {
        Config.Bind("Config", "AddHandSlotNum", 6,
                new ConfigDescription("增加手牌槽位的数量", new AcceptableValueRange<int>(-6, 10000))).SettingChanged +=
            (_, _) => PerkCtrl.ModifyAddHandSlotPerkNum();
        Config.Bind("Config", "ForceAddHandSlot", false, "强制增加手牌槽位（谨慎使用，生效后自动关闭）").SettingChanged +=
            (_, _) => LineCtrl.ForceAddHandSlot();
        Config.Bind("Config", "EnableModifyEncumbrance", false, "是否修改负重上限").SettingChanged +=
            (_, _) => StatCtrl.ModifyEncumbranceLimit();
        Config.Bind("Config", "AddEncumbranceNum", 6000,
                new ConfigDescription("增加负重上限的值", new AcceptableValueRange<int>(-4000, 1000000))).SettingChanged +=
            (_, _) => StatCtrl.ModifyEncumbranceLimit();

        Config.Bind("DoubleLine", "EnableLocation", false, "双行环境槽位");
        Config.Bind("DoubleLine", "EnableBase", false, "双行基础槽位");
        Config.Bind("DoubleLine", "EnableHand", false, "双行手牌槽位");
        Config.Bind("DoubleLine", "EnableBlueprint", false, "双行蓝图槽位（体验版）");
        Config.Bind("DoubleLine", "EnableInventory", false, "双行容器槽位");
        Config.Bind("DoubleLine", "EnableEquipment", false, "双行装备槽位");

        Config.Bind("SlotScale", "EnableLocation", false, "修改环境槽位的尺寸");
        Config.Bind("SlotScale", "EnableBase", false, "修改基础槽位的尺寸");
        Config.Bind("SlotScale", "EnableHand", false, "修改手牌槽位的尺寸");
        Config.Bind("SlotScale", "EnableBlueprint", false, "修改蓝图槽位的尺寸");
        Config.Bind("SlotScale", "EnableInventory", false, "修改容器槽位的尺寸");
        Config.Bind("SlotScale", "EnableEquipment", false, "修改装备槽位的尺寸");

        Config.Bind("StatScale", "EnableBar", false, "修改状态栏的尺寸").SettingChanged += (_, _) => StatBarCtrl.UpdateStatus();

        Config.Bind("Special", "EnableInventoryDynamicDoubleLine", false,
            "容器动态双行槽位（仅当启用双行容器槽位生效，启用后仅当容器槽位数量大于8时才会按双行显示）");
        Config.Bind("Special", "EnableStatusBarElongate", false, "状态条延长（仅当启用修改状态栏的尺寸时生效）").SettingChanged +=
            (_, _) => StatBarCtrl.UpdateStatusBar();

        foreach (var kpv in Config)
        {
            if (kpv.Key.Section is "SlotScale" or "DoubleLine")
                ((ConfigEntry<bool>)kpv.Value).SettingChanged += LineCtrl.OnConfigChange;
        }
    }
}