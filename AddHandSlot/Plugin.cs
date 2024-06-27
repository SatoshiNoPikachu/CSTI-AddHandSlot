using AddHandSlot.Line;
using AddHandSlot.Stat;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace AddHandSlot;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class Plugin : BaseUnityPlugin
{
    public const string PluginGuid = "Pikachu.AddHandSlot";
    public const string PluginName = "Add Hand Slot";
    public const string PluginVersion = "2.1.0";

    // internal static Plugin Instance;
    // public static string PluginPath => Path.GetDirectoryName(Instance.Info.Location);
    internal static ManualLogSource Log;
    private static readonly Harmony Harmony = new(PluginGuid);

    private void Awake()
    {
        if (AccessTools.TypeByName("ModLoader.ModPack") != null)
            if (IsDisable("AddHandSlot"))
                return;

        ConfigManager.Config = Config;
        InitConfig();

        Harmony.PatchAll();

        // Instance = this;
        Log = Logger;
        Log.LogInfo($"Plugin {PluginName} is loaded!");
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

        Config.Bind("DoubleLine", "EnableLocation", true, "双行环境槽位");
        Config.Bind("DoubleLine", "EnableBase", true, "双行基础槽位");
        Config.Bind("DoubleLine", "EnableHand", false, "双行手牌槽位");
        Config.Bind("DoubleLine", "EnableBlueprint", true, "双行蓝图槽位");
        Config.Bind("DoubleLine", "EnableInventory", true, "双行容器槽位");
        Config.Bind("DoubleLine", "EnableEquipment", true, "双行装备槽位");

        Config.Bind("SlotScale", "EnableLocation", true, "修改环境槽位的尺寸");
        Config.Bind("SlotScale", "EnableBase", true, "修改基础槽位的尺寸");
        Config.Bind("SlotScale", "EnableHand", false, "修改手牌槽位的尺寸");
        Config.Bind("SlotScale", "EnableBlueprint", true, "修改蓝图槽位的尺寸");
        Config.Bind("SlotScale", "EnableInventory", true, "修改容器槽位的尺寸");
        Config.Bind("SlotScale", "EnableEquipment", true, "修改装备槽位的尺寸");

        Config.Bind("StatScale", "EnableBar", true, "修改状态栏的尺寸").SettingChanged += (_, _) => StatBarCtrl.UpdateStatus();

        Config.Bind("Special", "EnableInventoryDynamicDoubleLine", true,
            "容器动态双行槽位（仅当启用双行容器槽位生效，启用后仅当容器槽位数量大于8时才会按双行显示）");
        Config.Bind("Special", "EnableStatusBarElongate", true, "状态条延长（仅当启用修改状态栏的尺寸时生效）").SettingChanged +=
            (_, _) => StatBarCtrl.UpdateStatusBar();

        foreach (var (def, entry) in Config)
        {
            if (def.Section is "SlotScale" or "DoubleLine")
                ((ConfigEntry<bool>)entry).SettingChanged += LineCtrl.OnConfigChange;
        }
    }
}