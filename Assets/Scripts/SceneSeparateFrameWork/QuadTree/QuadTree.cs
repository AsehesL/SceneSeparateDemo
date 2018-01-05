using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public delegate void TriggerHandle<T>(T trigger);
/// <summary>
/// 场景区块四叉树
/// </summary>
public class QuadTree<T> where T : ISceneObject
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
    private QuadTreeNode<T> m_Root;
    
    public int m_MaxDepth;

    public QuadTree(Vector3 center, Vector3 size, int maxDepth)
    {
        this.m_MaxDepth = maxDepth;
        this.m_Root = new QuadTreeNode<T>(new Bounds(center, size), 0);
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


    public void Trigger(IDetector detector, TriggerHandle<T> handle)
    {
        if (handle == null)
            return;
        m_Root.Trigger(detector, handle);
    }

    public static implicit operator bool(QuadTree<T> tree)
    {
        return tree != null;
    }
}
