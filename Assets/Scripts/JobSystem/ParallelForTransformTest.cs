using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class ParallelForTransformTest : MonoBehaviour
{
    [SerializeField] public Transform[] m_Transforms;
    TransformAccessArray m_AccessArray;
    // Start is called before the first frame update
    void Start()
    {
        m_AccessArray = new TransformAccessArray(m_Transforms);
    }

    // Update is called once per frame
    void Update()
    {
        var velocity = new NativeArray<Vector3>(m_Transforms.Length, Allocator.Persistent);

        for (var i = 0; i < velocity.Length; ++i)
            velocity[i] = new Vector3(0f, 10f, 0f);

        var job = new VelocityJob()
        {
            deltaTime = Time.deltaTime,
            velocity = velocity
        };

        JobHandle jobHandle = job.Schedule(m_AccessArray);
     
        jobHandle.Complete();

        Debug.Log(m_Transforms[0].position);

        velocity.Dispose();
    }

    private void OnDestroy()
    {
        m_AccessArray.Dispose();
    }
}

public struct VelocityJob : IJobParallelForTransform
{
    [ReadOnly]
    public NativeArray<Vector3> velocity;
    public float deltaTime;

    public void Execute(int index, TransformAccess transform)
    {
        var pos = transform.position;
        pos += velocity[index] * deltaTime;
        transform.position = pos;
    }
}
