using UnityEngine;


namespace AddHandSlot.Blueprint;

public class BlueprintTabCtrl : MonoBehaviour
{
    private static BlueprintTabCtrl _instance;

    public static BlueprintTabCtrl Instance
    {
        get => _instance.SafeAccess();
        private set => _instance = value;
    }

    public static void OnBlueprintModelsScreenAwake(BlueprintModelsScreen screen)
    {
        if (!screen) return;
        if (Instance) return;

        if (screen.TabsParent)
        {
            new ScaleCtrl(-185, null, screen.TabsParent).Apply(screen.TabsParent);
        }

        Create(screen);
    }

    private static void Create(BlueprintModelsScreen screen)
    {
        var ctrl = screen.gameObject.AddComponent<BlueprintTabCtrl>();
        ctrl.Setup(screen);
        Instance = ctrl;
    }

    private BlueprintModelsScreen _screen;

    private NextTabButton _nextTabButton;

    private int _currentTab;

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Setup(BlueprintModelsScreen screen)
    {
        _screen = screen;
        _nextTabButton = NextTabButton.CreateTopTabButton(screen);
        _nextTabButton.AddListener(NextTabPage);
    }

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