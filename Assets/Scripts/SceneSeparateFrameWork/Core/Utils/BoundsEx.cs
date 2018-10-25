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

        int code =
            MatrixEx.ComputeOutCode(new Vector4(bounds.center.x + bounds.size.x/2, bounds.center.y + bounds.size.y/2,
                bounds.center.z + bounds.size.z/2, 1), matrix);


        code &=
            MatrixEx.ComputeOutCode(new Vector4(bounds.center.x - bounds.size.x/2, bounds.center.y + bounds.size.y/2,
                bounds.center.z + bounds.size.z/2, 1), matrix);

        code &=
            MatrixEx.ComputeOutCode(new Vector4(bounds.center.x + bounds.size.x/2, bounds.center.y - bounds.size.y/2,
                bounds.center.z + bounds.size.z/2, 1), matrix);

        code &=
            MatrixEx.ComputeOutCode(new Vector4(bounds.center.x - bounds.size.x/2, bounds.center.y - bounds.size.y/2,
                bounds.center.z + bounds.size.z/2, 1), matrix);

        code &=
            MatrixEx.ComputeOutCode(new Vector4(bounds.center.x + bounds.size.x/2, bounds.center.y + bounds.size.y/2,
                bounds.center.z - bounds.size.z/2, 1), matrix);

        code &=
            MatrixEx.ComputeOutCode(new Vector4(bounds.center.x - bounds.size.x/2, bounds.center.y + bounds.size.y/2,
                bounds.center.z - bounds.size.z/2, 1), matrix);

        code &=
            MatrixEx.ComputeOutCode(new Vector4(bounds.center.x + bounds.size.x/2, bounds.center.y - bounds.size.y/2,
                bounds.center.z - bounds.size.z/2, 1), matrix);

        code &=
            MatrixEx.ComputeOutCode(new Vector4(bounds.center.x - bounds.size.x/2, bounds.center.y - bounds.size.y/2,
                bounds.center.z - bounds.size.z/2, 1), matrix);


        if (code != 0) return false;

        return true;
    }

    public static bool IsBoundsInCameraEx(this Bounds bounds, Camera camera, float leftex, float rightex, float downex,
        float upex)
    {

        Matrix4x4 matrix = camera.projectionMatrix*camera.worldToCameraMatrix;

        int code =
            MatrixEx.ComputeOutCodeEx(new Vector4(bounds.center.x + bounds.size.x/2, bounds.center.y + bounds.size.y/2,
                bounds.center.z + bounds.size.z/2, 1), matrix, leftex, rightex, downex, upex);


        code &=
            MatrixEx.ComputeOutCodeEx(new Vector4(bounds.center.x - bounds.size.x/2, bounds.center.y + bounds.size.y/2,
                bounds.center.z + bounds.size.z/2, 1), matrix, leftex, rightex, downex, upex);

        code &=
            MatrixEx.ComputeOutCodeEx(new Vector4(bounds.center.x + bounds.size.x/2, bounds.center.y - bounds.size.y/2,
                bounds.center.z + bounds.size.z/2, 1), matrix, leftex, rightex, downex, upex);

        code &=
            MatrixEx.ComputeOutCodeEx(new Vector4(bounds.center.x - bounds.size.x/2, bounds.center.y - bounds.size.y/2,
                bounds.center.z + bounds.size.z/2, 1), matrix, leftex, rightex, downex, upex);

        code &=
            MatrixEx.ComputeOutCodeEx(new Vector4(bounds.center.x + bounds.size.x/2, bounds.center.y + bounds.size.y/2,
                bounds.center.z - bounds.size.z/2, 1), matrix, leftex, rightex, downex, upex);

        code &=
            MatrixEx.ComputeOutCodeEx(new Vector4(bounds.center.x - bounds.size.x/2, bounds.center.y + bounds.size.y/2,
                bounds.center.z - bounds.size.z/2, 1), matrix, leftex, rightex, downex, upex);

        code &=
            MatrixEx.ComputeOutCodeEx(new Vector4(bounds.center.x + bounds.size.x/2, bounds.center.y - bounds.size.y/2,
                bounds.center.z - bounds.size.z/2, 1), matrix, leftex, rightex, downex, upex);

        code &=
            MatrixEx.ComputeOutCodeEx(new Vector4(bounds.center.x - bounds.size.x/2, bounds.center.y - bounds.size.y/2,
                bounds.center.z - bounds.size.z/2, 1), matrix, leftex, rightex, downex, upex);


        if (code != 0) return false;

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
        if (
            !bounds.Contains(compareTo.center +
                             new Vector3(-compareTo.size.x/2, compareTo.size.y/2, -compareTo.size.z/2)))
            return false;
        if (
            !bounds.Contains(compareTo.center + new Vector3(compareTo.size.x/2, compareTo.size.y/2, -compareTo.size.z/2)))
            return false;
        if (!bounds.Contains(compareTo.center + new Vector3(compareTo.size.x/2, compareTo.size.y/2, compareTo.size.z/2)))
            return false;
        if (
            !bounds.Contains(compareTo.center + new Vector3(-compareTo.size.x/2, compareTo.size.y/2, compareTo.size.z/2)))
            return false;
        if (
            !bounds.Contains(compareTo.center +
                             new Vector3(-compareTo.size.x/2, -compareTo.size.y/2, -compareTo.size.z/2)))
            return false;
        if (
            !bounds.Contains(compareTo.center +
                             new Vector3(compareTo.size.x/2, -compareTo.size.y/2, -compareTo.size.z/2)))
            return false;
        if (
            !bounds.Contains(compareTo.center + new Vector3(compareTo.size.x/2, -compareTo.size.y/2, compareTo.size.z/2)))
            return false;
        if (
            !bounds.Contains(compareTo.center +
                             new Vector3(-compareTo.size.x/2, -compareTo.size.y/2, compareTo.size.z/2)))
            return false;
        return true;
    }
}
