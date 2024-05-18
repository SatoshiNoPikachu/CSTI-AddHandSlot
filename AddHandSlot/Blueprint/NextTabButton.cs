using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AddHandSlot.Blueprint;

public class NextTabButton : MonoBehaviour
{
    private static NextTabButton CreateButton(BlueprintModelsScreen screen)
    {
        if (!screen) return null;

        var parent = screen.TabsParent?.parent;
        if (!parent) return null;

        var target = parent.Find("CloseButton");
        if (!target) return null;

        var go = Instantiate(target.gameObject, parent, false);
        return go.AddComponent<NextTabButton>();
    }

    public static NextTabButton CreateTopTabButton(BlueprintModelsScreen screen)
    {
        var btn = CreateButton(screen);
        if (!btn) return null;

        btn.name = "NextTabButton";
        btn.Setup(1385, "→");

        return btn;
    }

    private Button _button;

    private void Setup(float x, string text)
    {
        var rt = GetComponent<RectTransform>();
        new PosCtrl(rt, x).Apply(rt);

        _button = transform.Find("ButtonObject")?.GetComponent<Button>();
        if (_button) _button.onClick = new Button.ButtonClickedEvent();

        SetText(text);
    }

    private void SetText(string text)
    {
        var textObj = transform.Find("ButtonText");

        var textMeshProUGUI = textObj?.GetComponent<TextMeshProUGUI>();
        if (!textMeshProUGUI) return;

        // var localizedStaticText = textObj.GetComponent<LocalizedStaticText>();
        // if (localizedStaticText is null) return;

        textMeshProUGUI.SetText(text);
        // localizedStaticText.LocalizedStringKey = key;
    }

    // public void SetInteractable(bool interactable)
    // {
    //     _button.interactable = interactable;
    // }

    public void AddListener(UnityAction action)
    {
        _button?.onClick.AddListener(action);
    }
}