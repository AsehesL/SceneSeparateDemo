using UnityEngine;
using System.Collections;

public abstract class SceneDetectorBase : MonoBehaviour, IDetector
{

	public Vector3 Position
    {
        get { return transform.position; }
    }

	public abstract bool UseCameraCulling { get; }

	public abstract bool IsDetected(Bounds bounds);

	public abstract int GetDetectedCode(float x, float y, float z, bool ignoreY);

	//public abstract int DetecedCode2D(float centerX, float centerY, float centerZ, float sizeX, float sizeY, float sizeZ);
	//public abstract int DetecedCode3D(float centerX, float centerY, float centerZ, float sizeX, float sizeY, float sizeZ);

	//public abstract int DetectedCode(Bounds bounds, SceneSeparateTreeType treeType);

	//public abstract int DetecedCode(float centerX, float centerY, float centerZ, float sizeX, float sizeY, float sizeZ, SceneSeparateTreeType treeType);
}
