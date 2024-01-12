using System;
using UnityEngine;

namespace AddHandSlot;

public class ScaleCtrl
{
    private bool _isSetScale;
    private bool _isSetSize;
    public float X { get; private set; }
    public float Y { get; private set; }
    public float W { get; private set; }
    public float H { get; private set; }

    public ScaleCtrl(RectTransform trans)
    {
        Set(trans);
    }

    public ScaleCtrl(RectTransform trans, float? x, float? y)
    {
        Set(trans, x, y);
    }

    public ScaleCtrl(float? w, float? h, RectTransform trans)
    {
        Set(w, h, trans);
    }

    public ScaleCtrl(RectTransform trans, float? x, float? y, float? w, float? h)
    {
        Set(trans, x, y, w, h);
    }

    public void Set(RectTransform trans)
    {
        var scale = trans.localScale;
        var size = trans.sizeDelta;
        X = scale.x;
        Y = scale.y;
        W = size.x;
        H = size.y;
        _isSetScale = true;
        _isSetSize = true;
    }

    public void Set(RectTransform trans, float? x, float? y)
    {
        if (trans is null)
        {
            if (x is null || y is null) throw new ArgumentException();

            X = (float)x;
            Y = (float)y;
        }
        else
        {
            var scale = trans.localScale;
            X = x ?? scale.x;
            Y = y ?? scale.y;
        }

        _isSetScale = true;
    }

    public void Set(float? w, float? h, RectTransform trans)
    {
        if (trans is null)
        {
            if (w is null || h is null) throw new ArgumentException();

            W = (float)w;
            H = (float)h;
        }
        else
        {
            var size = trans.sizeDelta;
            W = w ?? size.x;
            H = h ?? size.y;
        }

        _isSetSize = true;
    }

    public void Set(RectTransform trans, float? x, float? y, float? w, float? h)
    {
        if (trans is null)
        {
            if (x is null || y is null || w is null || h is null) throw new ArgumentException();

            X = (float)x;
            Y = (float)y;
            W = (float)w;
            H = (float)h;
        }
        else
        {
            var scale = trans.localScale;
            var size = trans.sizeDelta;
            X = x ?? scale.x;
            Y = y ?? scale.y;
            W = w ?? size.x;
            H = h ?? size.y;
        }

        _isSetScale = true;
        _isSetSize = true;
    }

    public void Apply(RectTransform trans)
    {
        ApplyScale(trans);
        ApplySize(trans);
    }

    public void ApplyScale(Transform trans)
    {
        if (_isSetScale) trans.localScale = new Vector3(X, Y, 1f);
    }

    public void ApplySize(RectTransform trans)
    {
        if (_isSetSize) trans.sizeDelta = new Vector2(W, H);
    }
}