using System;
using UnityEngine;

namespace AddHandSlot;

public class PosCtrl
{
    public float X { get; private set; }
    public float Y { get; private set; }

    public PosCtrl(Transform trans, float? x = null, float? y = null)
    {
        Set(trans, x, y);
    }

    public void Set(Transform trans, float? x, float? y)
    {
        if (trans is null)
        {
            if (x is null || y is null) throw new ArgumentException();

            X = (float)x;
            Y = (float)y;
        }
        else
        {
            var pos = trans.localPosition;
            X = x ?? pos.x;
            Y = y ?? pos.y;
        }
    }

    public void Apply(Transform trans)
    {
        trans.localPosition = new Vector3(X, Y, 1f);
    }
}