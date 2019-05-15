using UnityEngine;
using System.Collections;

public delegate void TriggerHandle<T>(T trigger);

/// <summary>
/// 场景树接口
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISeparateTree<T> where T : ISceneObject, ISOLinkedListNode
{
	/// <summary>
	/// 树的根节点包围盒
	/// </summary>
	Bounds Bounds { get; }

	/// <summary>
	/// 树的最大深度
	/// </summary>
	int MaxDepth { get; }

	void Add(T item);

	void Clear();

	bool Contains(T item);

	void Remove(T item);

	void Trigger(IDetector detector, TriggerHandle<T> handle);

#if UNITY_EDITOR
	void DrawTree(Color treeMinDepthColor, Color treeMaxDepthColor, Color objColor, Color hitObjColor, int drawMinDepth,
		int drawMaxDepth, bool drawObj);
#endif
}
