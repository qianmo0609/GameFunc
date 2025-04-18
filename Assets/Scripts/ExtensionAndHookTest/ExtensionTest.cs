using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtensionTest : MonoBehaviour
{
    void Start()
    {
        MyClass obj = new MyClass(5);
        // ���������ʵ������һ��������չ����
        int result = obj.DoubleValue();
        Debug.Log($"Double value: {result}");
    }
}

// ����һ���򵥵���
public class MyClass
{
    public int Value { get; set; }

    public MyClass(int value)
    {
        Value = value;
    }
}

// ��չ�������붨���ھ�̬����
public static class MyClassExtensions
{
    // ��չ���������Ǿ�̬�������ҵ�һ������ʹ�� this �ؼ���ָ��Ҫ��չ������
    public static int DoubleValue(this MyClass myClass)
    {
        return myClass.Value * 2;
    }
}
