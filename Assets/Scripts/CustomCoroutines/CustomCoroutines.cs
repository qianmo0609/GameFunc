using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCoroutines: MonoBehaviour
{
    // 自定义协程函数
    public static IEnumerator MyCoroutine()
    {
        Console.WriteLine("协程开始");
        // 暂停2秒
        yield return new WaitForSeconds(2);
        Console.WriteLine("2秒后继续执行");

    }

    static void Main()
    {
        // 创建协程实例
        IEnumerator coroutine = MyCoroutine();
        // 手动驱动协程执行
        while (coroutine.MoveNext())
        {
            // 如果返回值是WaitForSeconds类型，说明需要等待
            if (coroutine.Current is WaitForSeconds waitForSeconds)
            {
                // 模拟等待时间
                System.Threading.Thread.Sleep((int)(waitForSeconds.Seconds * 1000));
            }
        }
        Console.WriteLine("协程结束");
    }
}

// 辅助类，用于模拟等待时间
class WaitForSeconds
{
    public float Seconds { get; }

    public WaitForSeconds(float seconds)
    {
        Seconds = seconds;
    }
}
