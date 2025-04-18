using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtensionTest : MonoBehaviour
{
    void Start()
    {
        MyClass obj = new MyClass(5);
        // 可以像调用实例方法一样调用扩展方法
        int result = obj.DoubleValue();
        Debug.Log($"Double value: {result}");
    }
}

// 定义一个简单的类
public class MyClass
{
    public int Value { get; set; }

    public MyClass(int value)
    {
        Value = value;
    }
}

// 扩展方法必须定义在静态类中
public static class MyClassExtensions
{
    // 扩展方法必须是静态方法，且第一个参数使用 this 关键字指定要扩展的类型
    public static int DoubleValue(this MyClass myClass)
    {
        return myClass.Value * 2;
    }
}
