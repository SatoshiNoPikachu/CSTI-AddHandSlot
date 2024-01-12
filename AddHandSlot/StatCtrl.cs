using UnityEngine;

namespace AddHandSlot;

public class StatCtrl
{
    public const string UidHandSlotNum = "3ed9754d13824a918badb45308baff0c";

    /// <summary>
    /// 修改负重上限
    /// </summary>
    public static void ModifyEncumbranceLimit()
    {
        if (!ConfigManager.IsEnable("Config", "EnableModifyEncumbrance"))
        {
            ModifyEncumbranceLimit(4000);
            return;
        }

        var config = ConfigManager.Get<int>("Config", "AddEncumbranceNum");
        if (config is null) return;

        ModifyEncumbranceLimit(config.Value + 4000);
    }

    /// <summary>
    /// 修改负重上限
    /// </summary>
    /// <param name="num">上限值</param>
    private static void ModifyEncumbranceLimit(int num)
    {
        var ctrl = new StatCtrl("21574a6120f4d3c4b913c69987e2ff06");
        if (ctrl.Stat?.Statuses is null || ctrl.Stat.Statuses.Length != 4) return;

        var stat = ctrl.Stat;
        stat.MinMaxValue.y = num;
        stat.VisibleValueRange.y = num;
        stat.Statuses[0].ValueRange = new Vector2Int((int)(num * 0.5) + 1, (int)(num * 0.75));
        stat.Statuses[1].ValueRange = new Vector2Int((int)(num * 0.75) + 1, (int)(num * 0.875));
        stat.Statuses[2].ValueRange = new Vector2Int((int)(num * 0.875) + 1, num - 1);
        stat.Statuses[3].ValueRange = new Vector2Int(num, num);
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
}