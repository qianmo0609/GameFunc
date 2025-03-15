using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpinnerControl : MonoBehaviour
{
    public RectTransform imgSpine;

    public float speedMax = 1200;
    public float angularAcceleration = 200;
    float angle = 0;
    float angularVelocity = 0;

    bool isStart = false;

    int[] core = new int[] { 100, 200, 300, 400, 500, 600, 700, 800 };

    float startSpeedSlowVel = 0;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isStart = true;
        }

        if (Input.GetMouseButtonDown(1))
        {
            isStart = false;
            this.RandReward();
        }

        if (isStart)
        {
            angularVelocity = Mathf.Min(speedMax, angularVelocity + angularAcceleration * Time.deltaTime);
            angle += angularVelocity * Time.deltaTime;
            angle %= 360;
            imgSpine.rotation = Quaternion.Euler(0, 0, angle);
        }else if(!isStart && angularVelocity > 0)
        {
            if(startSpeedSlowVel > 0)
            {
                startSpeedSlowVel -= angularVelocity * Time.deltaTime;
            }
            else
            {
                angularVelocity = Mathf.Max(0, angularVelocity - angularAcceleration * Time.deltaTime);
                startSpeedSlowVel = 0;
            }
            angle += angularVelocity * Time.deltaTime;
            angle %= 360;
            imgSpine.rotation = Quaternion.Euler(0, 0, angle);
            if(angularVelocity <= 0f)
            {
                Debug.Log(core[Mathf.FloorToInt(angle / 45f)]);
            }
        }
    }

    void RandReward()
    {
        float num = Random.Range(0f, 360f);
        Debug.Log(Mathf.FloorToInt(num / 45));
        //停止的时候 angularVelocity有速度，所以最终停的位置是
        //v = v0 - at -> t = v0/a
        //_angle = 0.5f * a * t * t = 0.5f * v0 * v0 /a;
        //得到要从哪个角度开始减速
        float angle1 = num - (0.5f * angularVelocity * angularVelocity / angularAcceleration % 360);
        if (angle1 < 0)
        {
            angle1 += 360;
        }
        //得到当前角需要匀速转动多少角度
        startSpeedSlowVel = angle1 - angle;
        if (startSpeedSlowVel < 0)
        {
            startSpeedSlowVel += 360;
        }
    }
}
