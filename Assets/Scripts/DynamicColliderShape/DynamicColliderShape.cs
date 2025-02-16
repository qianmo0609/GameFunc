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
                    // 这里可以根据需求选择不同的算法来调整顶点位置
                    // 简单示例：将最近的顶点移向鼠标点击位置
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

                    // 平滑地移动顶点
                    vertices[closestVertexIndex] = transform.InverseTransformPoint(hit.point);//Vector3.Lerp(vertices[closestVertexIndex],
                        //transform.InverseTransformPoint(hit.point), 0.5f);
                    mesh.vertices = vertices;
                    mesh.RecalculateNormals();
                    meshCollider.sharedMesh = mesh;
                }
            }
        }
    }
    // 绘制辅助线的函数
    void OnDrawGizmos()
    {
        // 确保Mesh Collider组件存在
        if (meshCollider != null)
        {
            // 获取Mesh Collider的共享Mesh
            Mesh mesh = meshCollider.sharedMesh;
            // 确保Mesh存在
            if (mesh != null)
            {
                // 获取Mesh的顶点数组
                Vector3[] vertices = mesh.vertices;
                // 获取Mesh的三角形索引数组
                int[] triangles = mesh.triangles;

                // 设置绘制颜色为红色
                Gizmos.color = Color.red;

                // 遍历三角形索引数组，以三个为一组绘制三角形
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    // 获取三角形的三个顶点索引
                    int index1 = triangles[i];
                    int index2 = triangles[i + 1];
                    int index3 = triangles[i + 2];

                    // 获取三角形的三个顶点世界坐标
                    Vector3 vertex1 = transform.TransformPoint(vertices[index1]);
                    Vector3 vertex2 = transform.TransformPoint(vertices[index2]);
                    Vector3 vertex3 = transform.TransformPoint(vertices[index3]);

                    // 绘制三角形的三条边
                    Gizmos.DrawLine(vertex1, vertex2);
                    Gizmos.DrawLine(vertex2, vertex3);
                    Gizmos.DrawLine(vertex3, vertex1);
                }
            }
        }
    }
}
