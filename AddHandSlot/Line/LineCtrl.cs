using System;
using System.Collections.Generic;
using System.Linq;
using AddHandSlot.Blueprint;
using AddHandSlot.Common;
using AddHandSlot.Stat;
using BepInEx.Configuration;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AddHandSlot.Line;

/// <summary>
/// CardLine控制器
/// </summary>
public class LineCtrl
{
    /// <summary>
    /// 修改手牌数量
    /// </summary>
    public static void ModifyHandSlotNum()
    {
        var statCtrl = new StatCtrl(StatCtrl.UidHandSlotNum);
        if (statCtrl.InGame is null) return;

        ModifyHandSlotNum((int)statCtrl.InGame.SimpleCurrentValue);
    }

    /// <summary>
    /// 修改手牌数量
    /// </summary>
    /// <param name="num">手牌数量</param>
    public static void ModifyHandSlotNum(int num)
    {
        if (GraphicsManager.Instance is null) return;
        GetCtrl(LineType.Hand)?.SetSlotNum(num);
    }

    /// <summary>
    /// 强制增加手牌数量
    /// </summary>
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

    public static void OnExplorationPopupSetup(ExplorationPopup popup)
    {
        var ab = ActionBehaviour.Create(popup);
        var tag = false;
        ab.OnUpdateAction = () =>
        {
            if (tag) return;
            ModifyExplorationSlotNum();
            tag = true;
            ab.Destroy();
        };
    }

    public static void ModifyExplorationSlotNum()
    {
        var ctrl = GetCtrl(LineType.Exploration);
        if (ctrl is null) return;
        ModifyExplorationSlotNum(ctrl.GetSlotNum());
    }

    public static void ModifyExplorationSlotNum(int num)
    {
        if (GraphicsManager.Instance is null) return;
        GetCtrl(LineType.Exploration)?.SetSlotNum(num);
    }

    /// <summary>
    /// 配置项改变时
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public static void OnConfigChange(object sender, EventArgs args)
    {
        if (GraphicsManager.Instance is null) return;
        if (sender is not ConfigEntry<bool> config) return;
        if (!Enum.TryParse<LineType>(config.Definition.Key.Substring(6), out var type)) return;

        var line = GetCtrl(type);
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
        LineDelegates[LineType.Hand] = (line =>
        [
            (RectTransform)line.transform.parent
        ], trans => new Dictionary<LineStatus, ScaleCtrl>
        {
            [LineStatus.Initial] = new(trans),
            [LineStatus.ScaleDown] = new(trans, 0.7f, 0.7f, 380f, 148f),
            [LineStatus.DoubleLine] = new(trans, 0.5f, 0.5f, 1157f, 358f),
        });

        // 环境
        LineDelegates[LineType.Location] = (line =>
        [
            (RectTransform)line.transform.parent
        ], trans => new Dictionary<LineStatus, ScaleCtrl>
        {
            [LineStatus.Initial] = new(trans),
            [LineStatus.ScaleDown] = new(trans, 0.7f, 0.7f, 550f, null),
            [LineStatus.DoubleLine] = new(trans, 0.5f, 0.5f, 1310f, 360f),
        });

        // 基础
        LineDelegates[LineType.Base] = (line =>
        [
            (RectTransform)line.transform.parent
        ], trans => new Dictionary<LineStatus, ScaleCtrl>
        {
            [LineStatus.Initial] = new(trans),
            [LineStatus.ScaleDown] = new(trans, 0.7f, 0.7f, 380f, null),
            [LineStatus.DoubleLine] = new(trans, 0.5f, 0.5f, 1160f, 360f),
        });

        // 蓝图
        LineDelegates[LineType.Blueprint] = (line =>
        [
            (RectTransform)line.transform.parent
        ], trans => new Dictionary<LineStatus, ScaleCtrl>
        {
            [LineStatus.Initial] = new(trans),
            [LineStatus.ScaleDown] = new(trans, 0.7f, 0.7f, 620f, null),
            [LineStatus.DoubleLine] = new(trans, 0.5f, 0.5f, 1460f, 360f),
        });

        // 容器
        LineDelegates[LineType.Inventory] = (line =>
        [
            (RectTransform)line.transform.parent
        ], trans => new Dictionary<LineStatus, ScaleCtrl>
        {
            [LineStatus.Initial] = new(trans),
            [LineStatus.ScaleDown] = new(trans, 0.7f, 0.7f, 500f, 0f),
            [LineStatus.DoubleLine] = new(trans, 0.5f, 0.5f, 1175f, 360f),
        });

        // 装备
        LineDelegates[LineType.Equipment] = (line =>
        [
            (RectTransform)line.transform.parent
        ], trans => new Dictionary<LineStatus, ScaleCtrl>
        {
            [LineStatus.Initial] = new(trans),
            [LineStatus.ScaleDown] = new(trans, 0.7f, 0.7f, 600f, null),
            [LineStatus.DoubleLine] = new(trans, 0.5f, 0.5f, 1400f, 400f),
        });

        // 探索
        LineDelegates[LineType.Exploration] = (line =>
        [
            (RectTransform)line.transform.parent
        ], trans => new Dictionary<LineStatus, ScaleCtrl>
        {
            [LineStatus.Initial] = new(trans),
            [LineStatus.ScaleDown] = new(trans, 0.7f, 0.7f, 1800f, null),
            [LineStatus.DoubleLine] = new(trans, 0.5f, 0.5f, 2500f, 360f),
        });

        DoubleArgs[LineType.Base] = new DoubleLineArg(2, new Vector2(0f, -325f), new Vector2(50f, 360f));

        DoubleArgs[LineType.Blueprint] = new DoubleLineArg(1, new Vector2(0f, -325f), new Vector2(0f, 170f));

        DoubleArgs[LineType.Equipment] = new DoubleLineArg(1, new Vector2(0f, -325f), new Vector2(0f, -45f));

        DoubleArgs[LineType.Hand] = new DoubleLineArg(2, new Vector2(0f, -325f), new Vector2(55f, 162.5f));

        DoubleArgs[LineType.Inventory] = new DoubleLineArg(1, new Vector2(0f, -325f), new Vector2(0f, 170f));

        DoubleArgs[LineType.Location] = new DoubleLineArg(1, new Vector2(0f, -325f), new Vector2(0f, 180f));

        DoubleArgs[LineType.Exploration] = new DoubleLineArg(2, new Vector2(0f, -325f), new Vector2(55f, 162f));
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
        Lines[LineType.Exploration] =
            new LineCtrl(LineType.Exploration, graphics.ExplorationDeckPopup.ExplorationSlotsLine, false);
    }

    /// <summary>
    /// 获取CardLineCtrl
    /// </summary>
    /// <param name="type">卡槽类型</param>
    /// <returns>LineCtrl</returns>
    [CanBeNull]
    public static LineCtrl GetCtrl(LineType type)
    {
        return Lines.TryGetValue(type, out var ctrl) ? ctrl : null;
    }

    public static LineCtrl GetCtrl(CardLine line)
    {
        var type = ResolveLineType(line);
        return type is null ? null : GetCtrl((LineType)type);
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

        if (line == graphics.ExplorationDeckPopup.ExplorationSlotsLine)
        {
            return LineType.Exploration;
        }

        return null;
    }

    public static void OnUpdateList(DynamicViewLayoutGroup group)
    {
        if (group is not CardLine line) return;

        var type = ResolveLineType(line);
        if (type is null) return;

        var ctrl = GetCtrl((LineType)type);
        if (ctrl is null) return;

        if (ctrl.Status == LineStatus.DoubleLine) ctrl.RecalculateSizeOnDoubleLine();
    }

    public static void OnGetElementPosition(DynamicViewLayoutGroup group, int index, ref Vector3 result)
    {
        if (group is not CardLine line) return;

        var type = ResolveLineType(line);
        if (type is null) return;

        var ctrl = GetCtrl((LineType)type);
        if (ctrl is null) return;

        if (ctrl.Status == LineStatus.DoubleLine) ctrl.RecalculatePositionOnDoubleLine(index, ref result);
    }

    public static bool IsRunGetPointerIndex(CardLine line)
    {
        var ctrl = GetCtrl(line);

        return ctrl?.Status is null or not LineStatus.DoubleLine || ctrl._type is LineType.Equipment;
    }

    public static int GetPointerIndex(CardLine line, InGameCardBase card, int result)
    {
        var ctrl = GetCtrl(line);
        if (ctrl?.Status is null or not LineStatus.DoubleLine || ctrl._type is LineType.Equipment) return result;

        var slots = line.Slots;
        if (slots == null)
        {
            return -1;
        }

        var pointer = line.Pointer.position;
        var curIndex = slots.IndexOf(card.CurrentSlot);
        for (var i = 0; i < slots.Count; i++)
        {
            if (!slots[i].IsActive || !slots[i].IsVisible ||
                (slots[i].AssignedCard != null && card.CurrentContainer == slots[i].AssignedCard) ||
                i == curIndex) continue;
            if (pointer.x > slots[i].CurrentRect.xMin && pointer.x < slots[i].CurrentRect.xMax &&
                pointer.y > slots[i].CurrentRect.yMin && pointer.y < slots[i].CurrentRect.yMax)
            {
                return i;
            }

            if (i + 1 == curIndex && pointer.x > slots[0].CurrentRect.xMin) continue;

            float leftX;
            float leftY;
            float rightX;
            float rightY;
            if (pointer.x < slots[0].CurrentRect.xMin && i == 0)
            {
                leftX = line.WorldRect.xMin;
                leftY = line.WorldRect.yMin;
                rightX = slots[0].CurrentRect.xMin;
                rightY = slots[0].CurrentRect.yMin;
            }
            else
            {
                leftX = slots[i].CurrentRect.xMax;
                leftY = slots[i].CurrentRect.yMax;
                rightX = line.WorldRect.xMax;
                rightY = line.WorldRect.yMax;

                if (i < slots.Count - 1)
                {
                    for (var j = i + 1; j < slots.Count; j++)
                    {
                        if (!slots[j].IsActive) continue;
                        if (ctrl._type is not LineType.Hand && i % 2 != j % 2) continue;

                        rightX = slots[j].CurrentRect.xMin;
                        rightY = slots[j].CurrentRect.yMin;
                        break;
                    }

                    // for (int j = i + 1, z = i; j < slots.Count; j++)
                    // {
                    //     if (!slots[j].IsActive) continue;
                    //     z++;
                    //     if (i % 2 != z % 2) continue;
                    //
                    //     rightX = slots[j].CurrentRect.xMin;
                    //     rightY = slots[j].CurrentRect.yMin;
                    //     break;
                    // }
                }
            }

            if (pointer.x <= leftX || pointer.y >= leftY ||
                pointer.x >= rightX || pointer.y <= rightY) continue;

            if (i == 0 && pointer.x < slots[0].CurrentRect.xMin)
            {
                return 0;
            }

            return i + 1;

            // var index = i + 1;
            // for (var j = 0; j < slots.Count; j++)
            // {
            //     if (!slots[j].IsActive)
            //     {
            //         index--;
            //         continue;
            //     }
            //
            //     if (j >= i) break;
            // }

            // return index < 0 ? i + 1 : index;
        }

        return -1;
    }

    private readonly CardLine _line;

    private readonly LineType _type;

    private readonly RectTransform[] _transScales;

    private readonly Transform _transView;

    private readonly Dictionary<LineStatus, ScaleCtrl> _scaleCtrlDict;

    private readonly DoubleLineArg _args;

    private LineStatus _status = LineStatus.Initial;

    private readonly bool _notForceUpdate;

    public LineCtrl(LineType type, CardLine line, bool notForceUpdate = true)
    {
        _type = type;
        _line = line;
        _notForceUpdate = notForceUpdate;
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
            LineType.Exploration => graphics.ExplorationDeckPopup.SlotsParent,
            _ => null
        };
    }

    public LineStatus Status
    {
        get => _status;
        set
        {
            if (_notForceUpdate && _status == value) return;
            ApplyScale(value);
            _status = value;
            DragCtrl.UpdateStatus();
            if (_type is LineType.Blueprint) LockedBlueprintCtrl.Instance?.OnLineStatusChange(value);
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
            return _line.Count - _line.InactiveElements > 8;
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

    private void RecalculateSizeOnDoubleLine()
    {
        if (_transView is null) return;

        if (_type != LineType.Blueprint && _type != LineType.Exploration &&
            _line.ExtraSpaces is not { Count: > 0 }) return;

        var spaces = _line.ExtraSpaces;
        var count = _line.AllElements.Count - _line.InactiveElements;
        if (count % 2 == 1) count++;

        var size = _line.Spacing * 0.5f * count;

        if (spaces is not null)
        {
            size += spaces.Sum(dynamicViewExtraSpace => dynamicViewExtraSpace.Space);
        }

        if (_line.AddedSize && _line.AddedSize.gameObject.activeSelf)
        {
            if (_line.LayoutOrientation == RectTransform.Axis.Horizontal)
            {
                size += _line.AddedSize.rect.width;
            }
            else
            {
                size += _line.AddedSize.rect.height;
            }
        }

        if (_line.LayoutOrientation == RectTransform.Axis.Horizontal)
        {
            size += _line.Padding.left + _line.Padding.right;
        }
        else
        {
            size += _line.Padding.top + _line.Padding.right;
        }

        _line.Size = Mathf.Max(size, _line.MinSize);

        var rt = _transView.GetComponent<RectTransform>();
        new ScaleCtrl(_line.Size, null, rt).Apply(rt);
        // _transView.GetComponent<RectTransform>().sizeDelta = new Vector2(_line.Size, 0f);
    }

    public void RecalculatePositionOnDoubleLine(int _Index, ref Vector3 __result)
    {
        if (_transView is null) return;
        if (_type != LineType.Blueprint && _type != LineType.Exploration &&
            _line.ExtraSpaces is not { Count: > 0 }) return;

        var spaces = _line.ExtraSpaces;
        var padding = _args.Padding;
        var margin = _args.Margin;
        var offset = _args.Offset;

        var count = _line.AllElements.Count - _line.InactiveElements;
        if (count % 2 == 1)
        {
            if (_type is LineType.Hand or LineType.Exploration) margin.x = 0;
            count++;
        }

        var num = spaces.Where(space => space.AtIndex <= _Index).Sum(space => space.Space);

        if (_type is LineType.Hand or LineType.Exploration)
        {
            __result = _line.LayoutOriginPos / offset +
                       _line.LayoutDirection * (_line.Spacing * (_Index % (int)(count / 2.0)) + num) +
                       padding * (_Index < count / 2 ? 0 : 1) + margin;
            return;
        }

        __result = _line.LayoutOriginPos / offset +
                   _line.LayoutDirection * (_line.Spacing * (int)(_Index / 2.0) + num) +
                   padding * (_Index % 2) + margin;
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

        if (_type == LineType.Exploration)
        {
            GraphicsManager.Instance.ExplorationDeckPopup.ExplorationSlotCount = num;

            Status = num switch
            {
                < 7 => LineStatus.Initial,
                < 10 => LineStatus.ScaleDown,
                _ => LineStatus.DoubleLine
            };

            return;
        }

        if (_type != LineType.Hand) return;
        if (num == 0) GraphicsManager.Instance.MinItemSlots = 0;
        GraphicsManager.Instance.MaxItemSlots = num;
    }

    public int GetSlotNum()
    {
        return _line.Count;
    }

    // public int GetActiveSlotNum()
    // {
    //     return _line.AllElements.Count - _line.InactiveElements;
    // }
}