using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MobileClothSimulation : MonoBehaviour
{
    // ģ�����
    public int rows = 16;
    public int cols = 24;
    public float springStiffness = 1200f;
    public float damping = 0.97f;
    public float mass = 1f;

    [Header("Environment Forces")]
    public Vector3 gravity = new Vector3(0, -9.8f, 0);

    [Header("Cloth Constraints")]
    public float maxStretch = 1.3f;
    public float bendStiffness = 300f;

    [Header("Wind Settings")]
    public Vector3 windDirection = new Vector3(0, 0, 1);
    public float windStrength = 5f;
    public float windTurbulence = 0.3f;
    public float windFrequency = 1f;

    private float windOffset;

    private Mesh mesh;
    private Vector3[] vertices;
    private List<Particle> particles = new List<Particle>();
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    void Start()
    {
        InitializeParticles();
        GenerateMesh();
        StoreTransformState();
    }

    void Update()
    {
        HandleTransformMovement();
        Simulate(Time.deltaTime);
        UpdateMesh();
    }

    void InitializeParticles()
    {
        // �ھֲ��ռ��ʼ������λ��
        Vector3 size = new Vector3(2f, 2f, 0); // ���ϳߴ�
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Particle p = new Particle();
                Vector3 localPos = new Vector3(
                    (x / (cols - 1f) - 0.5f) * size.x,
                    (0.5f - y / (rows - 1f)) * size.y,
                    0);

                p.worldPosition = transform.TransformPoint(localPos);
                p.previousWorldPosition = p.worldPosition;
                p.mass = (y == 0) ? float.MaxValue : mass;
                particles.Add(p);
            }
        }

        // �����������ӣ��ṹ���ɺͶԽ��ߵ��ɣ�
        CreateSpringConnections();
    }

    void CreateSpringConnections()
    {
        // �ṹ����
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                int index = y * cols + x;
                if (x < cols - 1) ConnectParticles(index, index + 1);    // ˮƽ
                if (y < rows - 1) ConnectParticles(index, index + cols);  // ��ֱ
            }
        }

        // �������ɣ��Խ��ߣ�
        for (int y = 0; y < rows - 1; y++)
        {
            for (int x = 0; x < cols - 1; x++)
            {
                int index = y * cols + x;
                ConnectParticles(index, index + cols + 1);
                ConnectParticles(index + 1, index + cols);
            }
        }
    }

    void ConnectParticles(int a, int b)
    {
        Particle p1 = particles[a];
        Particle p2 = particles[b];
        float initialDist = Vector3.Distance(p1.worldPosition, p2.worldPosition);
        p1.connections.Add(new SpringConnection(p2, initialDist));
        p2.connections.Add(new SpringConnection(p1, initialDist));
    }

    void GenerateMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // ��ʼ����������
        vertices = new Vector3[particles.Count];
        Vector2[] uv = new Vector2[particles.Count];
        for (int i = 0; i < particles.Count; i++)
        {
            vertices[i] = transform.InverseTransformPoint(particles[i].worldPosition);
            uv[i] = new Vector2(
                (particles[i].worldPosition.x - transform.position.x) / 2f + 0.5f,
                (particles[i].worldPosition.y - transform.position.y) / 2f + 0.5f);
        }

        // ��������������
        int[] triangles = new int[(rows - 1) * (cols - 1) * 6];
        int t = 0;
        for (int y = 0; y < rows - 1; y++)
        {
            for (int x = 0; x < cols - 1; x++)
            {
                int i = y * cols + x;
                triangles[t++] = i;
                triangles[t++] = i + cols;
                triangles[t++] = i + 1;

                triangles[t++] = i + 1;
                triangles[t++] = i + cols;
                triangles[t++] = i + cols + 1;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    void HandleTransformMovement()
    {
        // ���任״̬�仯
        if (transform.position != lastPosition || transform.rotation != lastRotation)
        {
            Matrix4x4 deltaMatrix = Matrix4x4.TRS(
                transform.position - lastPosition,
                Quaternion.Inverse(lastRotation) * transform.rotation,
                Vector3.one
            );

            // ������������λ��
            foreach (Particle p in particles)
            {
                p.worldPosition = deltaMatrix.MultiplyPoint(p.worldPosition);
                p.previousWorldPosition = deltaMatrix.MultiplyPoint(p.previousWorldPosition);
            }

            StoreTransformState();
        }
    }

    void StoreTransformState()
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    void Simulate(float deltaTime)
    {
        // ���㶯̬����
        Vector3 wind = windDirection.normalized * windStrength;
        wind += new Vector3(
            Mathf.PerlinNoise(Time.time * windFrequency + windOffset, 0) - 0.5f,
            Mathf.PerlinNoise(0, Time.time * windFrequency + windOffset) - 0.5f,
            Mathf.PerlinNoise(Time.time * windFrequency + windOffset, 100) - 0.5f
        ) * windTurbulence * Mathf.Sin(windFrequency * Time.deltaTime * Mathf.PI);

        // Ӧ�û�����
        foreach (Particle p in particles)
        {
            if (p.mass < float.MaxValue)
            {
                p.force = gravity * p.mass + wind;
            }
        }

        // ������Լ��
        foreach (Particle p in particles)
        {
            foreach (SpringConnection connection in p.connections)
            {
                Particle other = connection.particle;
                Vector3 delta = p.worldPosition - other.worldPosition;
                float currentDist = delta.magnitude;
                float stretchRatio = currentDist / connection.initialDistance;

                // ��������
                if (stretchRatio > maxStretch)
                {
                    float compression = (currentDist - connection.initialDistance * maxStretch) * 0.5f;
                    Vector3 adjust = delta.normalized * compression;
                    if (p.mass < float.MaxValue) p.worldPosition -= adjust;
                    if (other.mass < float.MaxValue) other.worldPosition += adjust;
                }
                else
                {
                    // ���˶��ɼ��㵯����
                    float stiffness = (other.mass == float.MaxValue) ?
                        springStiffness * 2 : springStiffness;
                    Vector3 force = stiffness * (connection.initialDistance - currentDist) * delta.normalized;
                    p.force += force;
                    other.force -= force;
                }
            }
        }

        // Verlet���ָ���λ��
        foreach (Particle p in particles)
        {
            if (p.mass < float.MaxValue)
            {
                Vector3 velocity = (p.worldPosition - p.previousWorldPosition) * damping;
                p.previousWorldPosition = p.worldPosition;
                p.worldPosition += velocity + (p.force / p.mass) * deltaTime * deltaTime;
                p.force = Vector3.zero;
            }
        }
    }

    void UpdateMesh()
    {
        // ת���������굽�ֲ�����
        for (int i = 0; i < particles.Count; i++)
        {
            vertices[i] = transform.InverseTransformPoint(particles[i].worldPosition);
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    class Particle
    {
        public Vector3 worldPosition;
        public Vector3 previousWorldPosition;
        public Vector3 force;
        public float mass;
        public List<SpringConnection> connections = new List<SpringConnection>();
    }

    class SpringConnection
    {
        public Particle particle;
        public float initialDistance;

        public SpringConnection(Particle p, float distance)
        {
            particle = p;
            initialDistance = distance;
        }
    }
}
