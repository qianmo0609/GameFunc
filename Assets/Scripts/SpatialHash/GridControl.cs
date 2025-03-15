using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class GridControl : MonoBehaviour
{
    public int gridNum;
    public int gridInterval;
    public Transform objRoot;
    public Material red;
    public Material green;
    private float width;
    private float height;
    private SpatialHash spatialHash;
    private int layerAsLayerMask;

    private List<Transform> selectObjs = null;

    public void Start()
    {
        //ÒÆ³ý²ã
        layerAsLayerMask |= ~(1 << LayerMask.NameToLayer("hashObj"));
        Vector3 size = this.GetComponent<MeshFilter>().mesh.bounds.size;
        this.width = size.x * this.transform.localScale.x;
        this.height = size.z * this.transform.localScale.z;
        spatialHash = new SpatialHash(gridInterval);
        this.SpawnObj();
    }

    public Vector3 CalcGrid(Vector3 pos)
    {
        int X = Mathf.FloorToInt(pos.x / gridInterval)* gridInterval + gridInterval/2;
        int Z = Mathf.FloorToInt(pos.z / gridInterval)* gridInterval + gridInterval/2;
        return new Vector3(X,0,Z);
    }

    public void SpawnObj()
    {
        for(int i = 0; i< 100; i++)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.position = this.transform.position + new Vector3(Random.Range(-width/2,width/2),0,Random.Range(-height/2,height/2));
            obj.GetComponent<MeshRenderer>().sharedMaterial = green;
            obj.transform.localScale *= 3;
            int LayerIgnoreRaycast = LayerMask.NameToLayer("hashObj");
            obj.layer = LayerIgnoreRaycast;
            obj.transform.SetParent(objRoot);
            spatialHash.AddGameObject(CalcGrid(obj.transform.position),obj.transform);
        }
    }

    void Update()
    {
        Profiler.BeginSample("sampler_code");
        //if (Input.GetMouseButtonDown(0))
        //{
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, 1000, layerAsLayerMask , QueryTriggerInteraction.Collide))
            {
                this.UpdateObjsDisplay(green);
                
                selectObjs = spatialHash.GetGameObject(this.CalcGrid(hit.point));
                this.UpdateObjsDisplay(red);
            }
        //}
        Profiler.EndSample();
    }

    private void UpdateObjsDisplay(Material mat)
    {
        if(selectObjs != null && selectObjs.Count > 0)
        {
            for(int i = 0; i < selectObjs.Count; i++)
            {
                selectObjs[i].GetComponent<MeshRenderer>().sharedMaterial = mat;
            }
        }
    }

#if UNITY_EDITOR

    private void GrawGrid()
    {
        Vector3 startPos = this.transform.position + new Vector3(-width/2,0,height/2);
        for(int i = 0; i<= gridNum; i++)
        {
            Gizmos.DrawLine(startPos + new Vector3(i*gridInterval,0,0), startPos + new Vector3(i * gridInterval,0,-width));
        }

        for (int i = 0; i <= gridNum; i++)
        {
            Gizmos.DrawLine(startPos - new Vector3(0, 0, i * gridInterval), startPos - new Vector3(-height, 0, i * gridInterval));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        GrawGrid();
    }
#endif
}
