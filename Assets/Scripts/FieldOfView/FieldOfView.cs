using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FieldOfView : MonoBehaviour
{
    [Header("Vision Settings")]
    public float viewRadius = 5f;
    [Range(0, 360)] public float viewAngle = 90f;
    public int meshResolution = 50;
    public LayerMask obstacleMask;

    [Header("Visual Settings")]
    public Material fovMaterial;
    [Range(0, 1)] public float gizmosAlpha = 0.5f;

    [Tooltip("ÿ֡���¼�����룩")]
    public float updateInterval = 0.1f;
    private float lastUpdateTime;

    [Header("Gizmos")]
    [Range(1, 90)] public float gizmoSegmentResolution = 10f; // �����߶��ܶ�

    [Header("Check Obj Info")]
    public List<Transform> visibleTargets = new List<Transform>(); // �ɼ������б�
    public LayerMask targetMask; // Ŀ������㼶
    
    [Header("Check Obj Interval")]
    [SerializeField] private float detectionInterval = 0.2f; // �����
    private float nextDetectionTime;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = fovMaterial;
        vertices = new Vector3[meshResolution + 1];
        triangles = new int[meshResolution * 3];
    }

    void Update()
    {
        if (Time.time > nextDetectionTime)
        {
            FindVisibleTargets();
            nextDetectionTime = Time.time + detectionInterval;
        }
    }

    void LateUpdate()
    {
        if (Time.time - lastUpdateTime > updateInterval)
        {
            DrawFieldOfView();
            lastUpdateTime = Time.time;
        }
    }

    void DrawFieldOfView()
    {
        int vertexCount = meshResolution + 1;
        bool isFullCircle = Mathf.Approximately(viewAngle, 360f);

        // �������������������360�������һ���ظ����㣩
        vertices = new Vector3[isFullCircle ? meshResolution + 1 : meshResolution + 2];
        vertices[0] = Vector3.zero;

        float stepAngle = viewAngle / meshResolution;
        int actualResolution = isFullCircle ? meshResolution : meshResolution + 1;

        for (int i = 1; i <= actualResolution; i++)
        {
            float angle = -viewAngle / 2 + stepAngle * (i - 1);
            Vector3 dir = DirFromAngle(angle, false);

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, viewRadius, obstacleMask))
                vertices[i] = transform.InverseTransformPoint(hit.point);
            else
                vertices[i] = dir * viewRadius;
        }

        // �����360�ȣ��պ����һ�����㵽��һ������
        if (isFullCircle)
        {
            vertices[vertices.Length - 1] = vertices[1];
        }

        // ��������������
        int[] triangles = new int[actualResolution * 3];
        for (int i = 0; i < actualResolution; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = (i + 2) % (actualResolution + 1);
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        // 1. ��ȡ��Ұ��Χ�ڵ����к�ѡĿ��
        Collider[] targetsInViewRadius = Physics.OverlapSphere(
            transform.position,
            viewRadius,
            targetMask
        );

        foreach (Collider target in targetsInViewRadius)
        {
            Transform targetTrans = target.transform;
            // 2. ����Ŀ�귽��
            Vector3 dirToTarget = (targetTrans.position - transform.position).normalized;

            // 3. �Ƕȼ��
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                // 4. ������
                float dstToTarget = Vector3.Distance(transform.position, targetTrans.position);

                // 5. �ϰ�����
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(targetTrans);
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool useGlobalAngle)
    {
        if (!useGlobalAngle)
            angleInDegrees += transform.eulerAngles.y;
        return new Vector3(
            Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),
            0,
            Mathf.Cos(angleInDegrees * Mathf.Deg2Rad)
        );
    }


    void OnDrawGizmos()
    {
        if (viewRadius <= 0) return;

        Gizmos.color = new Color(1, 0.92f, 0.016f, gizmosAlpha);
        bool isFullCircle = Mathf.Approximately(viewAngle, 360f);

        // ���Ʊ��ߣ���360��ʱ��
        if (!isFullCircle)
        {
            Vector3 leftDir = DirFromAngle(-viewAngle / 2, true);
            Vector3 rightDir = DirFromAngle(viewAngle / 2, true);
            Gizmos.DrawLine(transform.position, transform.position + leftDir * viewRadius);
            Gizmos.DrawLine(transform.position, transform.position + rightDir * viewRadius);
        }

        // ���ƻ���
        Vector3 prevPoint = Vector3.zero;
        int segments = Mathf.CeilToInt(viewAngle / gizmoSegmentResolution); // ÿ10��һ���߶�
        float angleStep = viewAngle / segments;

        for (int i = 0; i <= segments; i++)
        {
            // ����360�ȱպ�����
            float currentAngle = isFullCircle ?
                i * angleStep :
                -viewAngle / 2 + i * angleStep;

            Vector3 dir = DirFromAngle(currentAngle, true);
            Vector3 newPoint = transform.position + dir * viewRadius;

            if (i > 0)
            {
                Gizmos.DrawLine(prevPoint, newPoint);
            }

            prevPoint = newPoint;

            // ǿ�Ʊպ����һ�Σ���������
            if (isFullCircle && i == segments)
            {
                dir = DirFromAngle(0, true); // �ص�0�ȷ���
                newPoint = transform.position + dir * viewRadius;
                Gizmos.DrawLine(prevPoint, newPoint);
            }
        }

        // ���ƿɼ�Ŀ������
        Gizmos.color = Color.red;
        foreach (Transform target in visibleTargets)
        {
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}