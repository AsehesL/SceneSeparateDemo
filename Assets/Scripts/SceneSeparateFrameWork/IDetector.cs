using UnityEngine;
using System.Collections;

/// <summary>
/// 触发器接口，用于检测和场景物件的触发
/// </summary>
public interface IDetector
{
    /// <summary>
    /// 是否触发包围盒
    /// </summary>
    /// <param name="bounds">包围盒</param>
    /// <returns></returns>
    bool IsTrigger(Bounds bounds);

    /// <summary>
    /// 触发器位置
    /// </summary>
    Vector3 Position { get; }
}
