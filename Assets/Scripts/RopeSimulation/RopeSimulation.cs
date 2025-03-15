using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Spring
{
    int end1;  
    int end2;

    float k;
    float d;

    float InitialLength;
}

public class Particle
{
    public float fMass;
    public Vector3 vPosition;
    public Vector3 vVelocity;
    public float fSpeed;
    public Vector3 vForce;
    public float fRadius;
    public float vGravity;
}

public class RopeSimulation : MonoBehaviour
{
    private int _NUM_OBJECTS = 10;
    private int _NUM_SPRINGS = 9;
    private int _SPRING_K = 1000;
    private int _SPRING_D = 100;
    private Spring[] springs;
    private Particle[] particles;


    // Start is called before the first frame update
    void Start()
    {
        springs = new Spring[_NUM_SPRINGS];
        particles = new Particle[_NUM_OBJECTS];
    }

    bool Initialize()
    {

        return true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
