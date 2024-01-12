using System.Collections.Generic;
using UnityEngine;

namespace AddHandSlot;

public static class DragCtrl
{
    private static Transform Trans => GameManager.Instance?.DraggingPlane?.Find("DraggingParent");

    private static readonly Dictionary<LineStatus, ScaleCtrl> ScaleCtrlMap = new()
    {
        [LineStatus.Initial] = new ScaleCtrl(null, 1f, 1f),
        [LineStatus.ScaleDown] = new ScaleCtrl(null, 0.7f, 0.7f),
        [LineStatus.DoubleLine] = new ScaleCtrl(null, 0.5f, 0.5f),
    };

    public static void UpdateStatus()
    {
        if (Trans is null) return;

        var status = LineStatus.Initial;
        foreach (var ctrl in LineCtrl.GetLines())
        {
            if (ctrl.Status == LineStatus.DoubleLine)
            {
                status = LineStatus.DoubleLine;
                break;
            }

            if (ctrl.Status == LineStatus.ScaleDown) status = LineStatus.ScaleDown;
        }

        if (ScaleCtrlMap.TryGetValue(status, out var scale)) scale.ApplyScale(Trans);
    }
}