using UnityEngine;
using System.Collections;

public static class GizmosEx 
{
    /// <summary>
    /// 绘制视锥体
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="color"></param>
    public static void DrawViewFrustum(Camera camera, Color color)
    {
        Vector3 p1 = camera.projectionMatrix.inverse.MultiplyPoint(new Vector3(-1, -1, -1));
        Vector3 p2 = camera.projectionMatrix.inverse.MultiplyPoint(new Vector3(1, -1, -1));
        Vector3 p3 = camera.projectionMatrix.inverse.MultiplyPoint(new Vector3(1, -1, 1));
        Vector3 p4 = camera.projectionMatrix.inverse.MultiplyPoint(new Vector3(-1, -1, 1));
        Vector3 p5 = camera.projectionMatrix.inverse.MultiplyPoint(new Vector3(-1, 1, -1));
        Vector3 p6 = camera.projectionMatrix.inverse.MultiplyPoint(new Vector3(1, 1, -1));
        Vector3 p7 = camera.projectionMatrix.inverse.MultiplyPoint(new Vector3(1, 1, 1));
        Vector3 p8 = camera.projectionMatrix.inverse.MultiplyPoint(new Vector3(-1, 1, 1));

        p1 = camera.cameraToWorldMatrix.MultiplyPoint(p1);
        p2 = camera.cameraToWorldMatrix.MultiplyPoint(p2);
        p3 = camera.cameraToWorldMatrix.MultiplyPoint(p3);
        p4 = camera.cameraToWorldMatrix.MultiplyPoint(p4);
        p5 = camera.cameraToWorldMatrix.MultiplyPoint(p5);
        p6 = camera.cameraToWorldMatrix.MultiplyPoint(p6);
        p7 = camera.cameraToWorldMatrix.MultiplyPoint(p7);
        p8 = camera.cameraToWorldMatrix.MultiplyPoint(p8);

        Gizmos.color = color;

        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);

        Gizmos.DrawLine(p5, p6);
        Gizmos.DrawLine(p6, p7);
        Gizmos.DrawLine(p7, p8);
        Gizmos.DrawLine(p8, p5);

        Gizmos.DrawLine(p1, p5);
        Gizmos.DrawLine(p2, p6);
        Gizmos.DrawLine(p3, p7);
        Gizmos.DrawLine(p4, p8);
    }
}
