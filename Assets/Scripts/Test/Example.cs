using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 测试场景物体-实际应用中可以根据需求增加或修改，只需实现ISceneObject接口即可
/// </summary>
[System.Serializable]
public class TestSceneObject : ISceneObject
{
    [SerializeField]
    private Bounds m_Bounds;
    [SerializeField]
    private string m_ResPath;
    [SerializeField]
    private Vector3 m_Position;
    [SerializeField]
    private Vector3 m_Rotation;
    [SerializeField]
    private Vector3 m_Size;

    private GameObject m_LoadedPrefab;

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
            TestResManager.UnLoad(m_ResPath);
        }
    }

    public bool OnShow(Transform parent)
    {
        if (m_LoadedPrefab == null)
        {
            var obj = TestResManager.Load(m_ResPath);
            m_LoadedPrefab = UnityEngine.Object.Instantiate<GameObject>(obj);
            m_LoadedPrefab.transform.SetParent(parent);
            m_LoadedPrefab.transform.position = m_Position;
            m_LoadedPrefab.transform.eulerAngles = m_Rotation;
            m_LoadedPrefab.transform.localScale = m_Size;
            return true;
        }
        return false;
    }

    public TestSceneObject(Bounds bounds, Vector3 position, Vector3 rotation, Vector3 size, string resPath)
    {
        m_Bounds = bounds;
        m_Position = position;
        m_Rotation = rotation;
        m_Size = size;
        m_ResPath = resPath;
    }


}

public class Example : MonoBehaviour
{
    public string desc;

    public List<TestSceneObject> loadObjects;

    public Bounds bounds;

    public bool asyn;

    public SceneDetectorBase detector;

    private SceneObjectLoadController m_Controller;

    void Start()
    {
        m_Controller = gameObject.GetComponent<SceneObjectLoadController>();
        if (m_Controller == null)
            m_Controller = gameObject.AddComponent<SceneObjectLoadController>();
        m_Controller.Init(bounds.center, bounds.size, asyn, SceneSeparateTreeType.QuadTree);


        for (int i = 0; i < loadObjects.Count; i++)
        {
            m_Controller.AddSceneBlockObject(loadObjects[i]);
        }
    }

    void OnGUI()
    {
        GUI.color = Color.red;
        GUILayout.Label(desc);
    }
    
    void Update()
    {
        m_Controller.RefreshDetector(detector);
    }

    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
        }
        else
        {
            bounds.DrawBounds(Color.green);
        }
    }
}
