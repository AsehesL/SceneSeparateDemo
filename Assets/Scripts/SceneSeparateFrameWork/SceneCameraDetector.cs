using UnityEngine;
using System.Collections;

/// <summary>
/// 该触发器根据相机裁剪区域触发
/// </summary>
public class SceneCameraDetector : SceneDetectorBase
{
    protected Camera m_Camera;

    protected int[] m_Codes;
    protected SceneSeparateTreeType m_CurrentTreeType;

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

    public override int DetectedCode(Bounds bounds, SceneSeparateTreeType treeType)
    {
        if (m_Camera == null)
            return 0;
        Matrix4x4 matrix = m_Camera.projectionMatrix * m_Camera.worldToCameraMatrix;

        if (m_Codes == null || m_CurrentTreeType != treeType)
        {
            m_Codes = new int[treeType == SceneSeparateTreeType.OcTree ? 27 : 18];
            m_CurrentTreeType = treeType;
        }

        int code = 0;
        int index = 0;
        if (treeType == SceneSeparateTreeType.OcTree)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        m_Codes[index] =
                            CalculateCullCode(
                                new Vector4(bounds.center.x + bounds.size.x*0.5f*(i - 1),
                                    bounds.center.y + bounds.size.y*0.5f*(k - 1),
                                    bounds.center.z + bounds.size.z*0.5f*(j - 1), 1), matrix);
                        index += 1;
                    }
                }
            }

            
            code = CalculateOcTreeBoundsCullCode(code, 0, 1);
            code = CalculateOcTreeBoundsCullCode(code, 1, 2);
            code = CalculateOcTreeBoundsCullCode(code, 3, 4);
            code = CalculateOcTreeBoundsCullCode(code, 4, 8);
            code = CalculateOcTreeBoundsCullCode(code, 9, 16);
            code = CalculateOcTreeBoundsCullCode(code, 10, 32);
            code = CalculateOcTreeBoundsCullCode(code, 12, 64);
            code = CalculateOcTreeBoundsCullCode(code, 13, 128);
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = -1; k < 2; k+=2)
                    {
                        m_Codes[index] =
                            CalculateCullCode(
                                new Vector4(bounds.center.x + bounds.size.x * 0.5f * (i - 1),
                                    bounds.center.y + bounds.size.y * 0.5f * k,
                                    bounds.center.z + bounds.size.z * 0.5f * (j - 1), 1), matrix);
                        index += 1;
                    }
                }
            }


            code = CalculateQuadTreeBoundsCullCode(code, 0, 1);
            code = CalculateQuadTreeBoundsCullCode(code, 2, 2);
            code = CalculateQuadTreeBoundsCullCode(code, 6, 4);
            code = CalculateQuadTreeBoundsCullCode(code, 8, 8);
        }

        return code;
    }

	public override int DetecedCode(float centerX, float centerY, float centerZ, float sizeX, float sizeY, float sizeZ, SceneSeparateTreeType treeType)
	{
		throw new System.NotImplementedException();
	}

	protected virtual int CalculateCullCode(Vector4 position, Matrix4x4 matrix)
    {
        return MatrixEx.ComputeOutCode(position, matrix);
    }

    private int CalculateOcTreeBoundsCullCode(int code, int index, int mask)
    {
        int c = m_Codes[index];
        c &= m_Codes[index + 1];
        c &= m_Codes[index + 2];
        c &= m_Codes[index + 3];
        c &= m_Codes[index + 4];
        c &= m_Codes[index + 5];
        c &= m_Codes[index + 9];
        c &= m_Codes[index + 10];
        c &= m_Codes[index + 11];
        c &= m_Codes[index + 12];
        c &= m_Codes[index + 13];
        c &= m_Codes[index + 14];

        if (c == 0)
            code |= mask;
        return code;
    }

    private int CalculateQuadTreeBoundsCullCode(int code, int index, int mask)
    {
        int c = m_Codes[index];
        c &= m_Codes[index + 1];
        c &= m_Codes[index + 2];
        c &= m_Codes[index + 3];
        c &= m_Codes[index + 6];
        c &= m_Codes[index + 7];
        c &= m_Codes[index + 8];
        c &= m_Codes[index + 9];

        if (c == 0)
            code |= mask;
        return code;
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
