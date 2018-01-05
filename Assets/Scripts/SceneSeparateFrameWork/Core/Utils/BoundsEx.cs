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
    public static bool IsBoundsInCamera(this Bounds bounds, Camera camera)
    {
        
        Matrix4x4 matrix = camera.projectionMatrix*camera.worldToCameraMatrix;
        
        Vector3 projPos =
            matrix.MultiplyPoint(new Vector3(bounds.center.x + bounds.size.x/2, bounds.center.y + bounds.size.y/2,
                bounds.center.z + bounds.size.z/2));
        Vector3 max = projPos;
        Vector3 min = projPos;

        projPos =
            matrix.MultiplyPoint(new Vector3(bounds.center.x - bounds.size.x / 2, bounds.center.y + bounds.size.y / 2,
                bounds.center.z + bounds.size.z / 2));
        max = Vector3.Max(projPos, max);
        min = Vector3.Min(projPos, min);

        projPos =
            matrix.MultiplyPoint(new Vector3(bounds.center.x + bounds.size.x / 2, bounds.center.y - bounds.size.y / 2,
                bounds.center.z + bounds.size.z / 2));
        max = Vector3.Max(projPos, max);
        min = Vector3.Min(projPos, min);

        projPos =
            matrix.MultiplyPoint(new Vector3(bounds.center.x - bounds.size.x / 2, bounds.center.y - bounds.size.y / 2,
                bounds.center.z + bounds.size.z / 2));
        max = Vector3.Max(projPos, max);
        min = Vector3.Min(projPos, min);

        projPos =
            matrix.MultiplyPoint(new Vector3(bounds.center.x + bounds.size.x / 2, bounds.center.y + bounds.size.y / 2,
                bounds.center.z - bounds.size.z / 2));
        max = Vector3.Max(projPos, max);
        min = Vector3.Min(projPos, min);

        projPos =
            matrix.MultiplyPoint(new Vector3(bounds.center.x - bounds.size.x / 2, bounds.center.y + bounds.size.y / 2,
                bounds.center.z - bounds.size.z / 2));
        max = Vector3.Max(projPos, max);
        min = Vector3.Min(projPos, min);

        projPos =
            matrix.MultiplyPoint(new Vector3(bounds.center.x + bounds.size.x / 2, bounds.center.y - bounds.size.y / 2,
                bounds.center.z - bounds.size.z / 2));
        max = Vector3.Max(projPos, max);
        min = Vector3.Min(projPos, min);

        projPos =
            matrix.MultiplyPoint(new Vector3(bounds.center.x - bounds.size.x / 2, bounds.center.y - bounds.size.y / 2,
                bounds.center.z - bounds.size.z / 2));
        max = Vector3.Max(projPos, max);
        min = Vector3.Min(projPos, min);

        if (camera.orthographic)
        {
            if (max.x < -1 || min.x > 1) return false;
            if (max.y < -1 || min.y > 1) return false;
            if (max.z < -1 || min.z > 1) return false;
        }
        else
        {
            if ((max.z < 1 && max.x < -1) || (max.z >= 1 && max.x > 1) || (min.z < 1 && min.z > 1) ||
                (min.z >= 1 && min.z < -1)) return false;
            if ((max.z < 1 && max.y < -1) || (max.z >= 1 && max.y > 1) || (min.z < 1 && min.y > 1) ||
                (min.z >= 1 && min.y < -1)) return false;
            if (max.z < -1 || min.z > 1) return false;
        }

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
