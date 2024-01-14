using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using JetBrains.Annotations;
using UnityEngine;

namespace AddHandSlot;

/// <summary>
/// CardLine控制器
/// </summary>
public class LineCtrl
{
    public static void ModifyHandSlotNum()
    {
        var statCtrl = new StatCtrl(StatCtrl.UidHandSlotNum);
        if (statCtrl.InGame is null) return;

        ModifyHandSlotNum((int)statCtrl.InGame.SimpleCurrentValue);
    }

    public static void ModifyHandSlotNum(int num)
    {
        if (GraphicsManager.Instance is null) return;
        GetLine(LineType.Hand)?.SetSlotNum(num);
    }

    public static void ForceAddHandSlot()
    {
        if (GameManager.Instance is null) return;
        if (!ConfigManager.IsEnable("Config", "ForceAddHandSlot")) return;

        var config = ConfigManager.Get<int>("Config", "AddHandSlotNum");
        if (config is null) return;

        var ctrl = new StatCtrl(StatCtrl.UidHandSlotNum);
        if (ctrl.InGame is not null) ctrl.InGame.CurrentBaseValue = 6 + config.Value;

        ModifyHandSlotNum(6 + config.Value);

        ConfigManager.Get<bool>("Config", "ForceAddHandSlot").Value = false;
    }

    public static void OnConfigChange(object sender, EventArgs args)
    {
        if (GraphicsManager.Instance is null) return;
        if (sender is not ConfigEntry<bool> config) return;
        if (!Enum.TryParse<LineType>(config.Definition.Key.Substring(6), out var type)) return;

        var line = GetLine(type);
        if (line is null) return;

        if (line.IsAllowDoubleLine()) line.Status = LineStatus.DoubleLine;
        else if (line.IsAllowScaleDown()) line.Status = LineStatus.ScaleDown;
        else line.Status = LineStatus.Initial;
    }

    private delegate RectTransform[] ResolveTrans(CardLine line);

    private delegate Dictionary<LineStatus, ScaleCtrl> ResolveScale(RectTransform trans);

    private static readonly Dictionary<LineType, LineCtrl> Lines = new();

    private static readonly Dictionary<LineType, (ResolveTrans trans, ResolveScale scale)> LineDelegates = new();

    private static readonly Dictionary<LineType, DoubleLineArg> DoubleArgs = new();

    static LineCtrl()
    {
        // 手牌
        LineDelegates[LineType.Hand] = (line => new[]
        {
            (RectTransform)line.transform.parent
        }, trans => new Dictionary<LineStatus, ScaleCtrl>
        {
            [LineStatus.Initial] = new(trans),
            [LineStatus.ScaleDown] = new(trans, 0.7f, 0.7f, 380f, 148f),
            [LineStatus.DoubleLine] = new(trans, 0.5f, 0.5f, 1157f, 358f),
        });

        // 环境
        LineDelegates[LineType.Location] = (line => new[]
        {
            (RectTransform)line.transform.parent
        }, trans => new Dictionary<LineStatus, ScaleCtrl>
        {
            [LineStatus.Initial] = new(trans),
            [LineStatus.ScaleDown] = new(trans, 0.7f, 0.7f, 550f, null),
            [LineStatus.DoubleLine] = new(trans, 0.5f, 0.5f, 1310f, 360f),
        });

        // 基础
        LineDelegates[LineType.Base] = (line => new[]
        {
            (RectTransform)line.transform.parent
        }, trans => new Dictionary<LineStatus, ScaleCtrl>
        {
            [LineStatus.Initial] = new(trans),
            [LineStatus.ScaleDown] = new(trans, 0.7f, 0.7f, 380f, null),
            [LineStatus.DoubleLine] = new(trans, 0.5f, 0.5f, 1160f, 360f),
        });

        // 蓝图
        LineDelegates[LineType.Blueprint] = (line => new[]
        {
            (RectTransform)line.transform.parent
        }, trans => new Dictionary<LineStatus, ScaleCtrl>
        {
            [LineStatus.Initial] = new(trans),
            [LineStatus.ScaleDown] = new(trans, 0.7f, 0.7f, 620f, null),
            [LineStatus.DoubleLine] = new(trans, 0.5f, 0.5f, 1460f, 360f),
        });

        // 容器
        LineDelegates[LineType.Inventory] = (line => new[]
        {
            (RectTransform)line.transform.parent
        }, trans => new Dictionary<LineStatus, ScaleCtrl>
        {
            [LineStatus.Initial] = new(trans),
            [LineStatus.ScaleDown] = new(trans, 0.7f, 0.7f, 500f, 0f),
            [LineStatus.DoubleLine] = new(trans, 0.5f, 0.5f, 1175f, 360f),
        });

        // 装备
        LineDelegates[LineType.Equipment] = (line => new[]
        {
            (RectTransform)line.transform.parent
        }, trans => new Dictionary<LineStatus, ScaleCtrl>
        {
            [LineStatus.Initial] = new(trans),
            [LineStatus.ScaleDown] = new(trans, 0.7f, 0.7f, 600f, null),
            [LineStatus.DoubleLine] = new(trans, 0.5f, 0.5f, 1400f, 400f),
        });

        DoubleArgs[LineType.Base] = new DoubleLineArg(2, new Vector2(0f, -325f), new Vector2(50f, 360f));

        DoubleArgs[LineType.Blueprint] = new DoubleLineArg(1, new Vector2(0f, -325f), new Vector2(0f, 170f));

        DoubleArgs[LineType.Equipment] = new DoubleLineArg(1, new Vector2(0f, -325f), new Vector2(0f, 170f));

        DoubleArgs[LineType.Hand] = new DoubleLineArg(2, new Vector2(0f, -325f), new Vector2(55f, 162.5f));

        DoubleArgs[LineType.Inventory] = new DoubleLineArg(1, new Vector2(0f, -325f), new Vector2(0f, 170f));

        DoubleArgs[LineType.Location] = new DoubleLineArg(1, new Vector2(0f, -325f), new Vector2(0f, 180f));
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init()
    {
        Lines.Clear();

        var graphics = GraphicsManager.Instance;

        Lines[LineType.Hand] = new LineCtrl(LineType.Hand, graphics.ItemSlotsLine);
        Lines[LineType.Location] = new LineCtrl(LineType.Location, graphics.LocationSlotsLine);
        Lines[LineType.Base] = new LineCtrl(LineType.Base, graphics.BaseSlotsLine);
        Lines[LineType.Blueprint] = new LineCtrl(LineType.Blueprint, graphics.BlueprintSlotsLine);
        Lines[LineType.Inventory] =
            new LineCtrl(LineType.Inventory, graphics.InventoryInspectionPopup.InventorySlotsLine);
        Lines[LineType.Equipment] = new LineCtrl(LineType.Equipment, graphics.CharacterWindow.EquipmentSlotsLine);
    }

    /// <summary>
    /// 获取CardLineCtrl
    /// </summary>
    /// <param name="type">卡槽类型</param>
    /// <returns>LineCtrl</returns>
    [CanBeNull]
    public static LineCtrl GetLine(LineType type)
    {
        return Lines.TryGetValue(type, out var line) ? line : null;
    }

    public static IEnumerable<LineCtrl> GetLines()
    {
        return Lines.Values;
    }

    public static LineType? ResolveLineType(CardLine line)
    {
        var graphics = GraphicsManager.Instance;
        if (line == graphics.ItemSlotsLine)
        {
            return LineType.Hand;
        }

        if (line == graphics.LocationSlotsLine)
        {
            return LineType.Location;
        }

        if (line == graphics.BaseSlotsLine)
        {
            return LineType.Base;
        }

        if (line == graphics.BlueprintSlotsLine)
        {
            return LineType.Blueprint;
        }

        if (line == graphics.InventoryInspectionPopup.InventorySlotsLine)
        {
            return LineType.Inventory;
        }

        if (line == graphics.CharacterWindow.EquipmentSlotsLine)
        {
            return LineType.Equipment;
        }

        return null;
    }

    private readonly CardLine _line;

    private readonly LineType _type;

    private readonly RectTransform[] _transScales;

    private readonly Transform _transView;

    private readonly Dictionary<LineStatus, ScaleCtrl> _scaleCtrlDict;

    private readonly DoubleLineArg _args;

    private LineStatus _status = LineStatus.Initial;

    public LineCtrl(LineType type, CardLine line)
    {
        _type = type;
        _line = line;
        _transScales = LineDelegates[type].trans(line);
        _scaleCtrlDict = LineDelegates[type].scale(_transScales.First());
        _transView = ResolveView();
        if (DoubleArgs.TryGetValue(_type, out var arg)) _args = arg;
    }

    private Transform ResolveView()
    {
        var graphics = GraphicsManager.Instance;

        return _type switch
        {
            LineType.Base => graphics.BaseParent,
            LineType.Blueprint => graphics.BlueprintParent,
            LineType.Equipment => graphics.CharacterWindow.EquipmentsParent,
            LineType.Hand => graphics.ItemsParent,
            LineType.Inventory => graphics.InventoryInspectionPopup.InventorySlotsParent,
            LineType.Location => graphics.LocationParent,
            _ => null
        };
    }

    public LineStatus Status
    {
        get => _status;
        set
        {
            if (_status == value) return;
            ApplyScale(value);
            _status = value;
            DragCtrl.UpdateStatus();
        }
    }

    // public CardLine Line => _line;

    public DoubleLineArg Args => _args;

    public void CheckStatus()
    {
        if (IsAllowDoubleLine()) Status = LineStatus.DoubleLine;
        else if (IsAllowScaleDown()) Status = LineStatus.ScaleDown;
        else Status = LineStatus.Initial;
    }

    public bool IsAllowScaleDown()
    {
        return ConfigManager.IsEnable("SlotScale", $"Enable{Enum.GetName(typeof(LineType), _type)}");
    }

    public bool IsAllowDoubleLine()
    {
        if (_type == LineType.Inventory && ConfigManager.IsEnable("Special", "EnableInventoryDynamicDoubleLine"))
        {
            return (_line.Count - _line.InactiveElements) > 8;
        }

        return ConfigManager.IsEnable("DoubleLine", $"Enable{Enum.GetName(typeof(LineType), _type)}");
    }

    private void ApplyScale(LineStatus status)
    {
        if (!_scaleCtrlDict.TryGetValue(status, out var ctrl)) return;
        foreach (var trans in _transScales)
        {
            if (trans is null) continue;
            ctrl.Apply(trans);
        }

        _line.RecalculateSize = true;
    }

    public void DoubleLine(int _Index, ref Vector3 __result)
    {
        if (_transView is null || _type != LineType.Blueprint && _line.ExtraSpaces is not { Count: > 0 }) return;
        var spaces = _line.ExtraSpaces;

        var padding = _args.Padding;
        var margin = _args.Margin;
        var offset = _args.Offset;

        var count = _line.AllElements.Count - _line.InactiveElements;

        if (count % 2 == 1)
        {
            if (_type == LineType.Hand) margin.x = 0;
            count++;
        }

        _line.Size = _line.Spacing * 0.5f * count;

        if (_line.ExtraSpaces != null)
        {
            foreach (var dynamicViewExtraSpace in _line.ExtraSpaces)
                _line.Size += dynamicViewExtraSpace.Space;
        }

        if (_line.AddedSize && _line.AddedSize.gameObject.activeSelf)
        {
            if (_line.LayoutOrientation == RectTransform.Axis.Horizontal)
                _line.Size += _line.AddedSize.rect.width;
            else _line.Size += _line.AddedSize.rect.height;
        }

        if (_line.LayoutOrientation == RectTransform.Axis.Horizontal)
            _line.Size += _line.Padding.left + _line.Padding.right;
        else _line.Size += _line.Padding.top + _line.Padding.right;

        _line.Size = Mathf.Max(_line.Size, _line.MinSize);

        _transView.GetComponent<RectTransform>().sizeDelta = new Vector2(_line.Size, 0f);

        var num = spaces.Where(space => space.AtIndex <= _Index).Sum(space => space.Space);

        __result = _line.LayoutOriginPos / offset +
                   _line.LayoutDirection * (_line.Spacing * (int)(_Index / 2) + num) + padding * (_Index % 2) + margin;
    }


    /// <summary>
    /// 设置槽位数量
    /// </summary>
    /// <param name="num">数量</param>
    public void SetSlotNum(int num)
    {
        if (num < 0) num = 0;

        if (num > _line.Count)
        {
            for (var i = _line.Count; i < num; i++) _line.AddSlot(_line.Count);
        }
        else
        {
            for (var i = _line.Count; i > num; i--)
            {
                var slot = _line.Slots[_line.Count - 1];
                for (var j = slot.CardPileCount(); j > 0; j--)
                {
                    try
                    {
                        slot.AssignedCard.SwapCard();
                    }
                    catch (NullReferenceException)
                    {
                    }
                }

                _line.RemoveSlot(_line.Count - 1);
            }
        }

        if (_type != LineType.Hand) return;
        if (num == 0) GraphicsManager.Instance.MinItemSlots = 0;
        GraphicsManager.Instance.MaxItemSlots = num;
    }
}