using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class ParallelJobTest : MonoBehaviour
{
    NativeArray<float> a;
    NativeArray<float> b;
    NativeArray<float> result;
    JobHandle handle;

    // Update is called once per frame
    void Update()
    {
        a = new NativeArray<float>(3, Allocator.TempJob)
        {
            [0] = 1,
            [1] = 2,
            [2] = 3
        };
        b = new NativeArray<float>(3, Allocator.TempJob)
        {
            [0] = 100,
            [1] = 200,
            [2] = 111
        };
        result = new NativeArray<float>(3, Allocator.TempJob);
        MyParallelJob jobData = new MyParallelJob();
        jobData.a = a;
        jobData.b = b;
        jobData.result = result;
        // Schedule the job with one Execute per index in the results array and only 1 item per processing batch
        handle = jobData.Schedule(result.Length, 1);
        // Wait for the job to complete
        handle.Complete();
        Debug.Log($"{result[0]},{result[1]},{result[2]}");
        // Free the memory allocated by the arrays
        a.Dispose();
        b.Dispose();
        result.Dispose();
    }
}

public struct MyParallelJob : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<float> a;
    [ReadOnly]
    public NativeArray<float> b;
    public NativeArray<float> result;

    public void Execute(int i)
    {
        result[i] = a[i] + b[i];
    }
}
