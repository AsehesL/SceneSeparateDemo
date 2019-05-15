using UnityEngine;
using System.Collections;

/// <summary>
/// 检测器接口，用于检测和场景物件的触发
/// </summary>
public interface IDetector
{
	/// <summary>
	/// 是否使用相机裁剪检测
	/// </summary>
	bool UseCameraCulling { get; }

    /// <summary>
    /// 是否检测成功
    /// </summary>
    /// <param name="bounds">包围盒</param>
    /// <returns></returns>
    bool IsDetected(Bounds bounds);

	/// <summary>
	/// 计算某坐标与检测器的碰撞掩码
	/// ps：如果UseCameraCulling为True，则计算的是坐标的裁剪掩码
	/// 否则按照检测器碰撞的象限返回code：
	/// 如果ignoreY为True，则对应四个象限的碰撞掩码：
	/// |2|8|
	/// |1|4|
	/// 如果ignoreY为False，则对应八个象限的碰撞掩码：
	/// 下层： |2|32|    上层：|8|128|  
	///        |1|16|          |4|64 |
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	/// <returns></returns>
	int GetDetectedCode(float x, float y, float z, bool ignoreY);

	/// <summary>
	/// 触发器位置
	/// </summary>
	Vector3 Position { get; }
}
