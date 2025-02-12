using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomCoroutines2 : MonoBehaviour
{
    CustomCoroutineScheduler scheduler = new CustomCoroutineScheduler();

    void Start()
    {
        // 启动一个协程
        CustomCoroutine c = scheduler.StartCoroutine(TestCoroutine());
    }

    void Update()
    {
        scheduler.Update();
    }

    IEnumerator TestCoroutine()
    {
        Debug.Log("Coroutine started");
        yield return new WaitForFrames(3);
        Debug.Log("Waited for 3 frames");
        Debug.Log("Coroutine ended");
    }
}

// 自定义协程调度器类
public class CustomCoroutineScheduler
{
    // 存储待执行的协程列表
    private List<CustomCoroutine> coroutines = new List<CustomCoroutine>();
    // 启动一个协程
    public CustomCoroutine StartCoroutine(IEnumerator routine)
    {
        CustomCoroutine coroutine = new CustomCoroutine(routine);
        coroutines.Add(coroutine);
        return coroutine;
    }
    // 更新协程调度器，需要在每一帧调用
    public void Update()
    {
        for (int i = coroutines.Count - 1; i >= 0; i--)
        {
            coroutines[i].MoveNext();
        }
    }

    public void StopCoroutine(CustomCoroutine routine)
    {
        coroutines.Remove(routine);
    }

    public void StopAllCoroutine()
    {
        coroutines.Clear();
    }
}
// 自定义协程类
public class CustomCoroutine
{
    private IEnumerator routine;
    private YieldInstruction currentYield;
    public CustomCoroutine(IEnumerator routine)
    {
        this.routine = routine;
    }

    public IEnumerator Routine { get => routine;}

    // 移动到协程的下一个状态
    public bool MoveNext()
    {
        if (currentYield != null)
        {
            if (!currentYield.IsDone())
            {
                // 当前 YieldInstruction 未完成，继续等待
                return true;
            }
            // 当前 YieldInstruction 完成，重置
            currentYield = null;
        }
        if (routine.MoveNext())
        {
            currentYield = routine.Current as YieldInstruction;
            return true;
        }
        // 协程执行完毕
        return false;
    }
}

// 自定义 YieldInstruction 基类
public abstract class YieldInstruction
{
    public abstract bool IsDone();
}

// 自定义等待指定帧数的 YieldInstruction
public class WaitForFrames : YieldInstruction
{
    private int remainingFrames;
    public WaitForFrames(int frames)
    {
        remainingFrames = frames;
    }
    public override bool IsDone()
    {
        remainingFrames--;
        return remainingFrames <= 0;
    }
}

