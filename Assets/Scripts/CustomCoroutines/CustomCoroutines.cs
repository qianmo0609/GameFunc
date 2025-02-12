using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCoroutines: MonoBehaviour
{
    // �Զ���Э�̺���
    public static IEnumerator MyCoroutine()
    {
        Console.WriteLine("Э�̿�ʼ");
        // ��ͣ2��
        yield return new WaitForSeconds(2);
        Console.WriteLine("2������ִ��");

    }

    static void Main()
    {
        // ����Э��ʵ��
        IEnumerator coroutine = MyCoroutine();
        // �ֶ�����Э��ִ��
        while (coroutine.MoveNext())
        {
            // �������ֵ��WaitForSeconds���ͣ�˵����Ҫ�ȴ�
            if (coroutine.Current is WaitForSeconds waitForSeconds)
            {
                // ģ��ȴ�ʱ��
                System.Threading.Thread.Sleep((int)(waitForSeconds.Seconds * 1000));
            }
        }
        Console.WriteLine("Э�̽���");
    }
}

// �����࣬����ģ��ȴ�ʱ��
class WaitForSeconds
{
    public float Seconds { get; }

    public WaitForSeconds(float seconds)
    {
        Seconds = seconds;
    }
}
