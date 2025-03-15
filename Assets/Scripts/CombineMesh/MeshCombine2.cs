using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeshCombine2 : MonoBehaviour
{
    public GameObject[] obj1;
    public GameObject[] obj2;

    public void Start()
    {
        this.MeshCombine();
    }

    void MeshCombine()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        List<CombineInstance> combine1 = new List<CombineInstance>(obj1.Length);
        List<CombineInstance> combine2 = new List<CombineInstance>(obj2.Length);

        for (int j = 0; j < meshFilters.Length; j++)
        {
            MeshFilter meshFilter = meshFilters[j];
            CombineInstance l_combine = new CombineInstance();
            MeshRenderer meshRender = meshFilter.GetComponent<MeshRenderer>();
            string materialName = meshRender.material.name.Replace(" (Instance)", "");
            if (materialName == "Grass")
            {
                l_combine.mesh = meshFilter.mesh;
                l_combine.transform = meshFilter.transform.localToWorldMatrix;
                combine2.Add(l_combine);
            }
            else
            {
                l_combine.mesh = meshFilter.mesh;
                l_combine.transform = meshFilter.transform.localToWorldMatrix;
                combine1.Add(l_combine);
            }
            meshFilter.gameObject.SetActive(false);
        }
        Mesh obj1Mesh = new Mesh();
        obj1Mesh.CombineMeshes(combine1.ToArray());

        Mesh obj2Mesh = new Mesh();
        obj2Mesh.CombineMeshes(combine2.ToArray());

        CombineInstance[] combine = new CombineInstance[2];

        combine[0].mesh = obj1Mesh;
        combine[0].transform = this.transform.localToWorldMatrix;
        combine[1].mesh = obj2Mesh;
        combine[1].transform = this.transform.localToWorldMatrix;

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine,false);
        transform.GetComponent<MeshFilter>().sharedMesh = mesh;
        transform.gameObject.SetActive(true);
    }
}
