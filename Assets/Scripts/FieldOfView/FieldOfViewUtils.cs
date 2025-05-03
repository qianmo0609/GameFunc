using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public static class FieldOfViewUtils
{

    //方案1：网格遍历算法（DDA算法）
    //适用场景​​：基于网格的2D/3D游戏（如策略游戏、迷宫类游戏）
    //优点：规则网格结构检测效率高，可预先生成导航网格优化性能
    //缺点：网格精度影响检测结果，斜向移动时可能有漏检
    public static bool CheckLineOfSight(Vector3 start, Vector3 end, float gridSize, int layerMask)
    {
        Vector3 direction = (end - start).normalized;
        float maxDistance = Vector3.Distance(start, end);
        int steps = Mathf.CeilToInt(maxDistance / gridSize);

        for (int i = 0; i <= steps; i++)
        {
            Vector3 checkPos = start + direction * i * gridSize;
            // 假设使用自定义的网格检测方法
            if (IsGridBlocked(checkPos, gridSize, layerMask))
                return false;
        }
        return true;
    }

    static bool IsGridBlocked(Vector3 position, float gridSize, int obstacleMask)
    {
        // 自定义网格检测逻辑（示例使用OverlapBox）
        Collider[] colliders = Physics.OverlapBox(
            position,
            Vector3.one * gridSize / 2,
            Quaternion.identity,
            obstacleMask
        );
        return colliders.Length > 0;
    }

    //方案2：视线路径采样检测
    //​适用场景​​：需要高精度的3D场景检测
    //优点：可检测体积遮挡，避免射线穿透薄物体的问题
    //缺点：采样数影响性能，小物体可能漏检
    public static bool CheckLineOfSight(Vector3 start, Vector3 end, int samples, int obstacleMask)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        direction.Normalize();

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / (samples - 1);
            Vector3 checkPoint = start + direction * distance * t;

            // 使用球体检测替代射线检测
            if (Physics.CheckSphere(checkPoint, 0.1f, obstacleMask))
                return false;
        }
        return true;
    }

    //方案3：几何视锥检测
    //适用场景​​：需要数学精确解的3D场景
    //优点：精确的几何计算，可检测部分遮挡
    //缺点：计算量较大，需要处理目标物体的包围盒
    public static bool IsInSight(Vector3 targetPos)
    {
        return false;
        //// 1. 计算目标相对位置
        //Vector3 localPos = transform.InverseTransformPoint(targetPos);

        //// 2. 视锥角度检测
        //float angle = Vector3.Angle(Vector3.forward, localPos.normalized);
        //if (angle > viewAngle / 2) return false;

        //// 3. 构建视锥平面
        //Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(
        //    new Camera()
        //    {
        //        fieldOfView = viewAngle,
        //        aspect = 1,
        //        nearClipPlane = 0.1f,
        //        farClipPlane = viewRadius
        //    }
        //);

        //// 4. 包围盒检测
        //Bounds targetBounds = new Bounds(targetPos, Vector3.one * 0.5f);
        //return GeometryUtility.TestPlanesAABB(frustumPlanes, targetBounds);
    }

    //方案4：空间划分加速结构
    //使用场景：大规模开放世界场景
    //优化:动态更新空间划分结构，使用跳表加速邻近搜索
    static void CheckVisibility()
    {
        //Octree octree = GetComponent<Octree>();
        //foreach (var target in targets)
        //{
        //    if (octree.CheckPath(transform.position, target.position))
        //        visibleTargets.Add(target);
        //}
    }

    //方案5：GPU并行检测 (Compute Shader)
    //适用场景​​：

    //public ComputeShader visibilityShader;
    //ComputeBuffer positionBuffer;
    //ComputeBuffer resultBuffer;

    //void GPUDetect()
    //{
    //    int kernel = visibilityShader.FindKernel("CheckVisibility");

    //    // 设置缓冲区
    //    positionBuffer = new ComputeBuffer(targetCount, 12);
    //    resultBuffer = new ComputeBuffer(targetCount, 4);

    //    // 传递参数
    //    visibilityShader.SetBuffer(kernel, "Positions", positionBuffer);
    //    visibilityShader.SetBuffer(kernel, "Results", resultBuffer);
    //    visibilityShader.SetVector("ViewerPos", transform.position);
    //    visibilityShader.SetFloat("MaxDistance", viewRadius);
    //    visibilityShader.SetFloat("ViewAngle", viewAngle);

    //    // 执行计算
    //    visibilityShader.Dispatch(kernel, Mathf.CeilToInt(targetCount / 64f), 1, 1);

    //    // 读取结果
    //    bool[] results = new bool[targetCount];
    //    resultBuffer.GetData(results);
    //}
}

public class OctreeTest
{
    public void Build(List<object> obstacles)
    {
        // 构建空间划分结构
    }

    public bool CheckPath(Vector3 start, Vector3 end)
    {
        // 递归检测路径节点
        return false;
    }
}