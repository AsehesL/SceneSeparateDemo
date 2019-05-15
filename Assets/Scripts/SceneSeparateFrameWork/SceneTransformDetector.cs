using UnityEngine;
using System.Collections;
using UnityEditorInternal.VersionControl;

/// <summary>
/// 该触发器根据Transform的包围盒区域触发
/// </summary>
public class SceneTransformDetector : SceneDetectorBase
{
    public Vector3 detectorSize;

    protected Bounds m_Bounds;

	public override bool UseCameraCulling
	{
		get { return false; }
	}

	protected virtual void RefreshBounds()
    {
        m_Bounds.center = Position;
        m_Bounds.size = detectorSize;
    }

    public override bool IsDetected(Bounds bounds)
    {
        RefreshBounds();
        return bounds.Intersects(m_Bounds);
    }

	public override int GetDetectedCode(float x, float y, float z, bool ignoreY)
	{
		RefreshBounds();
		int code = 0;
		if (ignoreY)
		{
			float minx = m_Bounds.min.x;
			float minz = m_Bounds.min.z;
			float maxx = m_Bounds.max.x;
			float maxz = m_Bounds.max.z;
			if (minx <= x && minz <= z)
				code |= 1;
			if (minx <= x && maxz >= z)
				code |= 2;
			if (maxx >= x && minz <= z)
				code |= 4;
			if (maxx >= x && maxz >= z)
				code |= 8;
		}
		else
		{
			float minx = m_Bounds.min.x;
			float miny = m_Bounds.min.y;
			float minz = m_Bounds.min.z;
			float maxx = m_Bounds.max.x;
			float maxy = m_Bounds.max.y;
			float maxz = m_Bounds.max.z;
			if (minx <= x && miny <= y && minz <= z)
				code |= 1;
			if (minx <= x && miny <= y && maxz >= z)
				code |= 2;
			if (minx <= x && maxy >= y && minz <= z)
				code |= 4;
			if (minx <= x && maxy >= y && maxz >= z)
				code |= 8;
			if (maxx >= x && miny <= y && minz <= z)
				code |= 16;
			if (maxx >= x && miny <= y && maxz >= z)
				code |= 32;
			if (maxx >= x && maxy >= y && minz <= z)
				code |= 64;
			if (maxx >= x && maxy >= y && maxz >= z)
				code |= 128;
		}
		return code;
	}

	//public override int DetecedCode2D(float centerX, float centerY, float centerZ, float sizeX, float sizeY, float sizeZ)
	//{
	//    RefreshBounds();
	//    int code = 0;
	//    float minx = m_Bounds.min.x;
	//    float minz = m_Bounds.min.z;
	//    float maxx = m_Bounds.max.x;
	//    float maxz = m_Bounds.max.z;
	//    if (minx <= centerX && minz <= centerZ)
	//        code |= 1;
	//    if (maxx >= centerX && minz <= centerZ)
	//        code |= 2;
	//    if (minx <= centerX && maxz >= centerZ)
	//        code |= 4;
	//    if (maxx >= centerX && maxz >= centerZ)
	//        code |= 8;
	//    return code;
	//}

	//public override int DetecedCode3D(float centerX, float centerY, float centerZ, float sizeX, float sizeY, float sizeZ)
	//{
	//    RefreshBounds();
	//    int code = 0;
	//    float minx = m_Bounds.min.x;
	//    float miny = m_Bounds.min.y;
	//    float minz = m_Bounds.min.z;
	//    float maxx = m_Bounds.max.x;
	//    float maxy = m_Bounds.max.y;
	//    float maxz = m_Bounds.max.z;
	//    if (minx <= centerX && miny <= centerY && minz <= centerZ)
	//        code |= 1;
	//    if (maxx >= centerX && miny <= centerY && minz <= centerZ)
	//        code |= 2;
	//    if (minx <= centerX && miny <= centerY && maxz >= centerZ)
	//        code |= 4;
	//    if (maxx >= centerX && miny <= centerY && maxz >= centerZ)
	//        code |= 8;
	//    if (minx <= centerX && maxy >= centerY && minz <= centerZ)
	//        code |= 16;
	//    if (maxx >= centerX && maxy >= centerY && minz <= centerZ)
	//        code |= 32;
	//    if (minx <= centerX && maxy >= centerY && maxz >= centerZ)
	//        code |= 64;
	//    if (maxx >= centerX && maxy >= centerY && maxz >= centerZ)
	//        code |= 128;
	//    return code;
	//}

	//   public override int DetectedCode(Bounds bounds, SceneSeparateTreeType treeType)
	//   {
	//       RefreshBounds();
	//       int code = treeType == SceneSeparateTreeType.OcTree ? CalculateOcTreeBoundsCode(m_Bounds, bounds) : CalculateQuadBoundsCode(m_Bounds, bounds);
	//       return code;
	//   }

	//public override int DetecedCode(float centerX, float centerY, float centerZ, float sizeX, float sizeY, float sizeZ, SceneSeparateTreeType treeType)
	//{
	//	RefreshBounds();
	//	int code = 0;
	//	if (treeType == SceneSeparateTreeType.QuadTree)
	//	{
	//		float minx = m_Bounds.min.x;
	//		float minz = m_Bounds.min.z;
	//		float maxx = m_Bounds.max.x;
	//		float maxz = m_Bounds.max.z;
	//		if (minx <= centerX && minz <= centerZ)
	//			code |= 1;
	//		if (maxx >= centerX && minz <= centerZ)
	//			code |= 2;
	//		if (minx <= centerX && maxz >= centerZ)
	//			code |= 4;
	//		if (maxx >= centerX && maxz >= centerZ)
	//			code |= 8;
	//	}
	//	else
	//	{

	//	}

	//	return code;
	//}

	//protected static int CalculateOcTreeBoundsCode(Bounds detectBounds, Bounds targetBounds)
	//   {
	//       if (detectBounds.max.x < targetBounds.min.x || detectBounds.min.x > targetBounds.max.x)
	//           return 0;
	//       if (detectBounds.max.y < targetBounds.min.y || detectBounds.min.y > targetBounds.max.y)
	//           return 0;
	//       if (detectBounds.max.z < targetBounds.min.z || detectBounds.min.z > targetBounds.max.z)
	//           return 0;
	//       int code = 255;
	//       if (detectBounds.max.x < targetBounds.center.x)
	//       {
	//           code = CodeMask(code, 16);
	//           code = CodeMask(code, 32);
	//           code = CodeMask(code, 64);
	//           code = CodeMask(code, 128);
	//       }
	//       if (detectBounds.min.x > targetBounds.center.x)
	//       {
	//           code = CodeMask(code, 1);
	//           code = CodeMask(code, 4);
	//           code = CodeMask(code, 2);
	//           code = CodeMask(code, 8);
	//       }

	//       if (detectBounds.max.y < targetBounds.center.y)
	//       {
	//           code = CodeMask(code, 2);
	//           code = CodeMask(code, 8);
	//           code = CodeMask(code, 32);
	//           code = CodeMask(code, 128);
	//       }
	//       if (detectBounds.min.y > targetBounds.center.y)
	//       {
	//           code = CodeMask(code, 1);
	//           code = CodeMask(code, 4);
	//           code = CodeMask(code, 16);
	//           code = CodeMask(code, 64);
	//       }

	//       if (detectBounds.max.z < targetBounds.center.z)
	//       {
	//           code = CodeMask(code, 4);
	//           code = CodeMask(code, 64);
	//           code = CodeMask(code, 8);
	//           code = CodeMask(code, 128);
	//       }
	//       if (detectBounds.min.z > targetBounds.center.z)
	//       {
	//           code = CodeMask(code, 1);
	//           code = CodeMask(code, 16);
	//           code = CodeMask(code, 2);
	//           code = CodeMask(code, 32);
	//       }
	//       return code;
	//   }

	//   protected static int CalculateQuadBoundsCode(Bounds detectBounds, Bounds targetBounds)
	//   {

	//       if (detectBounds.max.x < targetBounds.min.x || detectBounds.min.x > targetBounds.max.x)
	//           return 0;
	//       if (detectBounds.max.y < targetBounds.min.y || detectBounds.min.y > targetBounds.max.y)
	//           return 0;
	//       if (detectBounds.max.z < targetBounds.min.z || detectBounds.min.z > targetBounds.max.z)
	//           return 0;
	//       int code = 15;
	//       if (detectBounds.max.x < targetBounds.center.x)
	//       {
	//           code = CodeMask(code, 4);
	//           code = CodeMask(code, 8);
	//       }
	//       if (detectBounds.min.x > targetBounds.center.x)
	//       {
	//           code = CodeMask(code, 1);
	//           code = CodeMask(code, 2);
	//       }

	//       if (detectBounds.max.z < targetBounds.center.z)
	//       {
	//           code = CodeMask(code, 2);
	//           code = CodeMask(code, 8);
	//       }
	//       if (detectBounds.min.z > targetBounds.center.z)
	//       {
	//           code = CodeMask(code, 1);
	//           code = CodeMask(code, 4);
	//       }
	//       return code;
	//   }

	//   private static int CodeMask(int code, int mask)
	//   {
	//       if ((code & mask) != 0)
	//           code = code ^ mask;
	//       return code;
	//   }

#if UNITY_EDITOR
	void OnDrawGizmos()
    {
        Bounds b = new Bounds(transform.position, detectorSize);
        b.DrawBounds(Color.yellow);
    }
#endif
}
