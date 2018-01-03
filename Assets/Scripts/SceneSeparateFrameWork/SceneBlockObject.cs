using UnityEngine;
using System.Collections;

public class SceneBlockObject : ISceneBlockObject
{
    public enum CreateFlag
    {
        None,
        DontDestroy,
        Old,
        OutofBounds,
    }

    public enum CreatingProcessFlag
    {
        None,
        IsPrepareCreate,
        IsPrepareDestroy,
    }

    public Bounds Bounds
    {
        get { return m_TargetObj.Bounds; }
    }

    public ISceneBlockObject TargetObj
    {
        get { return m_TargetObj; }
    }

    public CreateFlag Flag { get; set; }

    public CreatingProcessFlag ProcessFlag { get; set; }

    private ISceneBlockObject m_TargetObj;

    public SceneBlockObject(ISceneBlockObject obj)
    {
        m_TargetObj = obj;
    }

    public void OnHide()
    {
        m_TargetObj.OnHide();
    }

    public bool OnShow(Transform parent)
    {
        return m_TargetObj.OnShow(parent);
    }
}
