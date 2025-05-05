using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class LPManager : MonoBehaviour
{
    public TMP_InputField targetAInput;
    public TMP_InputField targetBInput;
    public TMP_Dropdown optimizationType;
    public TMP_InputField constraintA;
    public TMP_InputField constraintB;
    public TMP_InputField constraintC;
    public TMP_Dropdown inequalityType;
    public Button addConstraintBtn;
    public Button solveBtn;
    public Button clearBtn;

    public GameObject linePrefab;
    public GameObject feasibleArea;
    public GameObject optimalMarker;

    private List<Constraint> constraints = new List<Constraint>();
    private List<GameObject> lineRenderers = new List<GameObject>();

    void Start()
    {
        addConstraintBtn.onClick.AddListener(AddConstraint);
        solveBtn.onClick.AddListener(Solve);
        clearBtn.onClick.AddListener(Clear);
    }

    void AddConstraint()
    {
        Constraint c = new Constraint();
        float.TryParse(constraintA.text, out c.a);
        float.TryParse(constraintB.text, out c.b);
        float.TryParse(constraintC.text, out c.c);
        c.isLessThanOrEqual = inequalityType.value == 0;
        constraints.Add(c);
        Debug.Log("��ӳɹ���");
    }

    void Solve()
    {
        if (constraints.Count < 2)
        {
            Debug.LogError("������Ҫ����Լ������");
            return;
        }

        if (string.IsNullOrEmpty(targetAInput.text) ||
            string.IsNullOrEmpty(targetBInput.text))
        {
            Debug.LogError("Ŀ�꺯��ϵ������Ϊ��");
            return;
        }
        ClearVisuals();

        // �������к�ѡ����
        List<Vector2> candidates = new List<Vector2>();
        for (int i = 0; i < constraints.Count; i++)
        {
            for (int j = i + 1; j < constraints.Count; j++)
            {
                Vector2 intersection;
                if (FindIntersection(constraints[i], constraints[j], out intersection))
                {
                    if (constraints.All(c => c.IsSatisfied(intersection)))
                    {
                        candidates.Add(intersection);
                    }
                }
            }
        }

        // ����͹��
        List<Vector2> convexHull = ComputeConvexHull(candidates);

        // ���ɿ�����
        GenerateFeasibleArea(convexHull);

        // ����Լ����
        foreach (var c in constraints)
        {
            DrawConstraintLine(c);
        }

        // ���㲢������Ž�
        if (convexHull.Count > 0)
        {
            Vector2 optimal = FindOptimalSolution(convexHull);
            optimalMarker.transform.position = optimal;
        }
    }

    bool FindIntersection(Constraint c1, Constraint c2, out Vector2 point)
    {
        float denom = c1.a * c2.b - c2.a * c1.b;
        if (Mathf.Approximately(denom, 0))
        {
            point = Vector2.zero;
            return false;
        }
        point = new Vector2(
            (c1.c * c2.b - c2.c * c1.b) / denom,
            (c1.a * c2.c - c2.a * c1.c) / denom
        );
        return true;
    }

    List<Vector2> ComputeConvexHull(List<Vector2> points)
    {
        // ʵ��Andrew's͹���㷨
        var sorted = points.OrderBy(p => p.x).ThenBy(p => p.y).ToList();
        List<Vector2> hull = new List<Vector2>();

        // ��͹��
        foreach (var p in sorted)
        {
            while (hull.Count >= 2 && Cross(hull[hull.Count - 2], hull[hull.Count - 1], p) <= 0)
                hull.RemoveAt(hull.Count - 1);
            hull.Add(p);
        }

        // ��͹��
        int lowerCount = hull.Count;
        for (int i = sorted.Count - 2; i >= 0; i--)
        {
            while (hull.Count >= lowerCount + 1 &&
                   Cross(hull[hull.Count - 2], hull[hull.Count - 1], sorted[i]) <= 0)
                hull.RemoveAt(hull.Count - 1);
            hull.Add(sorted[i]);
        }

        return hull.Distinct().ToList();
    }

    float Cross(Vector2 a, Vector2 b, Vector2 c)
    {
        return (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
    }

    void GenerateFeasibleArea(List<Vector2> vertices)
    {
        MeshFilter mf = feasibleArea.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mf.mesh = mesh;

        Vector3[] verts3D = vertices.Select(v => new Vector3(v.x, v.y, 0)).ToArray();
        int[] triangles = Triangulator.Triangulate(vertices.ToArray());

        mesh.vertices = verts3D;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    void DrawConstraintLine(Constraint c)
    {
        GameObject lineObj = Instantiate(linePrefab);
        LineRenderer lr = lineObj.GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;

        // ����Լ��������Ļ�߽�Ľ���
        Camera cam = Camera.main;
        Vector3[] screenCorners = new Vector3[4];
        screenCorners[0] = cam.ViewportToWorldPoint(new Vector3(0, 0, 10)); // ����
        screenCorners[1] = cam.ViewportToWorldPoint(new Vector3(1, 0, 10)); // ����
        screenCorners[2] = cam.ViewportToWorldPoint(new Vector3(0, 1, 10)); // ����
        screenCorners[3] = cam.ViewportToWorldPoint(new Vector3(1, 1, 10)); // ����

        List<Vector3> intersections = new List<Vector3>();

        // ���Լ������������Ļ�߽�Ľ���
        foreach (var edge in new[] {
        new { start = screenCorners[0], end = screenCorners[1] }, // �ױ�
        new { start = screenCorners[1], end = screenCorners[3] }, // �ұ�
        new { start = screenCorners[3], end = screenCorners[2] }, // ����
        new { start = screenCorners[2], end = screenCorners[0] }  // ���
    })
        {
            Vector2 inter;
            if (LineIntersection(c, edge.start, edge.end, out inter))
            {
                intersections.Add(inter);
            }
        }

        // ֻ����������Զ��
        if (intersections.Count >= 2)
        {
            lr.SetPosition(0, intersections[0]);
            lr.SetPosition(1, intersections[1]);
        }
        lineRenderers.Add(lineObj);
    }

    bool LineIntersection(Constraint constraint, Vector3 lineStart, Vector3 lineEnd, out Vector2 intersection)
    {
        float a1 = constraint.a;
        float b1 = constraint.b;
        float c1 = constraint.c;

        Vector2 p1 = new Vector2(lineStart.x, lineStart.y);
        Vector2 p2 = new Vector2(lineEnd.x, lineEnd.y);

        // ����Ļ�߽��߶�ת��Ϊһ��ʽ����
        float a2 = p2.y - p1.y;
        float b2 = p1.x - p2.x;
        float c2 = a2 * p1.x + b2 * p1.y;

        float determinant = a1 * b2 - a2 * b1;
        if (Mathf.Approximately(determinant, 0))
        {
            intersection = Vector2.zero;
            return false; // ƽ��
        }

        float x = (b2 * c1 - b1 * c2) / determinant;
        float y = (a1 * c2 - a2 * c1) / determinant;

        // ��齻���Ƿ����߶���
        bool isOnSegment = (x >= Mathf.Min(p1.x, p2.x) && x <= Mathf.Max(p1.x, p2.x)) &&
                           (y >= Mathf.Min(p1.y, p2.y) && y <= Mathf.Max(p1.y, p2.y));

        intersection = isOnSegment ? new Vector2(x, y) : Vector2.zero;
        return isOnSegment;
    }

    Vector2 FindOptimalSolution(List<Vector2> vertices)
    {
        float targetA = float.Parse(targetAInput.text);
        float targetB = float.Parse(targetBInput.text);
        bool maximize = optimizationType.value == 0;

        Vector2 optimal = vertices[0];
        float extremeVal = targetA * optimal.x + targetB * optimal.y;

        foreach (var v in vertices)
        {
            float current = targetA * v.x + targetB * v.y;
            if ((maximize && current > extremeVal) ||
                (!maximize && current < extremeVal))
            {
                extremeVal = current;
                optimal = v;
            }
        }
        return optimal;
    }

    void Clear()
    {
        constraints.Clear();
        ClearVisuals();
    }

    void ClearVisuals()
    {
        foreach (var lr in lineRenderers) Destroy(lr);
        lineRenderers.Clear();
        feasibleArea.GetComponent<MeshFilter>().mesh = null;
        optimalMarker.transform.position = Vector3.zero;
    }
}
