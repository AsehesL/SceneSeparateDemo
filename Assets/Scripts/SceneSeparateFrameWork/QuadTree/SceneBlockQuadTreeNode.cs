using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 场景区块四叉树节点
/// </summary>
public class SceneBlockQuadTreeNode<T> where T : ISceneBlockObject
{

    public Bounds Bounds
    {
        get { return m_Bounds; }
    }

    public int CurrentDepth
    {
        get { return m_CurrentDepth; }
    }

    public List<T> ObjectList
    {
        get { return m_ObjectList; }   
    }

    public bool HasTopLeftChild { get { return m_ChildNodes[0] != null; } }
    public bool HasTopRightChild { get { return m_ChildNodes[1] != null; } }
    public bool HasBottomLeftChild { get { return m_ChildNodes[2] != null; } }
    public bool HasBottomRightChild { get { return m_ChildNodes[3] != null; } }

    private SceneBlockQuadTreeNode<T>[] m_ChildNodes = new SceneBlockQuadTreeNode<T>[4] { null, null, null, null };
    
    private int m_CurrentDepth;
    
    private Bounds m_Bounds;

    private List<T> m_ObjectList;

    public SceneBlockQuadTreeNode(Bounds bounds)
    {
        m_Bounds = bounds;
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

    public SceneBlockQuadTreeNode<T> Insert(T obj, int depth, int maxDepth)
    {
        if (depth < maxDepth)
        {
            SceneBlockQuadTreeNode<T> node = GetContainerNode(obj);
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

    public void Trigger(Bounds bounds, TriggerHandle<T> handle)
    {
        if (handle == null)
            return;

        for (int i = 0; i < m_ChildNodes.Length; i++)
        {
            var node = m_ChildNodes[i];
            if (node != null)
                node.Trigger(bounds, handle);
        }

        if (m_Bounds.Intersects(bounds))
        {
            for (int i = 0; i < m_ObjectList.Count; i++)
            {
                if (m_ObjectList[i] != null)
                {
                    if (m_ObjectList[i].Bounds.Intersects(bounds))
                        handle(m_ObjectList[i]);
                }
            }
        }
    }

    private SceneBlockQuadTreeNode<T> GetContainerNode(T obj)
    {
        Vector3 halfSize = new Vector3(m_Bounds.size.x / 2, m_Bounds.size.y, m_Bounds.size.z / 2);
        SceneBlockQuadTreeNode<T> result = null;
        result = GetContainerNode(ref m_ChildNodes[0], m_Bounds.center + new Vector3(-halfSize.x / 2, 0, -halfSize.z / 2),
            halfSize, obj);
        if (result != null)
            return result;

        result = GetContainerNode(ref m_ChildNodes[1], m_Bounds.center + new Vector3(-halfSize.x / 2, 0, halfSize.z / 2),
            halfSize, obj);
        if (result != null)
            return result;

        result = GetContainerNode(ref m_ChildNodes[2], m_Bounds.center + new Vector3(halfSize.x / 2, 0, halfSize.z / 2),
            halfSize, obj);
        if (result != null)
            return result;

        result = GetContainerNode(ref m_ChildNodes[3], m_Bounds.center + new Vector3(halfSize.x / 2, 0, -halfSize.z / 2),
            halfSize, obj);
        if (result != null)
            return result;

        return null;
    }

    private SceneBlockQuadTreeNode<T> GetContainerNode(ref SceneBlockQuadTreeNode<T> node, Vector3 centerPos, Vector3 size, T obj)
    {
        SceneBlockQuadTreeNode<T> result = null;
        if (node == null)
        {
            Bounds bounds = new Bounds(centerPos, size);
            if (bounds.IsBoundsContainsAnotherBounds(obj.Bounds))
            {
                SceneBlockQuadTreeNode<T> newNode = new SceneBlockQuadTreeNode<T>(bounds);
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
}
