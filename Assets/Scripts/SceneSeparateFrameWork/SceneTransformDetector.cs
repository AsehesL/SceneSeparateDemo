using UnityEngine;
using System.Collections;

/// <summary>
/// 该触发器根据Transform的包围盒区域触发
/// </summary>
public class SceneTransformDetector : SceneDetectorBase
{
    public Vector3 detectorSize;

    protected Bounds m_Bounds;

    protected virtual void RefreshBounds()
    {
        m_Bounds.center = Position;
        m_Bounds.size = detectorSize;
    }

    public override bool IsDetected(Bounds bounds)
    {
        RefreshBounds();
        return bounds.Intersects(m_Bounds);
    }

    public override int DetectedCode(Bounds bounds, SceneSeparateTreeType treeType)
    {
        RefreshBounds();
        int code = treeType == SceneSeparateTreeType.OcTree ? CalculateOcTreeBoundsCode(m_Bounds, bounds) : CalculateQuadBoundsCode(m_Bounds, bounds);
        return code;
    }

    protected static int CalculateOcTreeBoundsCode(Bounds detectBounds, Bounds targetBounds)
    {
        if (detectBounds.max.x < targetBounds.min.x || detectBounds.min.x > targetBounds.max.x)
            return 0;
        if (detectBounds.max.y < targetBounds.min.y || detectBounds.min.y > targetBounds.max.y)
            return 0;
        if (detectBounds.max.z < targetBounds.min.z || detectBounds.min.z > targetBounds.max.z)
            return 0;
        int code = 255;
        if (detectBounds.max.x < targetBounds.center.x)
        {
            code = CodeMask(code, 16);
            code = CodeMask(code, 32);
            code = CodeMask(code, 64);
            code = CodeMask(code, 128);
        }
        if (detectBounds.min.x > targetBounds.center.x)
        {
            code = CodeMask(code, 1);
            code = CodeMask(code, 4);
            code = CodeMask(code, 2);
            code = CodeMask(code, 8);
        }

        if (detectBounds.max.y < targetBounds.center.y)
        {
            code = CodeMask(code, 2);
            code = CodeMask(code, 8);
            code = CodeMask(code, 32);
            code = CodeMask(code, 128);
        }
        if (detectBounds.min.y > targetBounds.center.y)
        {
            code = CodeMask(code, 1);
            code = CodeMask(code, 4);
            code = CodeMask(code, 16);
            code = CodeMask(code, 64);
        }

        if (detectBounds.max.z < targetBounds.center.z)
        {
            code = CodeMask(code, 4);
            code = CodeMask(code, 64);
            code = CodeMask(code, 8);
            code = CodeMask(code, 128);
        }
        if (detectBounds.min.z > targetBounds.center.z)
        {
            code = CodeMask(code, 1);
            code = CodeMask(code, 16);
            code = CodeMask(code, 2);
            code = CodeMask(code, 32);
        }
        return code;
    }

    protected static int CalculateQuadBoundsCode(Bounds detectBounds, Bounds targetBounds)
    {

        if (detectBounds.max.x < targetBounds.min.x || detectBounds.min.x > targetBounds.max.x)
            return 0;
        if (detectBounds.max.y < targetBounds.min.y || detectBounds.min.y > targetBounds.max.y)
            return 0;
        if (detectBounds.max.z < targetBounds.min.z || detectBounds.min.z > targetBounds.max.z)
            return 0;
        int code = 15;
        if (detectBounds.max.x < targetBounds.center.x)
        {
            code = CodeMask(code, 4);
            code = CodeMask(code, 8);
        }
        if (detectBounds.min.x > targetBounds.center.x)
        {
            code = CodeMask(code, 1);
            code = CodeMask(code, 2);
        }
        
        if (detectBounds.max.z < targetBounds.center.z)
        {
            code = CodeMask(code, 2);
            code = CodeMask(code, 8);
        }
        if (detectBounds.min.z > targetBounds.center.z)
        {
            code = CodeMask(code, 1);
            code = CodeMask(code, 4);
        }
        return code;
    }

    private static int CodeMask(int code, int mask)
    {
        if ((code & mask) != 0)
            code = code ^ mask;
        return code;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Bounds b = new Bounds(transform.position, detectorSize);
        b.DrawBounds(Color.yellow);
    }
#endif
}
