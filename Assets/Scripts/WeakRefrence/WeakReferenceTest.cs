using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakReferenceTest : MonoBehaviour
{
    WeakReference weakRef;

    WeakReference<TestWeakReferenceObj> weakRefGeneric;
    // Start is called before the first frame update
    void Start()
    {
        //// 创建一个对象
        //var myObject = new TestWeakReferenceObj();

        //// 创建弱引用
        //weakRef = new WeakReference(myObject);

        //// 解除强引用，只保留弱引用
        //myObject = null;

        //// 通过弱引用访问对象
        //if (weakRef.IsAlive)
        //{
        //    var obj = weakRef.Target as TestWeakReferenceObj;
        //    obj.DoSomething();
        //}
        //else
        //{
        //    Debug.Log("对象已被回收");
        //}

        weakRefGeneric = new WeakReference<TestWeakReferenceObj>(new TestWeakReferenceObj());

        if (weakRefGeneric.TryGetTarget(out TestWeakReferenceObj obj))
        {
            obj.DoSomething();
        }
        else
        {
            Debug.Log("对象已被回收");
        }
    }

    // Update is called once per frame
    void Update()
    {
        ////通过弱引用访问对象
        //if (weakRef.IsAlive)
        //{
        //    var obj = weakRef.Target as TestWeakReferenceObj;
        //    obj.DoSomething();
        //}
        //else
        //{
        //    Debug.Log("对象已被回收");
        //}

        if (weakRefGeneric.TryGetTarget(out TestWeakReferenceObj obj))
        {
            obj.DoSomething();
        }
        else
        {
            Debug.Log("对象已被回收");
        }
    }
}

class TestWeakReferenceObj { 
    public void DoSomething()
    {
        Debug.Log("对象没有被回收");
    }
}