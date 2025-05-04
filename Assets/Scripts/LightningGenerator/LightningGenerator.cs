using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class LightningGenerator : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public int iterations = 5;
    public float displacement = 0.3f;
    public Vector3 startPoint = Vector3.zero;
    public Vector3 endPoint = new Vector3(5, 0, 0);
    public float updateInterval = 0.1f;

    private float timer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0;
            GenerateLightning();
        }
    }

    void GenerateLightning()
    {
        List<Vector3> points = new List<Vector3> { startPoint, endPoint };

        for (int i = 0; i < iterations; i++)
        {
            List<Vector3> newPoints = new List<Vector3>();
            for (int j = 0; j < points.Count - 1; j++)
            {
                Vector3 current = points[j];
                Vector3 next = points[j + 1];
                Vector3 mid = (current + next) * 0.5f;

                // 计算随机垂直方向
                Vector3 direction = next - current;
                Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward).normalized;
                float disp = displacement * Mathf.Pow(0.5f, i);
                mid += perpendicular * Random.Range(-disp, disp);

                newPoints.Add(current);
                newPoints.Add(mid);
            }
            newPoints.Add(points[points.Count - 1]);
            points = newPoints;
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}
