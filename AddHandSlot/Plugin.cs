using BepInEx;
using HarmonyLib;
using ModCore;
using ModCore.Data;
using ModCore.Services;

namespace AddHandSlot;

[BepInDependency("Pikachu.CSTI.ModCore")]
[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class Plugin : BaseUnityPlugin<Plugin>
{
    private const string PluginGuid = "Pikachu.AddHandSlot";
    public const string PluginName = "AddHandSlot";
    public const string PluginVersion = "3.0.1";

    private static readonly Harmony Harmony = new(PluginGuid);

    protected override void Awake()
    {
        base.Awake();

        Harmony.PatchAll();

        ConfigManager.Bind();

        LocalizationService.RegisterPath(PluginPath, "AddHandSlot_");

        PreloadData();

        Log.LogMessage($"Plugin {PluginName} is loaded!");
    }

    private static void PreloadData()
    {
        Loader.PreloadData<GameStat>("AddHandSlot_HandSlotNum", ResourcesJson.HandSlotNum);
        Loader.PreloadData<GameStat>("AddHandSlot_EncumbranceLimitNum", ResourcesJson.EncumbranceLimitNum);

        Loader.PreloadData<CharacterPerk>("AddHandSlot_Pk_AddHandSlot", ResourcesJson.Pk_AddHandSlot);
        Loader.PreloadData<CharacterPerk>("AddHandSlot_Pk_AddEncumbranceLimit", ResourcesJson.Pk_AddEncumbranceLimit);

        Loader.LoadCompleteEvent += PerkCtrl.ModifyAddHandSlotPerkNum;
        Loader.LoadCompleteEvent += PerkCtrl.ModifyAddEncumbranceLimitNum;
    }
}