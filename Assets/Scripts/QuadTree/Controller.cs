using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public GameObject plane;
    public GameObject[] testObjs;

    QuadTree quadTree;
    void Start()
    {
        quadTree = new QuadTree(0, plane.GetComponent<BoxCollider>().bounds);
        test();
    }

    void test()
    {
        quadTree.clear();
        for (int i = 0; i < testObjs.Length; i++)
        {
            quadTree.insert(new QuadObj(testObjs[i], testObjs[i].GetComponent<BoxCollider>().bounds));
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (quadTree != null)
        {
            quadTree.DrawBounds();
        }
    }
#endif
}
