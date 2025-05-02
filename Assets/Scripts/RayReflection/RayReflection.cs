using UnityEngine;

[ExecuteInEditMode]
public class RayReflection : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Rect boundary = new Rect(-5, -3, 10, 6); // x, y, width, height
    public int maxReflections = 5;
    public Transform flagObj;

    [Header("Gizmos Settings")]
    public Color boundaryColor = Color.green;

    void Update()
    {
        CalculateAndDrawPath();
    }

    void CalculateAndDrawPath()
    {
        Vector2 currentPos = flagObj.position;
        Vector2 currentDir = new Vector2(flagObj.rotation.eulerAngles.x, flagObj.rotation.eulerAngles.z);

        Vector3[] points = new Vector3[maxReflections + 1];
        points[0] = currentPos;
        int pointCount = 1;

        for (int i = 0; i < maxReflections; i++)
        {
            float minDistance = Mathf.Infinity;
            Vector2 hitNormal = Vector2.zero;

            // 计算与各边界的交点
            CheckBoundaryCollision(currentPos, currentDir, ref minDistance, ref hitNormal);

            if (minDistance < Mathf.Infinity)
            {
                currentPos += currentDir * minDistance;
                currentDir = Vector2.Reflect(currentDir, hitNormal);
                points[pointCount++] = currentPos;
            }
            else
            {
                break;
            }
        }

        lineRenderer.positionCount = pointCount;
        lineRenderer.SetPositions(points);
    }

    void CheckBoundaryCollision(Vector2 origin, Vector2 dir, ref float minT, ref Vector2 normal)
    {
        // 左右边界
        if (dir.x != 0)
        {
            float tX = (dir.x > 0) ?
                (boundary.xMax - origin.x) / dir.x :
                (boundary.xMin - origin.x) / dir.x;

            Vector2 hitPoint = origin + tX * dir;
            if (hitPoint.y >= boundary.yMin && hitPoint.y <= boundary.yMax && tX > 0)
            {
                if (tX < minT)
                {
                    minT = tX;
                    normal = new Vector2(-Mathf.Sign(dir.x), 0);
                }
            }
        }

        // 上下边界
        if (dir.y != 0)
        {
            float tY = (dir.y > 0) ?
                (boundary.yMax - origin.y) / dir.y :
                (boundary.yMin - origin.y) / dir.y;

            Vector2 hitPoint = origin + tY * dir;
            if (hitPoint.x >= boundary.xMin && hitPoint.x <= boundary.xMax && tY > 0)
            {
                if (tY < minT)
                {
                    minT = tY;
                    normal = new Vector2(0, -Mathf.Sign(dir.y));
                }
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        DrawBoundaryGizmos();
    }

    void DrawBoundaryGizmos()
    {
        Gizmos.color = boundaryColor;

        // 计算矩形四个角的世界坐标
        Vector3 bottomLeft = new Vector3(boundary.xMin, boundary.yMin, 0);
        Vector3 topLeft = new Vector3(boundary.xMin, boundary.yMax, 0);
        Vector3 topRight = new Vector3(boundary.xMax, boundary.yMax, 0);
        Vector3 bottomRight = new Vector3(boundary.xMax, boundary.yMin, 0);

        // 绘制矩形边框
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);

        // 绘制法线指示箭头（可选）
        DrawNormalGizmo(boundary.center + Vector2.up * boundary.height / 2, Vector2.down); // 上边界
        DrawNormalGizmo(boundary.center + Vector2.down * boundary.height / 2, Vector2.up); // 下边界
        DrawNormalGizmo(boundary.center + Vector2.right * boundary.width / 2, Vector2.left); // 右边界
        DrawNormalGizmo(boundary.center + Vector2.left * boundary.width / 2, Vector2.right); // 左边界
    }

    void DrawNormalGizmo(Vector2 position, Vector2 normal)
    {
        float arrowSize = 0.3f;
        Vector2 arrowEnd = position + normal * arrowSize;
        Gizmos.DrawLine(position, arrowEnd);
        Gizmos.DrawRay(arrowEnd, Quaternion.Euler(0, 0, 135) * normal * arrowSize / 2);
        Gizmos.DrawRay(arrowEnd, Quaternion.Euler(0, 0, -135) * normal * arrowSize / 2);
    }
#endif
}