using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class JobSystemTest : MonoBehaviour
{
    NativeArray<Vector3> result;
    JobHandle handle;

    JobHandle firstHandle;

    public struct MyJob : IJob
    {
        public float a;
        public float b;
        public NativeArray<Vector3> result;

        public void Execute()
        {
            for (int i = 0; i< 100; i++)
            {
                a += 1;
                b += 1;
            }
            result[0] = new Vector3(a + 100, 0, b + 100);
        }
    }

    // 将一个值加一的作业
    public struct AddOneJob : IJob
    {
        public NativeArray<Vector3> result;

        public void Execute()
        {
            result[0] = result[0] + Vector3.one;
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

    void Update()
    {
        result = new NativeArray<Vector3>(1, Allocator.TempJob);

        MyJob jobData = new MyJob
        {
            a = 10,
            b = 10,
            result = result
        };

        firstHandle = jobData.Schedule();

        AddOneJob incJobData = new AddOneJob();
        incJobData.result = result;

        // 调度作业 #2
        handle = incJobData.Schedule(firstHandle);
    }

    private void LateUpdate()
    {
        handle.Complete();
        Debug.Log(result[0]);
        result.Dispose();   
    }

    //public NativeArray<Vector3> result;
    //public NativeSlice<Vector3> result1;
    //public NativeBitArray a3;
    //public NativeKeyValueArrays<int,Vector3> a4;
    //public NativeList<Vector3> a5;
    //public NativeParallelHashMap<int,Vector3> a8;
    //public NativeParallelHashSet<Vector3> a9;
    //public NativeParallelMultiHashMap<int,Vector3> a10;
    //public NativeParallelMultiHashMapIterator<int> a11;
    //public NativeQueue<int> a12;

    //public NativeHashMap<int, Vector3> A1;
    //public NativeHashSet<Vector3> a2;
    //public NativeMultiHashMap<int, Vector3> a6;
    //public NativeMultiHashMapIterator<int> a7;

}
