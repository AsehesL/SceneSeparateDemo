using UnityEngine;
using System.Collections;

public interface ISeparateTree<T> where T : ISceneObject, ISOLinkedListNode
{
	Bounds Bounds { get; }

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
