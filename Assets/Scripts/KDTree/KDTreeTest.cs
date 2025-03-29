using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KDTreeTest : MonoBehaviour
{
    public int objNum = 0;
    public GameObject obj;

    KDTree kDTree;
    List<Vector3> objPos;
    Dictionary<Vector3, MeshRenderer> gameObjDic;
    LayerMask layerAsLayerMask;

    MeshRenderer selectGoMR;
    MaterialPropertyBlock materialPropertyBlock;

    void Start()
    {
        objPos = new List<Vector3>();
        gameObjDic = new Dictionary<Vector3, MeshRenderer>();
        this.OnCreateDisplayObj();
        kDTree = new KDTree(objPos);
        layerAsLayerMask = 1 << LayerMask.NameToLayer("DetectObj");
        materialPropertyBlock = new MaterialPropertyBlock();
    }

    void OnCreateDisplayObj()
    {
        GameObject g;   
        Vector3 p;
        for (int i = 0; i < objNum; i++) { 
            g = Instantiate(obj);
            p = new Vector3 (Random.Range(-10.0f,10.0f),0, Random.Range(-10.0f, 10.0f));
            g.transform.position = p;
            objPos.Add(p);
            gameObjDic.Add(p, g.GetComponent<MeshRenderer>());
        }
    }

    void OnQueryObj(Vector3 tarPos)
    {
        KDTreeNode nearest = null;
        kDTree.NearestNeighborSearch(tarPos,kDTree.ROOT,0,ref nearest,float.MaxValue);
        if(selectGoMR != null)
        {
            this.OnSetColor(selectGoMR,Color.white);
        }
        selectGoMR = gameObjDic[nearest.Point];
        this.OnSetColor(selectGoMR,Color.red);
        //Debug.Log($"{selectGoMR},{nearest.Point}");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, 1000, layerAsLayerMask, QueryTriggerInteraction.Collide))
            {
                this.OnQueryObj(hit.point);
            }
        }
    }

    void OnSetColor(MeshRenderer mr,Color color)
    {
        mr.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetColor("_Color", color);
        mr.SetPropertyBlock(materialPropertyBlock);
    }

}
