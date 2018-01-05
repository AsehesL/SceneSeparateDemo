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
    /// 四叉树包围盒
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

    /// <summary>
    /// 四叉树最大深度
    /// </summary>
    public int MaxDepth
    {
        get { return m_MaxDepth; }
    }


    /// <summary>
    /// 根节点
    /// </summary>
    private QuadTreeNode<T> m_Root;
    
    /// <summary>
    /// 最大深度
    /// </summary>
    public int m_MaxDepth;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="center">四叉树中心</param>
    /// <param name="size">四叉树区域大小</param>
    /// <param name="maxDepth">四叉树最大深度</param>
    public QuadTree(Vector3 center, Vector3 size, int maxDepth)
    {
        this.m_MaxDepth = maxDepth;
        this.m_Root = new QuadTreeNode<T>(new Bounds(center, size), 0);
    }

    /// <summary>
    /// 添加数据
    /// </summary>
    /// <param name="item"></param>
    public void Add(T item)
    {
        m_Root.Insert(item, 0, m_MaxDepth);
    }

    /// <summary>
    /// 清除
    /// </summary>
    public void Clear()
    {
        m_Root.Clear();
    }

    /// <summary>
    /// 是否包含
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item)
    {
        return m_Root.Contains(item);
    }

    /// <summary>
    /// 移除数据
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(T item)
    {
        return m_Root.Remove(item);
    }

    /// <summary>
    /// 触发判断
    /// </summary>
    /// <param name="detector">触发器</param>
    /// <param name="handle">触发处理回调</param>
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

#if UNITY_EDITOR
    public void DrawTree(float h, float deltaH)
    {
        if (m_Root != null)
            m_Root.DrawNode(h, deltaH);
    }
#endif
}
