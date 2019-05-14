using UnityEngine;
using System.Collections;

/// <summary>
/// 检测器接口，用于检测和场景物件的触发
/// </summary>
public interface IDetector
{
    /// <summary>
    /// 是否检测成功
    /// </summary>
    /// <param name="bounds">包围盒</param>
    /// <returns></returns>
    bool IsDetected(Bounds bounds);

    /// <summary>
    /// 计算八叉树包围盒的碰撞域码
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="treeType"></param>
    /// <returns></returns>
    int DetectedCode(Bounds bounds, SceneSeparateTreeType treeType);

	int DetecedCode(float centerX, float centerY, float centerZ, float sizeX, float sizeY, float sizeZ, SceneSeparateTreeType treeType);

    /// <summary>
    /// 触发器位置
    /// </summary>
    Vector3 Position { get; }
}
