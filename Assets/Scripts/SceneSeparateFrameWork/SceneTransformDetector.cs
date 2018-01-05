using UnityEngine;
using System.Collections;

public class SceneTransformDetector : SceneDetectorBase
{
    public Vector3 detectorSize;

    private Bounds m_Bounds;

    public override bool IsTrigger(Bounds bounds)
    {
        m_Bounds.center = Position;
        m_Bounds.size = detectorSize;
        return bounds.Intersects(m_Bounds);
    }
}
