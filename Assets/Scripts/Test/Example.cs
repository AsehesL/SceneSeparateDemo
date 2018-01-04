using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TestSceneBlockObject : ISceneBlockObject
{
    public Bounds Bounds
    {
        get { return m_Bounds; }
    }

    public void OnHide()
    {
        if (m_LoadedPrefab)
        {
            Object.Destroy(m_LoadedPrefab);
            m_LoadedPrefab = null;
        }
        if (m_Obj)
        {
            Resources.UnloadAsset(m_Obj);
            m_Obj = null;
        }
    }

    public bool OnShow(Transform parent)
    {
        if (m_LoadedPrefab == null && m_Obj == null)
        {
            m_Obj = Resources.Load<GameObject>(resPath);
            m_LoadedPrefab = UnityEngine.Object.Instantiate<GameObject>(m_Obj);
            m_LoadedPrefab.transform.SetParent(parent);
            m_LoadedPrefab.transform.position = position;
            m_LoadedPrefab.transform.eulerAngles = rotation;
            m_LoadedPrefab.transform.localScale = size;
            return true;
        }
        return false;
    }

    [SerializeField]
    private Bounds m_Bounds;

    public string resPath;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 size;

    private GameObject m_LoadedPrefab;
    private GameObject m_Obj;
}

public class Example : MonoBehaviour
{
    public List<TestSceneBlockObject> loadObjects;

    public Bounds bounds;

    private Vector3 areaSize;

    public int maxCreateCount;

    //public 

    private SceneBlockQuadTreeManager m_Manager;

    void Start()
    {
        m_Manager = gameObject.GetComponent<SceneBlockQuadTreeManager>();
        if (m_Manager == null)
            m_Manager = gameObject.AddComponent<SceneBlockQuadTreeManager>();
        //m_Manager.Init()
    }
    
    void Update()
    {

    }
}
