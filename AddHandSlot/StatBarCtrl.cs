using System.Collections.Generic;
using UnityEngine;

namespace AddHandSlot;

public static class StatBarCtrl
{
    public static RectTransform Trans =>
        GraphicsManager.Instance?.ImportantStatusGraphicsParent?.parent as RectTransform;

    public static IEnumerable<RectTransform> ChildTrans => GraphicsManager.Instance is null
        ? null
        : new[]
        {
            GraphicsManager.Instance.ImportantStatusGraphicsParent, GraphicsManager.Instance.StatusGraphicsParent
        };

    public static readonly Dictionary<bool, ScaleCtrl> PanelScaleMap = new();

    public static readonly Dictionary<bool, ScaleCtrl> BarScaleMap = new();

    public static readonly Dictionary<bool, PosCtrl> BarPosMap = new();

    public static void Init()
    {
        PanelScaleMap.Clear();
        BarScaleMap.Clear();
        BarPosMap.Clear();

        var trans = Trans;
        if (trans is null) return;

        PanelScaleMap[true] = new ScaleCtrl(trans, 0.7f, 0.7f, 417f, null);
        PanelScaleMap[false] = new ScaleCtrl(trans);
    }

    public static void UpdateStatus()
    {
        if (Trans is null) return;
        
        if (!PanelScaleMap.TryGetValue(ConfigManager.IsEnable("StatScale", "EnableBar"), out var ctrl)) return;
        UpdateStatusBar();
        ctrl.Apply(Trans);

        foreach (var trans in ChildTrans)
        {
            new ScaleCtrl(ctrl.W, null, trans).Apply(trans);
        }
    }

    public static void UpdateStatusBar()
    {
        if (GraphicsManager.Instance?.CurrentStatusGraphics is null) return;

        foreach (var bar in GraphicsManager.Instance.CurrentStatusGraphics)
        {
            UpdateStatusBar(bar);
        }
    }

    public static void UpdateStatusBar(StatStatusGraphics bar)
    {
        var enable = ConfigManager.IsEnable("StatScale", "EnableBar") &&
                     ConfigManager.IsEnable("Special", "EnableStatusBarElongate");

        if (BarScaleMap.TryGetValue(enable, out var scaleCtrl))
            scaleCtrl.ApplySize((RectTransform)bar.transform);
        else return;

        if (bar.PinIcon is null) return;
        if (BarPosMap.TryGetValue(enable, out var posCtrl)) posCtrl.Apply(bar.PinIcon.transform);
    }

    public static void AddBar(StatStatusGraphics bar)
    {
        if (bar?.PinIcon is null) return;

        if (BarScaleMap.Count == 0)
        {
            var trans = (RectTransform)bar.transform;
            BarScaleMap[true] = new ScaleCtrl(400f, null, trans);
            BarScaleMap[false] = new ScaleCtrl(null, null, trans);
        }

        if (BarPosMap.Count == 0)
        {
            var trans = bar.PinIcon.transform;
            BarPosMap[true] = new PosCtrl(trans, 174f);
            BarPosMap[false] = new PosCtrl(trans);
        }

        UpdateStatusBar(bar);
    }
}