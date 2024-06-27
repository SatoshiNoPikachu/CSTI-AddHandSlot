using System.Collections.Generic;
using UnityEngine;

namespace AddHandSlot;

public static class Helper
{
    public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
    {
        key = pair.Key;
        value = pair.Value;
    }

    public static T SafeAccess<T>(this T obj) where T : Object
    {
        return !obj ? null : obj;
    }
    
    public static T GetComponent<T>(this Component m, string path) where T : Component
    {
        return m.transform.Find(path)?.GetComponent<T>();
    }
}