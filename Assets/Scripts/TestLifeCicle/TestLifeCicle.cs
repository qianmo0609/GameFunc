using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLifeCicle : MonoBehaviour
{
    public GameObject prefab;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate<GameObject>(prefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
