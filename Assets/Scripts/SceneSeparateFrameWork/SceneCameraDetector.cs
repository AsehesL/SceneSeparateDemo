using UnityEngine;
using System.Collections;

/// <summary>
/// 该触发器根据相机裁剪区域触发
/// </summary>
public class SceneCameraDetector : SceneDetectorBase
{
    protected Camera m_Camera;

	//protected int[] m_Codes;
	//protected SceneSeparateTreeType m_CurrentTreeType;

	public override bool UseCameraCulling
	{
		get { return true; }
	}

	void Start()
    {
        m_Camera = gameObject.GetComponent<Camera>();
        //m_Codes = new int[27];
    }

    public override bool IsDetected(Bounds bounds)
    {
        if (m_Camera == null)
            return false;
        return bounds.IsBoundsInCamera(m_Camera);
    }

	public override int GetDetectedCode(float x, float y, float z, bool ignoreY)
	{
		if (m_Camera == null)
			return 0;
		Matrix4x4 matrix = m_Camera.cullingMatrix;
		return CalculateCullCode(new Vector4(x, y, z, 1.0f), matrix);
	}

	//public override int DetecedCode2D(float centerX, float centerY, float centerZ, float sizeX, float sizeY, float sizeZ)
	//{
	//    if (m_Camera == null)
	//        return 0;
	//    Matrix4x4 matrix = m_Camera.cullingMatrix;

	//    float hsizex = sizeX * 0.5f;
	//    float hsizey = sizeY * 0.5f;
	//    float hsizez = sizeZ * 0.5f;

	//    int code = 0;
	//    for (int j = 0; j < 2; j++)
	//    {
	//        for (int i = 0; i < 2; i++)
	//        {
	//            code |= ComputeCellCode(
	//                new Vector3(centerX + (i - 1) * hsizex, centerY - hsizey, centerZ + (j - 1) * hsizez),
	//                hsizex, hsizey, hsizez, matrix, 1 << (j * 2 + i));
	//        }
	//    }

	//    return code;
	//}

	//public override int DetecedCode3D(float centerX, float centerY, float centerZ, float sizeX, float sizeY, float sizeZ)
	//{
	//    return 0;
	//}

	//private int ComputeCellCode(Vector3 position, float sizex, float sizey, float sizez, Matrix4x4 matrix, int code)
	//{
	//    int c = CalculateCullCode(new Vector4(position.x, position.y, position.z, 1.0f), matrix);
	//    c &= CalculateCullCode(new Vector4(position.x + sizex, position.y, position.z, 1.0f), matrix);
	//    c &= CalculateCullCode(new Vector4(position.x, position.y + sizey, position.z, 1.0f), matrix);
	//    c &= CalculateCullCode(new Vector4(position.x, position.y, position.z + sizez, 1.0f), matrix);
	//    c &= CalculateCullCode(new Vector4(position.x + sizex, position.y + sizey, position.z, 1.0f), matrix);
	//    c &= CalculateCullCode(new Vector4(position.x + sizex, position.y, position.z + sizez, 1.0f), matrix);
	//    c &= CalculateCullCode(new Vector4(position.x, position.y + sizey, position.z + sizez, 1.0f), matrix);
	//    c &= CalculateCullCode(new Vector4(position.x + sizex, position.y + sizey, position.z + sizez, 1.0f), matrix);
	//    if (c == 0)
	//        return code;
	//    return 0;
	//}

	protected virtual int CalculateCullCode(Vector4 position, Matrix4x4 matrix)
	{
		return MatrixEx.ComputeOutCode(position, matrix);
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
    {
        Camera camera = gameObject.GetComponent<Camera>();
        if (camera)
            GizmosEx.DrawViewFrustum(camera, Color.yellow);
    }
#endif
}
