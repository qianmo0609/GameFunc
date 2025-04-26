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
        //// ����һ������
        //var myObject = new TestWeakReferenceObj();

        //// ����������
        //weakRef = new WeakReference(myObject);

        //// ���ǿ���ã�ֻ����������
        //myObject = null;

        //// ͨ�������÷��ʶ���
        //if (weakRef.IsAlive)
        //{
        //    var obj = weakRef.Target as TestWeakReferenceObj;
        //    obj.DoSomething();
        //}
        //else
        //{
        //    Debug.Log("�����ѱ�����");
        //}

        weakRefGeneric = new WeakReference<TestWeakReferenceObj>(new TestWeakReferenceObj());

        if (weakRefGeneric.TryGetTarget(out TestWeakReferenceObj obj))
        {
            obj.DoSomething();
        }
        else
        {
            Debug.Log("�����ѱ�����");
        }
    }

    // Update is called once per frame
    void Update()
    {
        ////ͨ�������÷��ʶ���
        //if (weakRef.IsAlive)
        //{
        //    var obj = weakRef.Target as TestWeakReferenceObj;
        //    obj.DoSomething();
        //}
        //else
        //{
        //    Debug.Log("�����ѱ�����");
        //}

        if (weakRefGeneric.TryGetTarget(out TestWeakReferenceObj obj))
        {
            obj.DoSomething();
        }
        else
        {
            Debug.Log("�����ѱ�����");
        }
    }
}

class TestWeakReferenceObj { 
    public void DoSomething()
    {
        Debug.Log("����û�б�����");
    }
}