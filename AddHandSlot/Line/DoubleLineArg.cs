using UnityEngine;

namespace AddHandSlot.Line;

public struct DoubleLineArg
{
    public int Offset;
    public Vector2 Padding;
    public Vector2 Margin;

    public DoubleLineArg(int offset, Vector2 padding, Vector2 margin)
    {
        Offset = offset;
        Padding = padding;
        Margin = margin;
    }
}