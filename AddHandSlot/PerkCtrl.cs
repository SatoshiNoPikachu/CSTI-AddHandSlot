using UnityEngine;

namespace AddHandSlot;

public class PerkCtrl
{
    public static void ModifyAddHandSlotPerkNum()
    {
        var config = ConfigManager.Get<int>("Config", "AddHandSlotNum");
        if (config is null) return;

        var ctrl = new PerkCtrl("AddHandSlot:Pk_AddHandSlot");

        ctrl.Perk?.StartingStatModifiers[0].ValueModifier = new Vector2(config.Value, config.Value);
    }

    public static void ModifyAddEncumbranceLimitNum()
    {
        var config = ConfigManager.Get<int>("Config", "AddEncumbranceNum");
        if (config is null) return;

        var ctrl = new PerkCtrl("AddHandSlot:Pk_AddEncumbranceLimit");

        ctrl.Perk?.StartingStatModifiers[0].ValueModifier = new Vector2(config.Value, config.Value);
    }

    public CharacterPerk Perk { get; private set; }

    public PerkCtrl(string uid)
    {
        Perk = UniqueIDScriptable.GetFromID<CharacterPerk>(uid);
    }
}