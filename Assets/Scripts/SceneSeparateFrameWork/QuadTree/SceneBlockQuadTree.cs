using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public delegate void TriggerHandle<T>(T trigger);
/// <summary>
/// 场景区块四叉树
/// </summary>
public class SceneBlockQuadTree<T> where T : ISceneBlockObject
{
    /// <summary>
    /// 八叉树包围盒
    /// </summary>
    public Bounds Bounds
    {
        get
        {
            if (m_Root!=null)
                return m_Root.Bounds;
            return default(Bounds);
        }
    }

    public int MaxDepth
    {
        get { return m_MaxDepth; }
    }


    /// <summary>
    /// 根节点
    /// </summary>
    private SceneBlockQuadTreeNode<T> m_Root;
    
    public int m_MaxDepth;

    public void Build(Vector3 center, Vector3 size, int maxDepth)
    {
        this.m_MaxDepth = maxDepth;
        this.m_Root = new SceneBlockQuadTreeNode<T>(new Bounds(center, size));
    }

    public void Add(T item)
    {
        m_Root.Insert(item, 0, m_MaxDepth);
    }

    public void Clear()
    {
        m_Root.Clear();
    }

    public bool Contains(T item)
    {
        return m_Root.Contains(item);
    }

    public bool Remove(T item)
    {
        return m_Root.Remove(item);
    }


    public void Trigger(Bounds bounds, TriggerHandle<T> handle)
    {
        if (handle == null)
            return;
    }

    public static implicit operator bool(SceneBlockQuadTree<T> tree)
    {
        return tree != null;
    }
}
