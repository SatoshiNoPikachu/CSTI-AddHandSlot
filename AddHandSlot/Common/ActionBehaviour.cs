using System;
using UnityEngine;

namespace AddHandSlot.Common;

public class ActionBehaviour : MonoBehaviour
{
    public static ActionBehaviour Create(GameObject go)
    {
        return go?.AddComponent<ActionBehaviour>();
    }

    public static ActionBehaviour Create(MonoBehaviour mb)
    {
        return mb?.gameObject.AddComponent<ActionBehaviour>();
    }

    // public Action OnAwakeAction { get; set; }

    public Action OnDestroyAction { get; set; }

    public Action OnEnableAction { get; set; }

    public Action OnStartAction { get; set; }

    public Action OnUpdateAction { get; set; }

    private void OnDestroy()
    {
        OnDestroyAction?.Invoke();
    }

    private void OnEnable()
    {
        OnEnableAction?.Invoke();
    }

    private void Start()
    {
        OnStartAction?.Invoke();
    }

    private void Update()
    {
        OnUpdateAction?.Invoke();
    }

    public void Destroy()
    {
        Destroy(this);
    }
}