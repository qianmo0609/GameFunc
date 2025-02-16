using UnityEngine;

public class DynamicColliderShape : MonoBehaviour
{
    private MeshCollider meshCollider;
    private Mesh mesh;
    private Vector3[] originalVertices;
    private Camera mainCamera;

    void Start()
    {
        meshCollider = GetComponent<MeshCollider>();
        mesh = meshCollider.sharedMesh;
        originalVertices = mesh.vertices;
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider == meshCollider)
                {
                    // ������Ը�������ѡ��ͬ���㷨����������λ��
                    // ��ʾ����������Ķ������������λ��
                    Vector3[] vertices = mesh.vertices;
                    float minDistance = float.MaxValue;
                    int closestVertexIndex = 0;
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        Vector3 worldVertex = transform.TransformPoint(vertices[i]);
                        float distance = Vector3.Distance(worldVertex, hit.point);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestVertexIndex = i;
                        }
                    }

                    // ƽ�����ƶ�����
                    vertices[closestVertexIndex] = transform.InverseTransformPoint(hit.point);//Vector3.Lerp(vertices[closestVertexIndex],
                        //transform.InverseTransformPoint(hit.point), 0.5f);
                    mesh.vertices = vertices;
                    mesh.RecalculateNormals();
                    meshCollider.sharedMesh = mesh;
                }
            }
        }
    }
    // ���Ƹ����ߵĺ���
    void OnDrawGizmos()
    {
        // ȷ��Mesh Collider�������
        if (meshCollider != null)
        {
            // ��ȡMesh Collider�Ĺ���Mesh
            Mesh mesh = meshCollider.sharedMesh;
            // ȷ��Mesh����
            if (mesh != null)
            {
                // ��ȡMesh�Ķ�������
                Vector3[] vertices = mesh.vertices;
                // ��ȡMesh����������������
                int[] triangles = mesh.triangles;

                // ���û�����ɫΪ��ɫ
                Gizmos.color = Color.red;

                // �����������������飬������Ϊһ�����������
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    // ��ȡ�����ε�������������
                    int index1 = triangles[i];
                    int index2 = triangles[i + 1];
                    int index3 = triangles[i + 2];

                    // ��ȡ�����ε�����������������
                    Vector3 vertex1 = transform.TransformPoint(vertices[index1]);
                    Vector3 vertex2 = transform.TransformPoint(vertices[index2]);
                    Vector3 vertex3 = transform.TransformPoint(vertices[index3]);

                    // ���������ε�������
                    Gizmos.DrawLine(vertex1, vertex2);
                    Gizmos.DrawLine(vertex2, vertex3);
                    Gizmos.DrawLine(vertex3, vertex1);
                }
            }
        }
    }
}
