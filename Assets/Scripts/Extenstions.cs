using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extenstions
{
    public static Vector4 ToVector(this Color _color)
    {
        return new Vector4(_color.r, _color.g, _color.b, _color.a);
    }

    public static Color ToColor(this Vector4 _vector)
    {
        return new Color(_vector.x, _vector.y, _vector.z, _vector.w);
    }
    public static int Length(this Vector4 _vector)
    {
        return Mathf.RoundToInt(_vector.x + _vector.y+_vector.z+ _vector.w);
    }
}
