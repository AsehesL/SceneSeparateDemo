using UnityEngine;
using System.Collections;

/// <summary>
/// 场景区块物体接口：需要插入到场景区块四叉树并实现动态显示与隐藏的物体实现该接口
/// </summary>
public interface ISceneBlockObject
{

    Bounds Bounds { get; }

    bool OnShow(Transform parent);

    void OnHide();
}
