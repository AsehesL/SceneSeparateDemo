using UnityEngine;
using System.Collections;

public class SceneCameraDetector : SceneDetectorBase
{
    private Camera m_Camera;
    
    void Start()
    {
        m_Camera = gameObject.GetComponent<Camera>();
    }

    public override bool IsTrigger(Bounds bounds)
    {
        if (m_Camera == null)
            return false;
        return !bounds.IsBoundsOutOfCamera(m_Camera);
    }
}
