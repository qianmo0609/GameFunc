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
        // ����һ��Э��
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

// �Զ���Э�̵�������
public class CustomCoroutineScheduler
{
    // �洢��ִ�е�Э���б�
    private List<CustomCoroutine> coroutines = new List<CustomCoroutine>();
    // ����һ��Э��
    public CustomCoroutine StartCoroutine(IEnumerator routine)
    {
        CustomCoroutine coroutine = new CustomCoroutine(routine);
        coroutines.Add(coroutine);
        return coroutine;
    }
    // ����Э�̵���������Ҫ��ÿһ֡����
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
// �Զ���Э����
public class CustomCoroutine
{
    private IEnumerator routine;
    private YieldInstruction currentYield;
    public CustomCoroutine(IEnumerator routine)
    {
        this.routine = routine;
    }

    public IEnumerator Routine { get => routine;}

    // �ƶ���Э�̵���һ��״̬
    public bool MoveNext()
    {
        if (currentYield != null)
        {
            if (!currentYield.IsDone())
            {
                // ��ǰ YieldInstruction δ��ɣ������ȴ�
                return true;
            }
            // ��ǰ YieldInstruction ��ɣ�����
            currentYield = null;
        }
        if (routine.MoveNext())
        {
            currentYield = routine.Current as YieldInstruction;
            return true;
        }
        // Э��ִ�����
        return false;
    }
}

// �Զ��� YieldInstruction ����
public abstract class YieldInstruction
{
    public abstract bool IsDone();
}

// �Զ���ȴ�ָ��֡���� YieldInstruction
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

