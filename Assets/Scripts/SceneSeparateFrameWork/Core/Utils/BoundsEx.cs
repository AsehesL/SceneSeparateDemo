using UnityEngine;
using System.Collections;

public static class BoundsEx
{
    /// <summary>
    /// 绘制包围盒
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="color"></param>
    public static void DrawBounds(this Bounds bounds, Color color)
    {
        Gizmos.color = color;

        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }

    /// <summary>
    /// 判断包围盒是否被相机裁剪
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="camera"></param>
    /// <returns></returns>
    public static bool IsBoundsOutOfCamera(this Bounds bounds, Camera camera)
    {
        Matrix4x4 matrix = camera.projectionMatrix*camera.worldToCameraMatrix;
        Vector3 projPos =
            matrix.MultiplyPoint(new Vector3(bounds.center.x + bounds.size.x/2, bounds.center.y + bounds.size.y/2,
                bounds.center.z + bounds.size.z/2));
        if (!IsPosOutOfCCV(projPos))
            return false;
        projPos =
            matrix.MultiplyPoint(new Vector3(bounds.center.x - bounds.size.x / 2, bounds.center.y + bounds.size.y / 2,
                bounds.center.z + bounds.size.z / 2));
        if (!IsPosOutOfCCV(projPos))
            return false;
        projPos =
            matrix.MultiplyPoint(new Vector3(bounds.center.x + bounds.size.x / 2, bounds.center.y - bounds.size.y / 2,
                bounds.center.z + bounds.size.z / 2));
        if (!IsPosOutOfCCV(projPos))
            return false;
        projPos =
            matrix.MultiplyPoint(new Vector3(bounds.center.x - bounds.size.x / 2, bounds.center.y - bounds.size.y / 2,
                bounds.center.z + bounds.size.z / 2));
        if (!IsPosOutOfCCV(projPos))
            return false;
        projPos =
            matrix.MultiplyPoint(new Vector3(bounds.center.x + bounds.size.x / 2, bounds.center.y + bounds.size.y / 2,
                bounds.center.z - bounds.size.z / 2));
        if (!IsPosOutOfCCV(projPos))
            return false;
        projPos =
            matrix.MultiplyPoint(new Vector3(bounds.center.x - bounds.size.x / 2, bounds.center.y + bounds.size.y / 2,
                bounds.center.z - bounds.size.z / 2));
        if (!IsPosOutOfCCV(projPos))
            return false;
        projPos =
            matrix.MultiplyPoint(new Vector3(bounds.center.x + bounds.size.x / 2, bounds.center.y - bounds.size.y / 2,
                bounds.center.z - bounds.size.z / 2));
        if (!IsPosOutOfCCV(projPos))
            return false;
        projPos =
            matrix.MultiplyPoint(new Vector3(bounds.center.x - bounds.size.x / 2, bounds.center.y - bounds.size.y / 2,
                bounds.center.z - bounds.size.z / 2));
        if (!IsPosOutOfCCV(projPos))
            return false;
        return true;
    }

    /// <summary>
    /// 判断包围盒是否包含另一个包围盒
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="compareTo"></param>
    /// <returns></returns>
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

    private static bool IsPosOutOfCCV(Vector3 projPos)
    {
        if (projPos.x <= -1 || projPos.x >= 1 || projPos.y <= -1 || projPos.y >= 1 || projPos.z <= -1 || projPos.z >= 1)
            return true;
        return false;
    }
}
