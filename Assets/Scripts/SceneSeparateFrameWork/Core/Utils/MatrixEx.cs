using UnityEngine;
using System.Collections;

public static class MatrixEx
{

    public static int ComputeOutCode(Vector4 pos, Matrix4x4 projection)
    {
        pos = projection * pos;
        int code = 0;
        if (pos.x < -pos.w) code |= 0x01;
        if (pos.x > pos.w) code |= 0x02;
        if (pos.y < -pos.w) code |= 0x04;
        if (pos.y > pos.w) code |= 0x08;
        if (pos.z < -pos.w) code |= 0x10;
        if (pos.z > pos.w) code |= 0x20;
        return code;
    }

    public static int ComputeOutCodeEx(Vector4 pos, Matrix4x4 projection, float leftex, float rightex, float downex, float upex)
    {
        pos = projection * pos;
        int code = 0;
        if (pos.x < (-1 + leftex) * pos.w) code |= 0x01;
        if (pos.x > (1 + rightex) * pos.w) code |= 0x02;
        if (pos.y < (-1 + downex) * pos.w) code |= 0x04;
        if (pos.y > (1 + upex) * pos.w) code |= 0x08;
        if (pos.z < -pos.w) code |= 0x10;
        if (pos.z > pos.w) code |= 0x20;
        return code;
    }
}
