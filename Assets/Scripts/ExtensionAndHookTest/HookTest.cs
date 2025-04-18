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

        //// 订阅事件，插入自定义逻辑
        //processor.BeforeProcess += (sender, e) =>
        //{
        //    Debug.Log("Before process: Custom logic executed.");
        //};

        //processor.AfterProcess += (sender, e) =>
        //{
        //    Debug.Log("After process: Custom logic executed.");
        //};

        //processor.Process();

        // 创建魔法师角色
        GameCharacter mage = new Mage();
        mage.Attack();

        Console.WriteLine();

        // 创建战士角色
        GameCharacter warrior = new Warrior();
        warrior.Attack();
    }
}

// 定义一个包含钩子的类
public class MyProcessor
{
    // 定义事件，作为钩子
    public event EventHandler BeforeProcess;
    public event EventHandler AfterProcess;

    public void Process()
    {
        // 在处理前触发事件
        BeforeProcess?.Invoke(this, EventArgs.Empty);

        Debug.Log("Processing...");

        // 在处理后触发事件
        AfterProcess?.Invoke(this, EventArgs.Empty);
    }
}


// 基类：游戏角色
public class GameCharacter
{
    // 虚方法，作为钩子
    public virtual void SpecialAbility()
    {
        Debug.Log("No special ability.");
    }

    // 角色的攻击方法，会调用钩子方法
    public void Attack()
    {
        Debug.Log("Character attacks!");
        SpecialAbility();
    }
}

// 派生类：魔法师
public class Mage : GameCharacter
{
    // 重写虚方法，添加自定义逻辑
    public override void SpecialAbility()
    {
        Debug.Log("Mage casts a fireball!");
    }
}

// 派生类：战士
public class Warrior : GameCharacter
{
    // 重写虚方法，添加自定义逻辑
    public override void SpecialAbility()
    {
        Debug.Log("Warrior uses a powerful shield bash!");
    }
}
