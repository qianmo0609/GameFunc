using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //MyProcessor processor = new MyProcessor();

        //// �����¼��������Զ����߼�
        //processor.BeforeProcess += (sender, e) =>
        //{
        //    Debug.Log("Before process: Custom logic executed.");
        //};

        //processor.AfterProcess += (sender, e) =>
        //{
        //    Debug.Log("After process: Custom logic executed.");
        //};

        //processor.Process();

        // ����ħ��ʦ��ɫ
        GameCharacter mage = new Mage();
        mage.Attack();

        Console.WriteLine();

        // ����սʿ��ɫ
        GameCharacter warrior = new Warrior();
        warrior.Attack();
    }
}

// ����һ���������ӵ���
public class MyProcessor
{
    // �����¼�����Ϊ����
    public event EventHandler BeforeProcess;
    public event EventHandler AfterProcess;

    public void Process()
    {
        // �ڴ���ǰ�����¼�
        BeforeProcess?.Invoke(this, EventArgs.Empty);

        Debug.Log("Processing...");

        // �ڴ���󴥷��¼�
        AfterProcess?.Invoke(this, EventArgs.Empty);
    }
}


// ���ࣺ��Ϸ��ɫ
public class GameCharacter
{
    // �鷽������Ϊ����
    public virtual void SpecialAbility()
    {
        Debug.Log("No special ability.");
    }

    // ��ɫ�Ĺ�������������ù��ӷ���
    public void Attack()
    {
        Debug.Log("Character attacks!");
        SpecialAbility();
    }
}

// �����ࣺħ��ʦ
public class Mage : GameCharacter
{
    // ��д�鷽��������Զ����߼�
    public override void SpecialAbility()
    {
        Debug.Log("Mage casts a fireball!");
    }
}

// �����ࣺսʿ
public class Warrior : GameCharacter
{
    // ��д�鷽��������Զ����߼�
    public override void SpecialAbility()
    {
        Debug.Log("Warrior uses a powerful shield bash!");
    }
}
