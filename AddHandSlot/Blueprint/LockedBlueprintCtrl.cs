using AddHandSlot.Line;
using UnityEngine;
using UnityEngine.UI;

namespace AddHandSlot.Blueprint;

public class LockedBlueprintCtrl : MBSingleton<LockedBlueprintCtrl>
{
    public static void OnBlueprintModelsScreenAwake(BlueprintModelsScreen screen)
    {
        if (!screen) return;
        screen.LockedBlueprintsParent?.gameObject.AddComponent<LockedBlueprintCtrl>();
    }

    private GridLayoutGroup _gridLayoutGroup;
    private RectTransform _image;
    private ScaleCtrl _scaleInitial;
    private ScaleCtrl _scaleDouble;

    private void Awake()
    {
        var horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
        if (horizontalLayoutGroup is not null) DestroyImmediate(horizontalLayoutGroup);

        _gridLayoutGroup = gameObject.AddComponent<GridLayoutGroup>();
        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        // _gridLayoutGroup.childAlignment = TextAnchor.UpperLeft;
        _gridLayoutGroup.cellSize = new Vector2(210, 310);
        _gridLayoutGroup.spacing = new Vector2(30, 15);
        _gridLayoutGroup.padding.left = 15;

        _image = transform.GetComponent<RectTransform>("Image");
        _scaleInitial = new ScaleCtrl(null, null, _image);
        _scaleDouble = new ScaleCtrl(null, 685, _image);

        var ctrl = LineCtrl.GetCtrl(LineType.Blueprint);
        if (ctrl is not null) OnLineStatusChange(ctrl.Status);
    }

    public void OnLineStatusChange(LineStatus status)
    {
        if (_gridLayoutGroup is null || _image is null) return;

        if (status is LineStatus.DoubleLine)
        {
            _gridLayoutGroup.constraintCount = 2;
            _gridLayoutGroup.padding.top = 40;
            _scaleDouble.ApplySize(_image);
        }
        else
        {
            _gridLayoutGroup.constraintCount = 1;
            _gridLayoutGroup.padding.top = 36;
            _scaleInitial.ApplySize(_image);
        }
    }
}