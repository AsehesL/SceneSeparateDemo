using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 线性八叉树
/// 节点字典存放叶节点Morton作为Key
/// </summary>
public class LinearSceneOcTree<T> : LinearSceneTree<T> where T : ISceneObject, ISOLinkedListNode
{

	private float m_DeltaX;
	private float m_DeltaY;
	private float m_DeltaZ;

	public LinearSceneOcTree(Vector3 center, Vector3 size, int maxDepth) : base(center, size, maxDepth)
	{
		m_DeltaX = m_Bounds.size.x / m_Cols;
		m_DeltaY = m_Bounds.size.y / m_Cols;
		m_DeltaZ = m_Bounds.size.z / m_Cols;
	}

	public override void Add(T item)
	{
		if (item == null)
			return;
		if (m_Bounds.Intersects(item.Bounds))
		{
			if (m_MaxDepth == 0)
			{
				if (m_Nodes.ContainsKey(0) == false)
					m_Nodes[0] = new LinearSceneTreeLeaf<T>();
				var node = m_Nodes[0].Insert(item);
				item.SetLinkedListNode<T>(0, node);
			}
			else
			{
				InsertToNode(item, 0, m_Bounds.center.x, m_Bounds.center.y, m_Bounds.center.z, m_Bounds.size.x, m_Bounds.size.y, m_Bounds.size.z);
			}
		}
	}

	public override void Trigger(IDetector detector, TriggerHandle<T> handle)
	{
		if (handle == null)
			return;
		if (detector.UseCameraCulling)
		{
			//如果使用相机裁剪，则计算出八个角点的裁剪掩码，且子节点的裁剪检测可以复用部分父节点的角点
			TreeCullingCode code = new TreeCullingCode()
			{
				leftbottomback = detector.GetDetectedCode(m_Bounds.min.x, m_Bounds.min.y, m_Bounds.min.z, true),
				leftbottomforward = detector.GetDetectedCode(m_Bounds.min.x, m_Bounds.min.y, m_Bounds.max.z, true),
				lefttopback = detector.GetDetectedCode(m_Bounds.min.x, m_Bounds.max.y, m_Bounds.min.z, true),
				lefttopforward = detector.GetDetectedCode(m_Bounds.min.x, m_Bounds.max.y, m_Bounds.max.z, true),
				rightbottomback = detector.GetDetectedCode(m_Bounds.max.x, m_Bounds.min.y, m_Bounds.min.z, true),
				rightbottomforward = detector.GetDetectedCode(m_Bounds.max.x, m_Bounds.min.y, m_Bounds.max.z, true),
				righttopback = detector.GetDetectedCode(m_Bounds.max.x, m_Bounds.max.y, m_Bounds.min.z, true),
				righttopforward = detector.GetDetectedCode(m_Bounds.max.x, m_Bounds.max.y, m_Bounds.max.z, true),
			};
			TriggerToNodeByCamera(detector, handle, 0, code, m_Bounds.center.x, m_Bounds.center.y, m_Bounds.center.z, m_Bounds.size.x, m_Bounds.size.y,
				m_Bounds.size.z);
		}
		else
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
				TriggerToNode(detector, handle, 0, m_Bounds.center.x, m_Bounds.center.y, m_Bounds.center.z, m_Bounds.size.x, m_Bounds.size.y, m_Bounds.size.z);
			}
		}
	}

	private bool InsertToNode(T obj, int depth, float centerx, float centery, float centerz, float sizex, float sizey, float sizez)
	{
		if (depth == m_MaxDepth)
		{
			uint m = Morton3FromWorldPos(centerx, centery, centerz);
			if (m_Nodes.ContainsKey(m) == false)
				m_Nodes[m] = new LinearSceneTreeLeaf<T>();
			var node = m_Nodes[m].Insert(obj);
			obj.SetLinkedListNode<T>(m, node);
			return true;
		}
		else
		{
			int colider = 0;
			float minx = obj.Bounds.min.x;
			float miny = obj.Bounds.min.y;
			float minz = obj.Bounds.min.z;
			float maxx = obj.Bounds.max.x;
			float maxy = obj.Bounds.max.y;
			float maxz = obj.Bounds.max.z;

			if (minx <= centerx && miny <= centery && minz <= centerz)
				colider |= 1;
			if (minx <= centerx && miny <= centery && maxz >= centerz)
				colider |= 2;
			if (minx <= centerx && maxy >= centery && minz <= centerz)
				colider |= 4;
			if (minx <= centerx && maxy >= centery && maxz >= centerz)
				colider |= 8;
			if (maxx >= centerx && miny <= centery && minz <= centerz)
				colider |= 16;
			if (maxx >= centerx && miny <= centery && maxz >= centerz)
				colider |= 32;
			if (maxx >= centerx && maxy >= centery && minz <= centerz)
				colider |= 64;
			if (maxx >= centerx && maxy >= centery && maxz >= centerz)
				colider |= 128;
			float sx = sizex * 0.5f, sy = sizey * 0.5f, sz = sizez * 0.5f;

			bool insertresult = false;
			if ((colider & 1) != 0)
				insertresult = insertresult | InsertToNode(obj, depth + 1, centerx - sx * 0.5f, centery - sy * 0.5f, centerz - sz * 0.5f, sx, sy, sz);
			if ((colider & 2) != 0)
				insertresult = insertresult | InsertToNode(obj, depth + 1, centerx - sx * 0.5f, centery - sy * 0.5f, centerz + sz * 0.5f, sx, sy, sz);
			if ((colider & 4) != 0)
				insertresult = insertresult | InsertToNode(obj, depth + 1, centerx - sx * 0.5f, centery + sy * 0.5f, centerz - sz * 0.5f, sx, sy, sz);
			if ((colider & 8) != 0)
				insertresult = insertresult | InsertToNode(obj, depth + 1, centerx - sx * 0.5f, centery + sy * 0.5f, centerz + sz * 0.5f, sx, sy, sz);
			if ((colider & 16) != 0)
				insertresult = insertresult | InsertToNode(obj, depth + 1, centerx + sx * 0.5f, centery - sy * 0.5f, centerz - sz * 0.5f, sx, sy, sz);
			if ((colider & 32) != 0)
				insertresult = insertresult | InsertToNode(obj, depth + 1, centerx + sx * 0.5f, centery - sy * 0.5f, centerz + sz * 0.5f, sx, sy, sz);
			if ((colider & 64) != 0)
				insertresult = insertresult | InsertToNode(obj, depth + 1, centerx + sx * 0.5f, centery + sy * 0.5f, centerz - sz * 0.5f, sx, sy, sz);
			if ((colider & 128) != 0)
				insertresult = insertresult | InsertToNode(obj, depth + 1, centerx + sx * 0.5f, centery + sy * 0.5f, centerz + sz * 0.5f, sx, sy, sz);
			return insertresult;
		}
	}

	private void TriggerToNodeByCamera(IDetector detector, TriggerHandle<T> handle, int depth, TreeCullingCode cullingCode, float centerx, float centery, float centerz, float sizex, float sizey,
		float sizez)
	{
		if (cullingCode.IsCulled())
			return;
		if (depth == m_MaxDepth)
		{
			uint m = Morton3FromWorldPos(centerx, centery, centerz);
			if (m_Nodes.ContainsKey(m) && m_Nodes[m] != null)
			{
				m_Nodes[m].Trigger(detector, handle);
			}
		}
		else
		{
			float sx = sizex * 0.5f, sy = sizey * 0.5f, sz = sizez * 0.5f;
			int leftbottommiddle = detector.GetDetectedCode(centerx - sx, centery - sy, centerz, true);
			int middlebottommiddle = detector.GetDetectedCode(centerx, centery - sy, centerz, true);
			int rightbottommiddle = detector.GetDetectedCode(centerx + sx, centery - sy, centerz, true);
			int middlebottomback = detector.GetDetectedCode(centerx, centery - sy, centerz - sz, true);
			int middlebottomforward = detector.GetDetectedCode(centerx, centery - sy, centerz + sz, true);

			int leftmiddleback = detector.GetDetectedCode(centerx - sx, centery, centerz - sz, true);
			int leftmiddlemiddle = detector.GetDetectedCode(centerx - sx, centery, centerz, true);
			int leftmiddleforward = detector.GetDetectedCode(centerx - sx, centery, centerz + sz, true);
			int middlemiddleback = detector.GetDetectedCode(centerx, centery, centerz - sz, true);
			int middlemiddlemiddle = detector.GetDetectedCode(centerx, centery, centerz, true);
			int middlemiddleforward = detector.GetDetectedCode(centerx, centery, centerz + sz, true);
			int rightmiddleback = detector.GetDetectedCode(centerx + sx, centery, centerz - sz, true);
			int rightmiddlemiddle = detector.GetDetectedCode(centerx + sx, centery, centerz, true);
			int rightmiddleforward = detector.GetDetectedCode(centerx + sx, centery, centerz + sz, true);

			int lefttopmiddle = detector.GetDetectedCode(centerx - sx, centery + sy, centerz, true);
			int middletopmiddle = detector.GetDetectedCode(centerx, centery + sy, centerz, true);
			int righttopmiddle = detector.GetDetectedCode(centerx + sx, centery + sy, centerz, true);
			int middletopback = detector.GetDetectedCode(centerx, centery + sy, centerz - sz, true);
			int middletopforward = detector.GetDetectedCode(centerx, centery + sy, centerz + sz, true);

			TriggerToNodeByCamera(detector, handle, depth + 1, new TreeCullingCode()
			{
				leftbottomback = cullingCode.leftbottomback,
				leftbottomforward = leftbottommiddle,
				lefttopback = leftmiddleback,
				lefttopforward = leftmiddlemiddle,
				rightbottomback = middlebottomback,
				rightbottomforward = middlebottommiddle,
				righttopback = middlemiddleback,
				righttopforward = middlemiddlemiddle,
			}, centerx - sx * 0.5f, centery - sy * 0.5f, centerz - sz * 0.5f, sx, sy, sz);
			TriggerToNodeByCamera(detector, handle, depth + 1, new TreeCullingCode()
				{
					leftbottomback = leftbottommiddle,
					leftbottomforward = cullingCode.leftbottomforward,
					lefttopback = leftmiddlemiddle,
					lefttopforward = leftmiddleforward,
					rightbottomback = middlebottommiddle,
					rightbottomforward = middlebottomforward,
					righttopback = middlemiddlemiddle,
					righttopforward = middlemiddleforward,
				}, centerx - sx * 0.5f, centery - sy * 0.5f, centerz + sz * 0.5f, sx, sy, sz);
			TriggerToNodeByCamera(detector, handle, depth + 1, new TreeCullingCode()
				{
					leftbottomback = leftmiddleback,
					leftbottomforward = leftmiddlemiddle,
					lefttopback = cullingCode.lefttopback,
					lefttopforward = lefttopmiddle,
					rightbottomback = middlemiddleback,
					rightbottomforward = middlemiddlemiddle,
					righttopback = middletopback,
					righttopforward = middletopmiddle,
				}, centerx - sx * 0.5f, centery + sy * 0.5f, centerz - sz * 0.5f, sx, sy, sz);
			TriggerToNodeByCamera(detector, handle, depth + 1, new TreeCullingCode()
				{
					leftbottomback = leftmiddlemiddle,
					leftbottomforward = leftmiddleforward,
					lefttopback = lefttopmiddle,
					lefttopforward = cullingCode.lefttopforward,
					rightbottomback = middlemiddlemiddle,
					rightbottomforward = middlemiddleforward,
					righttopback = middletopmiddle,
					righttopforward = middletopforward,
				}, centerx - sx * 0.5f, centery + sy * 0.5f, centerz + sz * 0.5f, sx, sy, sz);

			TriggerToNodeByCamera(detector, handle, depth + 1, new TreeCullingCode()
			{
				leftbottomback = middlebottomback,
				leftbottomforward = middlebottommiddle,
				lefttopback = middlemiddleback,
				lefttopforward = middlemiddlemiddle,
				rightbottomback = cullingCode.rightbottomback,
				rightbottomforward = rightbottommiddle,
				righttopback = rightmiddleback,
				righttopforward = rightmiddlemiddle,
			}, centerx + sx * 0.5f, centery - sy * 0.5f, centerz - sz * 0.5f, sx, sy, sz);
			TriggerToNodeByCamera(detector, handle, depth + 1, new TreeCullingCode()
			{
				leftbottomback = middlebottommiddle,
				leftbottomforward = middlebottomforward,
				lefttopback = middlemiddlemiddle,
				lefttopforward = middlemiddleforward,
				rightbottomback = rightbottommiddle,
				rightbottomforward = cullingCode.rightbottomforward,
				righttopback = rightmiddlemiddle,
				righttopforward = rightmiddleforward,
			}, centerx + sx * 0.5f, centery - sy * 0.5f, centerz + sz * 0.5f, sx, sy, sz);
			TriggerToNodeByCamera(detector, handle, depth + 1, new TreeCullingCode()
			{
				leftbottomback = middlemiddleback,
				leftbottomforward = middlemiddlemiddle,
				lefttopback = middletopback,
				lefttopforward = middletopmiddle,
				rightbottomback = rightmiddleback,
				rightbottomforward = rightmiddlemiddle,
				righttopback = cullingCode.righttopback,
				righttopforward = righttopmiddle,
			}, centerx + sx * 0.5f, centery + sy * 0.5f, centerz - sz * 0.5f, sx, sy, sz);
			TriggerToNodeByCamera(detector, handle, depth + 1, new TreeCullingCode()
			{
				leftbottomback = middlemiddlemiddle,
				leftbottomforward = middlemiddleforward,
				lefttopback = middletopmiddle,
				lefttopforward = middletopforward,
				rightbottomback = rightmiddlemiddle,
				rightbottomforward = rightmiddleforward,
				righttopback = righttopmiddle,
				righttopforward = cullingCode.righttopforward,
			}, centerx + sx * 0.5f, centery + sy * 0.5f, centerz + sz * 0.5f, sx, sy, sz);
		}
	}

	private void TriggerToNode(IDetector detector, TriggerHandle<T> handle, int depth, float centerx, float centery, float centerz, float sizex, float sizey,
		float sizez)
	{
		if (depth == m_MaxDepth)
		{
			uint m = Morton3FromWorldPos(centerx, centery, centerz);
			if (m_Nodes.ContainsKey(m) && m_Nodes[m] != null)
			{
				m_Nodes[m].Trigger(detector, handle);
			}
		}
		else
		{

			int colider = detector.GetDetectedCode(centerx, centery, centerz, false);

			float sx = sizex * 0.5f, sy = sizey * 0.5f, sz = sizez * 0.5f;

			if ((colider & 1) != 0)
				TriggerToNode(detector, handle, depth + 1, centerx - sx * 0.5f, centery - sy * 0.5f, centerz - sz * 0.5f, sx, sy, sz);
			if ((colider & 2) != 0)
				TriggerToNode(detector, handle, depth + 1, centerx - sx * 0.5f, centery - sy * 0.5f, centerz + sz * 0.5f, sx, sy, sz);
			if ((colider & 4) != 0)
				TriggerToNode(detector, handle, depth + 1, centerx - sx * 0.5f, centery + sy * 0.5f, centerz - sz * 0.5f, sx, sy, sz);
			if ((colider & 8) != 0)
				TriggerToNode(detector, handle, depth + 1, centerx - sx * 0.5f, centery + sy * 0.5f, centerz + sz * 0.5f, sx, sy, sz);

			if ((colider & 16) != 0)
				TriggerToNode(detector, handle, depth + 1, centerx + sx * 0.5f, centery - sy * 0.5f, centerz - sz * 0.5f, sx, sy, sz);
			if ((colider & 32) != 0)
				TriggerToNode(detector, handle, depth + 1, centerx + sx * 0.5f, centery - sy * 0.5f, centerz + sz * 0.5f, sx, sy, sz);
			if ((colider & 64) != 0)
				TriggerToNode(detector, handle, depth + 1, centerx + sx * 0.5f, centery + sy * 0.5f, centerz - sz * 0.5f, sx, sy, sz);
			if ((colider & 128) != 0)
				TriggerToNode(detector, handle, depth + 1, centerx + sx * 0.5f, centery + sy * 0.5f, centerz + sz * 0.5f, sx, sy, sz);
		}
	}

	private uint Morton3FromWorldPos(float x, float y, float z)
	{
		uint px = (uint)Mathf.FloorToInt((x - m_Bounds.min.x) / m_DeltaX);
		uint py = (uint)Mathf.FloorToInt((y - m_Bounds.min.y) / m_DeltaY);
		uint pz = (uint)Mathf.FloorToInt((z - m_Bounds.min.z) / m_DeltaZ);
		return Morton3(px, py, pz);
	}

	private uint Morton3(uint x, uint y, uint z)
	{
		return (Part1By2(z) << 2) + (Part1By2(y) << 1) + Part1By2(x);
	}

	private uint Part1By2(uint n)
	{
		n = (n ^ (n << 16)) & 0xff0000ff;
		n = (n ^ (n << 8))  & 0x0300f00f;
		n = (n ^ (n << 4))  & 0x030c30c3;
		n = (n ^ (n << 2))  & 0x09249249;
		return n;
	}

	public static implicit operator bool(LinearSceneOcTree<T> tree)
	{
		return tree != null;
	}

#if UNITY_EDITOR
	public override void DrawTree(Color treeMinDepthColor, Color treeMaxDepthColor, Color objColor, Color hitObjColor, int drawMinDepth, int drawMaxDepth, bool drawObj)
	{
		DrawNodeGizmos(treeMinDepthColor, treeMaxDepthColor, objColor, hitObjColor, drawMinDepth, drawMaxDepth, drawObj,
			0, m_Bounds.center, m_Bounds.size);
	}

	private bool DrawNodeGizmos(Color treeMinDepthColor, Color treeMaxDepthColor, Color objColor, Color hitObjColor, int drawMinDepth, int drawMaxDepth, bool drawObj, int depth, Vector3 center, Vector3 size)
	{
		if (depth < drawMinDepth || depth > drawMaxDepth)
			return false;
		float d = ((float)depth) / m_MaxDepth;
		Color color = Color.Lerp(treeMinDepthColor, treeMaxDepthColor, d);
		if (depth == m_MaxDepth)
		{
			uint m = Morton3FromWorldPos(center.x, center.y, center.z);
			if (m_Nodes.ContainsKey(m) && m_Nodes[m] != null)
			{
				if (m_Nodes[m].DrawNode(objColor, hitObjColor, drawObj))
				{
					Bounds b = new Bounds(new Vector3(center.x, center.y, center.z),
						new Vector3(size.x, size.y, size.z));
					b.DrawBounds(color);
					return true;
				}
			}
		}
		else
		{
			bool draw = false;
			float sx = size.x * 0.5f, sy = size.y * 0.5f, sz = size.z * 0.5f;
			draw = draw | DrawNodeGizmos(treeMinDepthColor, treeMaxDepthColor, objColor, hitObjColor, drawMinDepth,
				       drawMaxDepth, drawObj, depth + 1,
				       new Vector3(center.x - sx * 0.5f, center.y - sy * 0.5f, center.z - sz * 0.5f),
				       new Vector3(sx, sy, sz));
			draw = draw | DrawNodeGizmos(treeMinDepthColor, treeMaxDepthColor, objColor, hitObjColor, drawMinDepth,
				       drawMaxDepth, drawObj, depth + 1,
				       new Vector3(center.x - sx * 0.5f, center.y - sy * 0.5f, center.z + sz * 0.5f),
				       new Vector3(sx, sy, sz));
			draw = draw | DrawNodeGizmos(treeMinDepthColor, treeMaxDepthColor, objColor, hitObjColor, drawMinDepth,
				       drawMaxDepth, drawObj, depth + 1,
				       new Vector3(center.x - sx * 0.5f, center.y + sy * 0.5f, center.z - sz * 0.5f),
				       new Vector3(sx, sy, sz));
			draw = draw | DrawNodeGizmos(treeMinDepthColor, treeMaxDepthColor, objColor, hitObjColor, drawMinDepth,
				       drawMaxDepth, drawObj, depth + 1,
				       new Vector3(center.x - sx * 0.5f, center.y + sy * 0.5f, center.z + sz * 0.5f),
				       new Vector3(sx, sy, sz));
			draw = draw | DrawNodeGizmos(treeMinDepthColor, treeMaxDepthColor, objColor, hitObjColor, drawMinDepth,
				       drawMaxDepth, drawObj, depth + 1,
				       new Vector3(center.x + sx * 0.5f, center.y - sy * 0.5f, center.z - sz * 0.5f),
				       new Vector3(sx, sy, sz));
			draw = draw | DrawNodeGizmos(treeMinDepthColor, treeMaxDepthColor, objColor, hitObjColor, drawMinDepth,
				       drawMaxDepth, drawObj, depth + 1,
				       new Vector3(center.x + sx * 0.5f, center.y - sy * 0.5f, center.z + sz * 0.5f),
				       new Vector3(sx, sy, sz));
			draw = draw | DrawNodeGizmos(treeMinDepthColor, treeMaxDepthColor, objColor, hitObjColor, drawMinDepth,
				       drawMaxDepth, drawObj, depth + 1,
				       new Vector3(center.x + sx * 0.5f, center.y + sy * 0.5f, center.z - sz * 0.5f),
				       new Vector3(sx, sy, sz));
			draw = draw | DrawNodeGizmos(treeMinDepthColor, treeMaxDepthColor, objColor, hitObjColor, drawMinDepth,
				       drawMaxDepth, drawObj, depth + 1,
				       new Vector3(center.x + sx * 0.5f, center.y + sy * 0.5f, center.z + sz * 0.5f),
				       new Vector3(sx, sy, sz));

			if (draw)
			{
				Bounds b = new Bounds(new Vector3(center.x, center.y, center.z),
					new Vector3(size.x, size.y, size.z));
				b.DrawBounds(color);
			}

			return draw;
		}

		return false;
	}
#endif
}
