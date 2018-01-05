using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 场景区块四叉树节点
/// </summary>
public class QuadTreeNode<T> where T : ISceneObject
{
    /// <summary>
    /// 节点包围盒
    /// </summary>
    public Bounds Bounds
    {
        get { return m_Bounds; }
    }

    /// <summary>
    /// 节点当前深度
    /// </summary>
    public int CurrentDepth
    {
        get { return m_CurrentDepth; }
    }

    /// <summary>
    /// 节点数据列表
    /// </summary>
    public List<T> ObjectList
    {
        get { return m_ObjectList; }   
    }

    public bool HasTopLeftChild { get { return m_ChildNodes[0] != null; } }
    public bool HasTopRightChild { get { return m_ChildNodes[1] != null; } }
    public bool HasBottomLeftChild { get { return m_ChildNodes[2] != null; } }
    public bool HasBottomRightChild { get { return m_ChildNodes[3] != null; } }

    private QuadTreeNode<T>[] m_ChildNodes = new QuadTreeNode<T>[4] { null, null, null, null };
    
    private int m_CurrentDepth;
    
    private Bounds m_Bounds;

    private List<T> m_ObjectList;

    public QuadTreeNode(Bounds bounds, int depth)
    {
        m_Bounds = bounds;
        m_CurrentDepth = depth;
        m_ObjectList = new List<T>();
    }

    public void Clear()
    {
        for (int i = 0; i < m_ChildNodes.Length; i++)
        {
            if (m_ChildNodes[i] != null)
                m_ChildNodes[i].Clear();
        }
        if (m_ObjectList != null)
            m_ObjectList.Clear();
    }

    public bool Contains(T obj)
    {
        for (int i = 0; i < m_ChildNodes.Length; i++)
        {
            if (m_ChildNodes[i] != null && m_ChildNodes[i].Contains(obj))
                return true;
        }

        if (m_ObjectList != null && m_ObjectList.Contains(obj))
            return true;
        return false;
    }

    public QuadTreeNode<T> Insert(T obj, int depth, int maxDepth)
    {
        if (m_ObjectList.Contains(obj))
            return this;
        if (depth < maxDepth)
        {
            QuadTreeNode<T> node = GetContainerNode(obj, depth);
            if (node != null)
                return node.Insert(obj, depth + 1, maxDepth);
        }
        m_ObjectList.Add(obj);
        return this;
    }

    public bool Remove(T obj)
    {
        if (m_ObjectList.Remove(obj))
        {
            return true;
        }
        return false;
    }

    public void Trigger(IDetector detector, TriggerHandle<T> handle)
    {
        if (handle == null)
            return;

        for (int i = 0; i < m_ChildNodes.Length; i++)
        {
            var node = m_ChildNodes[i];
            if (node != null)
                node.Trigger(detector, handle);
        }

        if (detector.IsTrigger(m_Bounds))
        {
            for (int i = 0; i < m_ObjectList.Count; i++)
            {
                if (m_ObjectList[i] != null)
                {
                    if (detector.IsTrigger(m_ObjectList[i].Bounds))
                        handle(m_ObjectList[i]);
                }
            }
        }
    }

    private QuadTreeNode<T> GetContainerNode(T obj, int depth)
    {
        Vector3 halfSize = new Vector3(m_Bounds.size.x / 2, m_Bounds.size.y, m_Bounds.size.z / 2);
        QuadTreeNode<T> result = null;
        result = GetContainerNode(ref m_ChildNodes[0], depth, m_Bounds.center + new Vector3(-halfSize.x / 2, 0, -halfSize.z / 2),
            halfSize, obj);
        if (result != null)
            return result;

        result = GetContainerNode(ref m_ChildNodes[1], depth, m_Bounds.center + new Vector3(-halfSize.x / 2, 0, halfSize.z / 2),
            halfSize, obj);
        if (result != null)
            return result;

        result = GetContainerNode(ref m_ChildNodes[2], depth, m_Bounds.center + new Vector3(halfSize.x / 2, 0, halfSize.z / 2),
            halfSize, obj);
        if (result != null)
            return result;

        result = GetContainerNode(ref m_ChildNodes[3], depth, m_Bounds.center + new Vector3(halfSize.x / 2, 0, -halfSize.z / 2),
            halfSize, obj);
        if (result != null)
            return result;

        return null;
    }

    private QuadTreeNode<T> GetContainerNode(ref QuadTreeNode<T> node, int depth, Vector3 centerPos, Vector3 size, T obj)
    {
        QuadTreeNode<T> result = null;
        if (node == null)
        {
            Bounds bounds = new Bounds(centerPos, size);
            if (bounds.IsBoundsContainsAnotherBounds(obj.Bounds))
            {
                QuadTreeNode<T> newNode = new QuadTreeNode<T>(bounds, depth+1);
                node = newNode;
                result = node;
            }
        }
        else if (node.Bounds.IsBoundsContainsAnotherBounds(obj.Bounds))
        {
            result = node;
        }
        return result;
    }

#if UNITY_EDITOR
    public void DrawNode(float h, float deltaH)
    {
        if (m_ChildNodes != null)
        {
            for (int i = 0; i < m_ChildNodes.Length; i++)
            {
                var node = m_ChildNodes[i];
                if (node != null)
                    node.DrawNode(h + deltaH, deltaH);
            }
        }

        DrawArea(h, 1, 1);
    }

    public void DrawArea(float H, float S, float V)
    {
        Color col = Color.HSVToRGB(H, S, V);
        DrawArea(col);
    }

    public void DrawArea(Color color)
    {
        m_Bounds.DrawBounds(color);
        for (int i = 0; i < m_ObjectList.Count; i++)
        {
            if (m_ObjectList[i] != null && m_ObjectList[i] is SceneObject)
            {
                var scenobj = m_ObjectList[i] as SceneObject;
                scenobj.DrawArea(Color.black);
            }
        }
    }
#endif
}
