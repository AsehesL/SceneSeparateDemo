using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 场景树（非线性结构）
/// </summary>
/// <typeparam name="T"></typeparam>
public class SceneTree<T> : ISeparateTree<T> where T : ISceneObject, ISOLinkedListNode
{
	public Bounds Bounds
	{
		get
		{
			if (m_Root != null)
				return m_Root.Bounds;
			return default(Bounds);
		}
	}

	public int MaxDepth
	{
		get { return m_MaxDepth; }
	}

	/// <summary>
	/// 最大深度
	/// </summary>
	protected int m_MaxDepth;

	protected SceneTreeNode<T> m_Root;

	public SceneTree(Vector3 center, Vector3 size, int maxDepth, bool ocTree)
	{
		this.m_MaxDepth = maxDepth;
		this.m_Root = new SceneTreeNode<T>(new Bounds(center, size), 0, ocTree ? 8 : 4);
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

	public void Remove(T item)
	{
		m_Root.Remove(item);
	}
	public void Trigger(IDetector detector, TriggerHandle<T> handle)
	{
		if (handle == null)
			return;
		if (detector.IsDetected(Bounds) == false)
			return;
		m_Root.Trigger(detector, handle);
	}

	public static implicit operator bool(SceneTree<T> tree)
	{
		return tree != null;
	}

#if UNITY_EDITOR
	public void DrawTree(Color treeMinDepthColor, Color treeMaxDepthColor, Color objColor, Color hitObjColor, int drawMinDepth, int drawMaxDepth, bool drawObj)
	{
		if (m_Root != null)
			m_Root.DrawNode(treeMinDepthColor, treeMaxDepthColor, objColor, hitObjColor, drawMinDepth, drawMaxDepth, drawObj, m_MaxDepth);
	}
#endif
}

public class SceneTreeNode<T> where T : ISceneObject, ISOLinkedListNode
{
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
	public LinkedList<T> ObjectList
	{
		get { return m_ObjectList; }
	}

	private int m_CurrentDepth;

	private Vector3 m_HalfSize;

	private LinkedList<T> m_ObjectList;

	private SceneTreeNode<T>[] m_ChildNodes;

	private int m_ChildCount;

	private Bounds m_Bounds;

	public SceneTreeNode(Bounds bounds, int depth, int childCount)
	{
		m_Bounds = bounds;
		m_CurrentDepth = depth;
		m_ObjectList = new LinkedList<T>();
		m_ChildNodes = new SceneTreeNode<T>[childCount];

		if (childCount == 8)
			m_HalfSize = new Vector3(m_Bounds.size.x / 2, m_Bounds.size.y / 2, m_Bounds.size.z / 2);
		else
			m_HalfSize = new Vector3(m_Bounds.size.x / 2, m_Bounds.size.y, m_Bounds.size.z / 2);

		m_ChildCount = childCount;
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

	public SceneTreeNode<T> Insert(T obj, int depth, int maxDepth)
	{
		if (m_ObjectList.Contains(obj))
			return this;
		if (depth < maxDepth)
		{
			SceneTreeNode<T> node = GetContainerNode(obj, depth);
			if (node != null)
				return node.Insert(obj, depth + 1, maxDepth);
		}
		var n = m_ObjectList.AddFirst(obj);
		obj.SetLinkedListNode(0, n);
		return this;
	}

	public void Remove(T obj)
	{
		var node = obj.GetLinkedListNode<T>(0);
		if (node != null)
		{
			m_ObjectList.Remove(node);
			obj.GetNodes<T>().Clear();
		}

		//{
		//    return true;
		//}
		//return false;
	}

	public void Trigger(IDetector detector, TriggerHandle<T> handle)
	{
		if (handle == null)
			return;

		int code = detector.GetDetectedCode(m_Bounds.center.x, m_Bounds.center.y, m_Bounds.center.z, m_ChildCount == 4);
		for (int i = 0; i < m_ChildNodes.Length; i++)
		{
			var node = m_ChildNodes[i];
			if (node != null && (code & (1 << i)) != 0)
			{
				node.Trigger(detector, handle);
			}
		}

		{
			var node = m_ObjectList.First;
			while (node != null)
			{
				if (detector.IsDetected(node.Value.Bounds))
					handle(node.Value);
				node = node.Next;
			}
		}
	}

	protected SceneTreeNode<T> GetContainerNode(T obj, int depth)
	{
		SceneTreeNode<T> result = null;
		int ix = -1;
		int iz = -1;
		int iy = m_ChildNodes.Length == 4 ? 0 : -1;

		int nodeIndex = 0;

		for (int i = ix; i <= 1; i += 2)
		{
			for (int k = iy; k <= 1; k += 2)
			{
				for (int j = iz; j <= 1; j += 2)
				{
					result = CreateNode(ref m_ChildNodes[nodeIndex], depth,
						m_Bounds.center + new Vector3(i * m_HalfSize.x * 0.5f, k * m_HalfSize.y * 0.5f, j * m_HalfSize.z * 0.5f),
						m_HalfSize, obj);
					if (result != null)
					{
						return result;
					}

					nodeIndex += 1;
				}
			}
		}
		return null;
	}

	protected SceneTreeNode<T> CreateNode(ref SceneTreeNode<T> node, int depth, Vector3 centerPos, Vector3 size, T obj)
	{
		SceneTreeNode<T> result = null;

		if (node == null)
		{
			Bounds bounds = new Bounds(centerPos, size);
			if (bounds.IsBoundsContainsAnotherBounds(obj.Bounds))
			{
				SceneTreeNode<T> newNode = new SceneTreeNode<T>(bounds, depth + 1, m_ChildNodes.Length);
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
	public void DrawNode(Color treeMinDepthColor, Color treeMaxDepthColor, Color objColor, Color hitObjColor, int drawMinDepth, int drawMaxDepth, bool drawObj, int maxDepth)
	{
		if (m_ChildNodes != null)
		{
			for (int i = 0; i < m_ChildNodes.Length; i++)
			{
				var node = m_ChildNodes[i];
				if (node != null)
					node.DrawNode(treeMinDepthColor, treeMaxDepthColor, objColor, hitObjColor, drawMinDepth, drawMaxDepth, drawObj, maxDepth);
			}
		}

		if (m_CurrentDepth >= drawMinDepth && m_CurrentDepth <= drawMaxDepth)
		{
			float d = ((float)m_CurrentDepth) / maxDepth;
			Color color = Color.Lerp(treeMinDepthColor, treeMaxDepthColor, d);

			m_Bounds.DrawBounds(color);
		}
		if (drawObj)
		{
			var node = m_ObjectList.First;
			while (node != null)
			{
				var sceneobj = node.Value as SceneObject;
				if (sceneobj != null)
					sceneobj.DrawArea(objColor, hitObjColor);
				node = node.Next;
			}
		}

	}

#endif
}
