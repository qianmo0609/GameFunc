using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLife : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("Awake...");
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable...");
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start...");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Update...");
    }

    private void FixedUpdate()
    {
        Debug.Log("FixUpdate...");
    }

    private void LateUpdate()
    {
        Debug.Log("LateUpdate...");
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable...");
    }

    private void OnDestroy()
    {
        Debug.Log("OnDestroy...");
    }
}
