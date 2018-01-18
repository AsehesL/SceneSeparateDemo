using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof (Example))]
public class ExampleEditor : Editor
{
    private Example m_Target;

    void OnEnable()
    {
        m_Target = target as Example;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("拾取节点下的物体"))
        {
            Undo.RecordObject(target, "PickChilds");
            PickChilds();
        }
    }

    private void PickChilds()
    {
        if (m_Target.transform.childCount == 0)
            return;
        List<TestSceneObject> list = new List<TestSceneObject>();
        PickChild(m_Target.transform, list);

        float maxX, maxY, maxZ, minX, minY, minZ;
        maxX = maxY = maxZ = -Mathf.Infinity;
        minX = minY = minZ = Mathf.Infinity;
        if (list.Count > 0)
            m_Target.loadObjects = list;

        for (int i = 0; i < list.Count; i++)
        {
            maxX = Mathf.Max(list[i].Bounds.max.x, maxX);
            maxY = Mathf.Max(list[i].Bounds.max.y, maxY);
            maxZ = Mathf.Max(list[i].Bounds.max.z, maxZ);

            minX = Mathf.Min(list[i].Bounds.min.x, minX);
            minY = Mathf.Min(list[i].Bounds.min.y, minY);
            minZ = Mathf.Min(list[i].Bounds.min.z, minZ);
        }
        Vector3 size = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
        Vector3 center = new Vector3(minX + size.x/2, minY + size.y/2, minZ + size.z/2);
        m_Target.bounds = new Bounds(center, size);
    }

    private void PickChild(Transform transform, List<TestSceneObject> sceneObjectList)
    {
        var obj = PrefabUtility.GetPrefabParent(transform);
        if (obj != null)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            path = path.Replace("Assets/Resources/", "");
            string ext = Path.GetExtension(path);
            path = path.Replace(ext, "");
            var o = GetChildInfo(transform, path);
            if (o != null)
                sceneObjectList.Add(o);
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                PickChild(transform.GetChild(i), sceneObjectList);
            }
        }
    }

    private TestSceneObject GetChildInfo(Transform transform, string resPath)
    {
        if (string.IsNullOrEmpty(resPath))
            return null;
        Renderer[] renderers = transform.gameObject.GetComponentsInChildren<MeshRenderer>();
        if (renderers == null || renderers.Length == 0)
            return null;
        Vector3 min = renderers[0].bounds.min;
        Vector3 max = renderers[0].bounds.max;
        for (int i = 1; i < renderers.Length; i++)
        {
            min = Vector3.Min(renderers[i].bounds.min, min);
            max = Vector3.Max(renderers[i].bounds.max, max);
        }
        Vector3 size = max - min;
        Bounds bounds = new Bounds(min + size/2, size);
        if (size.x <= 0)
            size.x = 0.2f;
        if (size.y <= 0)
            size.y = 0.2f;
        if (size.z <= 0)
            size.z = 0.2f;
        bounds.size = size;
        
        
        TestSceneObject obj = new TestSceneObject(bounds, transform.position, transform.eulerAngles, transform.localScale, resPath);
        return obj;
        
    }
}