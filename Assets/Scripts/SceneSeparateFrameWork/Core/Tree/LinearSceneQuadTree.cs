using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearSceneQuadTree<T> : ISeparateTree<T> where T : ISceneObject, ISOLinkedListNode
{
	public Bounds Bounds
	{
		get { return m_Bounds; }
	}

	public int MaxDepth
	{
		get { return m_MaxDepth; }
	}

	private int m_MaxDepth;

	private Bounds m_Bounds;

	//private LinearSceneQuadTreeLeaf<T>[] m_Nodes;
	private Dictionary<int, LinearSceneQuadTreeLeaf<T>> m_Nodes;//使用Morton码索引的节点字典

	private int m_Cols;
	private float m_DeltaWidth;
	private float m_DeltaHeight;

	public LinearSceneQuadTree(Vector3 center, Vector3 size, int maxDepth)
	{
		this.m_MaxDepth = maxDepth;
		m_Bounds = new Bounds(center, size);

		m_Cols = (int)Mathf.Pow(2, maxDepth);
		m_DeltaWidth = m_Bounds.size.x / m_Cols;
		m_DeltaHeight = m_Bounds.size.z / m_Cols;

		//m_Nodes = new LinearSceneQuadTreeLeaf<T>[m_Cols * m_Cols];
		m_Nodes = new Dictionary<int, LinearSceneQuadTreeLeaf<T>>();
	}

	public void Add(T item)
	{
		if (item == null)
			return;
		if (m_Bounds.Intersects(item.Bounds))
		{
			if (m_MaxDepth == 0)
			{
				if (m_Nodes.ContainsKey(0) == false)
					m_Nodes[0] = new LinearSceneQuadTreeLeaf<T>();
				//if (m_Nodes[0] == null)
				//	m_Nodes[0] = new LinearSceneQuadTreeLeaf<T>();
				m_Nodes[0].Insert(item);
			}
			else
			{
				InsertToNode(item, 0, m_Bounds.center.x, m_Bounds.center.z, m_Bounds.size.x, m_Bounds.size.z);
			}
		}
	}

	public void Clear()
	{
		m_Nodes.Clear();
	}

	public bool Contains(T item)
	{
		return false;
	}

	public void Remove(T item)
	{
	}

	public void Trigger(IDetector detector, TriggerHandle<T> handle)
	{
		if (handle == null)
			return;
		if (detector.IsDetected(m_Bounds))
		{
			if (m_MaxDepth == 0)
			{
				if (m_Nodes.ContainsKey(0) && m_Nodes[0] != null)
				{
					m_Nodes[0].Trigger(detector, handle);
				}
			}
			else
			{
				TriggerToNode(detector, handle, 0, m_Bounds.center.x, m_Bounds.center.z, m_Bounds.size.x, m_Bounds.size.z);
			}
		}
	}

	private bool InsertToNode(T obj, int depth, float centerx, float centerz, float sizex, float sizez)
	{
		if (depth == m_MaxDepth)
		{
			int x = Mathf.FloorToInt((centerx - m_Bounds.min.x) / m_DeltaWidth);
			int z = Mathf.FloorToInt((centerz - m_Bounds.min.z) / m_DeltaHeight);
			int m = Morton2(x, z);
			if (m_Nodes.ContainsKey(m) == false)
				m_Nodes[m] = new LinearSceneQuadTreeLeaf<T>();
			//if (m_Nodes[m] == null)
			//	m_Nodes[m] = new LinearSceneQuadTreeLeaf<T>();
			m_Nodes[m].Insert(obj);
			return true;
		}
		else
		{
			int colider = 0;
			float minx = obj.Bounds.min.x;
			float minz = obj.Bounds.min.z;
			float maxx = obj.Bounds.max.x;
			float maxz = obj.Bounds.max.z;

			if (minx <= centerx && minz <= centerz)
				colider |= 1;
			if (maxx >= centerx && minz <= centerz)
				colider |= 2;
			if (minx <= centerx && maxz >= centerz)
				colider |= 4;
			if (maxx >= centerx && maxz >= centerz)
				colider |= 8;
			float sx = sizex * 0.5f, sz = sizez * 0.5f;

			bool insertresult = false;
			if ((colider & 1) != 0)
				insertresult = insertresult | InsertToNode(obj, depth + 1, centerx - sx * 0.5f, centerz - sz * 0.5f, sx, sz);
			if ((colider & 2) != 0)
				insertresult = insertresult | InsertToNode(obj, depth + 1, centerx + sx * 0.5f, centerz - sz * 0.5f, sx, sz);
			if ((colider & 4) != 0)
				insertresult = insertresult | InsertToNode(obj, depth + 1, centerx - sx * 0.5f, centerz + sz * 0.5f, sx, sz);
			if ((colider & 8) != 0)
				insertresult = insertresult | InsertToNode(obj, depth + 1, centerx + sx * 0.5f, centerz + sz * 0.5f, sx, sz);
			return insertresult;
		}
	}

	private void TriggerToNode(IDetector detector, TriggerHandle<T> handle, int depth, float centerx, float centerz, float sizex,
		float sizez)
	{
		if (depth == m_MaxDepth)
		{
			int x = Mathf.FloorToInt((centerx - m_Bounds.min.x) / m_DeltaWidth);
			int z = Mathf.FloorToInt((centerz - m_Bounds.min.z) / m_DeltaHeight);
			int m = Morton2(x, z);
			if (m_Nodes.ContainsKey(m) && m_Nodes[m] != null)
			{
				m_Nodes[m].Trigger(detector, handle);
			}
		}
		else
		{
			int colider = detector.DetecedCode(centerx, m_Bounds.center.y, centerz, sizex, m_Bounds.size.y, sizez, SceneSeparateTreeType.QuadTree);
			//float minx = bounds.min.x;
			//float minz = bounds.min.z;
			//float maxx = bounds.max.x;
			//float maxz = bounds.max.z;

			//if (minx <= centerx && minz <= centerz)
			//	colider |= 1;
			//if (maxx >= centerx && minz <= centerz)
			//	colider |= 2;
			//if (minx <= centerx && maxz >= centerz)
			//	colider |= 4;
			//if (maxx >= centerx && maxz >= centerz)
			//	colider |= 8;
			float sx = sizex * 0.5f, sz = sizez * 0.5f;

			if ((colider & 1) != 0)
				TriggerToNode(detector, handle, depth + 1, centerx - sx * 0.5f, centerz - sz * 0.5f, sx, sz);
			if ((colider & 2) != 0)
				TriggerToNode(detector, handle, depth + 1, centerx + sx * 0.5f, centerz - sz * 0.5f, sx, sz);
			if ((colider & 4) != 0)
				TriggerToNode(detector, handle, depth + 1, centerx - sx * 0.5f, centerz + sz * 0.5f, sx, sz);
			if ((colider & 8) != 0)
				TriggerToNode(detector, handle, depth + 1, centerx + sx * 0.5f, centerz + sz * 0.5f, sx, sz);
		}
	}

	private int Morton2(int x, int y)
	{
		return (Part1By1(y) << 1) + Part1By1(x);
	}

	private int Part1By1(int n)
	{
		n = (n ^ (n << 8)) & 0x00ff00ff;
		n = (n ^ (n << 4)) & 0x0f0f0f0f;
		n = (n ^ (n << 2)) & 0x33333333;
		n = (n ^ (n << 1)) & 0x55555555;
		return n;
	}

	public static implicit operator bool(LinearSceneQuadTree<T> tree)
	{
		return tree != null;
	}

#if UNITY_EDITOR
	public void DrawTree(Color treeMinDepthColor, Color treeMaxDepthColor, Color objColor, Color hitObjColor, int drawMinDepth, int drawMaxDepth, bool drawObj)
	{
	}
#endif

}

public class LinearSceneQuadTreeLeaf<T> where T : ISceneObject, ISOLinkedListNode
{
	public LinkedList<T> Datas
	{
		get { return m_DataList; }
	}

	private LinkedList<T> m_DataList;

	public LinearSceneQuadTreeLeaf()
	{
		m_DataList = new LinkedList<T>();
	}

	public void Insert(T obj)
	{
		m_DataList.AddFirst(obj);
	}

	public void Trigger(IDetector detector, TriggerHandle<T> handle)
	{
		if (handle != null)
		{
			LinkedListNode<T> node = m_DataList.First;

			while (node != null)
			{
				if (detector.IsDetected(node.Value.Bounds))
					handle(node.Value);

				node = node.Next;
			}
		}
	}

#if UNITY_EDITOR
	public bool DrawNode(Color objColor, Color hitObjColor, bool drawObj)
	{
		if (drawObj && m_DataList.Count > 0)
		{
			LinkedListNode<T> node = m_DataList.First;

			while (node != null)
			{
				var sceneobj = node.Value as SceneObject;
				if (sceneobj != null)
					sceneobj.DrawArea(objColor, hitObjColor);

				node = node.Next;
			}
			return true;
		}

		return false;
	}

#endif
}