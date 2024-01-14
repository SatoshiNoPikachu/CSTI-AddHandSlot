using UnityEngine;

namespace AddHandSlot;

public class PerkCtrl
{
    public static void ModifyAddHandSlotPerkNum()
    {
        var config = ConfigManager.Get<int>("Config", "AddHandSlotNum");
        if (config is null) return;

        var ctrl = new PerkCtrl("97dbfde7a82f4bc69421363adffeb0b5");
        if (ctrl.Perk is null) return;

        ctrl.Perk.StartingStatModifiers[0].ValueModifier = new Vector2(config.Value, config.Value);
    }

    public CharacterPerk Perk { get; private set; }

    public PerkCtrl(string uid)
    {
        Perk = UniqueIDScriptable.GetFromID<CharacterPerk>(uid);
    }
}