using ModCore.UI;

namespace AddHandSlot.Blueprint;

public class BlueprintTabCtrl : MBSingleton<BlueprintTabCtrl>
{
    public static void OnBlueprintModelsScreenAwake(BlueprintModelsScreen screen)
    {
        if (!screen) return;

        var prefab = UIManager.GetPrefab<ActionButton>(CommonPrefab.UidActionButton);
        if (!prefab) return;

        new ScaleCtrl(-185, null, screen.TabsParent).Apply(screen.TabsParent);

        var ctrl = screen.gameObject.AddComponent<BlueprintTabCtrl>();
        ctrl._screen = screen;

        var btn = Instantiate(prefab, screen.TabsParent.parent, false);
        btn.name = "NextTabButton";
        btn.Text = "→";
        btn.OnClick = ctrl.NextTabPage;
        new PosCtrl(btn.transform, 1385).Apply(btn.transform);
    }

    private BlueprintModelsScreen _screen;

    private int _currentTab;

    public void ChangeTabPage()
    {
        var tabBtnList = _screen.TabButtons;
        if (tabBtnList is null) return;

        var min = _currentTab * 10;
        var max = min + 10;

        for (var i = 0; i < tabBtnList.Count; i++)
        {
            tabBtnList[i]?.gameObject.SetActive(i >= min && i < max);
        }
    }

    public void OnShow()
    {
        if (_screen is null) return;

        _currentTab = _screen.CurrentTab / 10;

        ChangeTabPage();
    }

    private void NextTabPage()
    {
        var tabBtnList = _screen.TabButtons;
        if (tabBtnList is null) return;

        _currentTab++;
        if (_currentTab * 10 > tabBtnList.Count) _currentTab = 0;

        ChangeTabPage();
    }
}