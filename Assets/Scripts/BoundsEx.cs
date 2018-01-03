using UnityEngine;
using System.Collections;

public static class BoundsEx
{

    public static void DrawBounds(this Bounds bounds, float H, float S, float V)
    {
        if (H > 1)
            H = 0;
        Color col = Color.HSVToRGB(H, S, V);
        DrawBounds(bounds, col);
    }

    public static void DrawBounds(this Bounds bounds, Color color)
    {
        Gizmos.color = color;

        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }

    public static bool IsBoundsContainsAnotherBounds(this Bounds bounds, Bounds compareTo)
    {
        //if (!bounds.Contains(compareTo.center))
        //    return false;
        //if (bounds.size.x < compareTo.size.x)
        //    return false;
        //if (bounds.size.y < compareTo.size.y)
        //    return false;
        //if (bounds.size.z < compareTo.size.z)
        //    return false;
        //return true;
        if (!bounds.Contains(compareTo.center + new Vector3(-compareTo.size.x / 2, compareTo.size.y / 2, -compareTo.size.z / 2)))
            return false;
        if (!bounds.Contains(compareTo.center + new Vector3(compareTo.size.x / 2, compareTo.size.y / 2, -compareTo.size.z / 2)))
            return false;
        if (!bounds.Contains(compareTo.center + new Vector3(compareTo.size.x / 2, compareTo.size.y / 2, compareTo.size.z / 2)))
            return false;
        if (!bounds.Contains(compareTo.center + new Vector3(-compareTo.size.x / 2, compareTo.size.y / 2, compareTo.size.z / 2)))
            return false;
        if (!bounds.Contains(compareTo.center + new Vector3(-compareTo.size.x / 2, -compareTo.size.y / 2, -compareTo.size.z / 2)))
            return false;
        if (!bounds.Contains(compareTo.center + new Vector3(compareTo.size.x / 2, -compareTo.size.y / 2, -compareTo.size.z / 2)))
            return false;
        if (!bounds.Contains(compareTo.center + new Vector3(compareTo.size.x / 2, -compareTo.size.y / 2, compareTo.size.z / 2)))
            return false;
        if (!bounds.Contains(compareTo.center + new Vector3(-compareTo.size.x / 2, -compareTo.size.y / 2, compareTo.size.z / 2)))
            return false;
        return true;
    }
}
