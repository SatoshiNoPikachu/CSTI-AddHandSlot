using System.Collections.Generic;
using AddHandSlot.Line;
using UnityEngine;

namespace AddHandSlot.Stat;

public class StatCtrl
{
    public const string UidHandSlotNum = "AddHandSlot-HandSlotNum";

    public static void OnStatValueChange(InGameStat stat)
    {
        if (!stat) return;

        var model = stat.StatModel;
        if (!model) return;

        var curValue = (int)stat.CurrentValue(GameManager.Instance.NotInBase);

        if (model.UniqueID == UidHandSlotNum)
        {
            LineCtrl.ModifyHandSlotNum(curValue);
            return;
        }

        if (model.UniqueID != "AddHandSlot-EncumbranceLimitNum") return;

        ModifyEncumbranceLimit(curValue);
    }

    /// <summary>
    /// 修改负重上限
    /// </summary>
    public static void ForceModifyEncumbranceLimit()
    {
        if (GameManager.Instance is null) return;
        if (!ConfigManager.IsEnable("Config", "ForceModifyEncumbrance")) return;

        var config = ConfigManager.Get<int>("Config", "AddEncumbranceNum");
        if (config is null) return;

        var ctrl = new StatCtrl("AddHandSlot-EncumbranceLimitNum");
        if (!ctrl.InGame) return;

        ctrl.InGame.CurrentBaseValue = 4000 + config.Value;
        ModifyEncumbranceLimit((int)ctrl.InGame.CurrentValue(GameManager.Instance.NotInBase));

        ConfigManager.Get<bool>("Config", "ForceModifyEncumbrance").Value = false;
    }

    public static void ModifyEncumbranceLimit()
    {
        var ctrl = new StatCtrl("AddHandSlot-EncumbranceLimitNum");
        if (!ctrl.InGame) return;
        
        ModifyEncumbranceLimit((int)ctrl.InGame.CurrentValue(GameManager.Instance.NotInBase));
    }

    /// <summary>
    /// 修改负重上限
    /// </summary>
    /// <param name="num">上限值</param>
    private static void ModifyEncumbranceLimit(int num)
    {
        var ctrl = new StatCtrl("21574a6120f4d3c4b913c69987e2ff06");
        if (ctrl.Stat?.Statuses?.Length is null or not 4) return;

        var stat = ctrl.Stat;
        var map = ctrl.GetStatusesMap();

        stat.MinMaxValue.y = num;
        stat.VisibleValueRange.y = num;
        stat.Statuses[0].ValueRange = new Vector2Int((int)(num * 0.5) + 1, (int)(num * 0.75));
        stat.Statuses[1].ValueRange = new Vector2Int((int)(num * 0.75) + 1, (int)(num * 0.875));
        stat.Statuses[2].ValueRange = new Vector2Int((int)(num * 0.875) + 1, num - 1);
        stat.Statuses[3].ValueRange = new Vector2Int(num, num);

        foreach (var status in map)
        {
            status.Key.ValueRange = status.Value.ValueRange;
        }

        if (ctrl.InGame is null) return;
        GameManager.Instance.StartCoroutine(GameManager.Instance.UpdateStatStatuses(ctrl.InGame, -1, null));
    }

    public GameStat Stat { get; }

    public InGameStat InGame =>
        GameManager.Instance is not null && Stat is not null &&
        GameManager.Instance.StatsDict.TryGetValue(Stat, out var stat)
            ? stat
            : null;

    public StatCtrl(string uid)
    {
        Stat = UniqueIDScriptable.GetFromID<GameStat>(uid);
    }

    public Dictionary<StatStatus, StatStatus> GetStatusesMap()
    {
        var map = new Dictionary<StatStatus, StatStatus>();
        if (InGame is null) return map;

        foreach (var current in InGame.CurrentStatuses)
        {
            foreach (var status in Stat.Statuses)
            {
                if (!current.IsSameStatus(status)) continue;
                map[current] = status;
                break;
            }
        }

        return map;
    }
}