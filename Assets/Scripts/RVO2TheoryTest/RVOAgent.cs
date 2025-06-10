// RVOAgent.cs - 智能体逻辑
using UnityEngine;
using System.Collections.Generic;

public class RVOAgent : MonoBehaviour
{
    [Header("智能体半径")]
    public float radius = 0.5f;     // 智能体半径
    [Header("智能体最大速度")]
    public float maxSpeed = 2.0f;   // 最大速度
    [Header("目标位置")]
    public Vector3 goal;           // 目标位置
    [Header("当前最大速度")]
    public Vector3 velocity;        // 当前速度

    private float detectionRadius = 5.0f; //检测半径
    private float taoWinTime = 5.0f;  //时间窗口τ
    private List<ORCAConstraint> constraints = new List<ORCAConstraint>();

    void Update()
    {
        ComputeConstraints();          // 计算所有ORCA约束
        velocity = ComputeOptimalVelocity(); // 求解最优速度
        transform.position += velocity * Time.deltaTime; // 更新位置
        if(Vector3.Distance(transform.position,goal) < 1.5f)
        {
            Vector2 g = Random.insideUnitCircle;
            goal = new Vector3(g.x * 10f, 0,g.y * 10f);
        }
        // 朝向调整（可选）
        if (velocity.magnitude > 0.1f)
        {
            transform.forward = velocity.normalized;
        }
    }

    /// <summary>
    /// 计算所有ORCA约束
    /// </summary>
    void ComputeConstraints()
    {
        constraints.Clear();
        //得到检测半径内的RVOAgent对象
        Collider[] neighbors = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (var neighbor in neighbors)
        {
            if (neighbor.gameObject == this.gameObject) continue;

            //临近的RVOAgent的位置
            Vector3 otherPos = neighbor.transform.position;
            //临近的RVOAgent的速度
            Vector3 otherVel = neighbor.GetComponent<RVOAgent>()?.velocity ?? Vector3.zero;
            //临近的RVOAgent的半径
            float otherRadius = neighbor.GetComponent<RVOAgent>()?.radius ?? 0.5f;

            // 计算与障碍物/智能体的ORCA约束
            AddORCAConstraint(otherPos, otherVel, otherRadius);
        }
    }

    /// <summary>
    /// 添加ORCA约束
    /// </summary>
    /// <param name="otherPos">临近的RVOAgent的位置</param>
    /// <param name="otherVel">临近的RVOAgent的速度</param>
    /// <param name="otherRadius">临近的RVOAgent的半径</param>
    void AddORCAConstraint(Vector3 otherPos, Vector3 otherVel, float otherRadius)
    {
        //临近物体到当前物体的向量
        Vector3 relativePos = otherPos - transform.position;
        //当前速度与临近物体速度的相对差
        Vector3 relativeVel = velocity - otherVel;
        //两物体半径之和
        float combinedRadius = radius + otherRadius;
        //物体间距离
        float distance = relativePos.magnitude;

        //若 distance = 0（已重叠），直接返回避免除零错误
        if (distance == 0) return;

        Vector3 u; // 需调整的速度方向
        if (distance < combinedRadius)
        {
            // 已经碰撞，直接推开
            // 直接沿相对方向推开至刚好接触（重叠部分为移动距离）。
            u = relativePos.normalized * (combinedRadius - distance);
        }
        //预测未来碰撞
        else
        {
            // 计算碰撞时间
            //timeToCollision = 需缩小的距离 / 相对速度在碰撞方向的分量。
            //相对速度在碰撞方向的分量:Vector3.Dot(relativeVel, relativePos.normalized)
            float timeToCollision = (distance - combinedRadius) / Vector3.Dot(relativeVel, relativePos.normalized);
            //若时间超出窗口(τ = 5s) 或远离（负值），忽略此约束。
            if (timeToCollision < 0 || timeToCollision > taoWinTime) return; // 时间窗口τ=5秒

            // 计算避让方向（ORCA核心）
            Vector3 collisionDir = relativePos.normalized;

            //​​避让速度 u 的计算​​
            //目标：在 timeToCollision 内将相对速度调整至刚好避开（移动 combinedRadius）
            //公式：新相对速度 = combinedRadius / timeToCollision * collisionDir
            //u = 新相对速度 - 当前的相对速度
            u = (collisionDir * combinedRadius / timeToCollision) - relativeVel;
        }

        // 生成半平面约束：velocity · direction <= offset
        ORCAConstraint constraint;
        constraint.direction = u.normalized;
        //ORCA 责任分配原则​​：
        //双方各承担一半避让责任（各调整 0.5u），实现双向协调。
        //offset 是对方速度加一半调整量(otherVel + 0.5u) 在约束方向的值，确保新速度不会导致碰撞。
        constraint.offset = Vector3.Dot(otherVel + 0.5f * u, constraint.direction);
        constraints.Add(constraint);
    }

    /// <summary>
    /// 求解最优速度
    /// </summary>
    /// <returns></returns>
    Vector3 ComputeOptimalVelocity()
    {
        //计算期望速度​​，朝向目标 (goal) 的满速方向
        Vector3 desiredVel = (goal - transform.position).normalized * maxSpeed;
        Vector3 result = desiredVel;

        for (int i = 0; i < constraints.Count; i++)
        {
            ORCAConstraint c = constraints[i];
            //如果当前速度在约束方向上的投影超过偏移量（即违反了约束），则将速度投影到约束边界上。
            float currentDot = Vector3.Dot(result, c.direction);
            if (currentDot > c.offset)
            {
                // 投影到约束半平面
                result += (c.offset - currentDot) * c.direction;
            }
        }

        // 限制最大速度
        if (result.magnitude > maxSpeed)
        {
            result = result.normalized * maxSpeed;
        }

        result.y = 0;
        return result;
    }

    void OnDrawGizmos()
    {
        // 目标方向
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, goal);

        // 当前速度
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + velocity);

        // ORCA约束半平面
        foreach (var c in constraints)
        {
            Vector3 tangent = new Vector3(-c.direction.z, 0, c.direction.x); // 切线方向
            Vector3 lineStart = transform.position + tangent * 10;
            Vector3 lineEnd = transform.position - tangent * 10;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(lineStart, lineEnd);
        }

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

public struct ORCAConstraint
{
    public Vector3 direction;      // 约束半平面法向量
    public float offset;           // 约束偏移量
}