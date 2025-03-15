using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector]
    public float m_decisionTime;

    public float m_hungryValue;
    [HideInInspector]
    public float m_energeValue;
    [HideInInspector]
    public bool m_isGoodHumor;

    [SerializeField]
    float speed;
    [SerializeField]
    float y;

    public float Speed { get => speed; set => speed = value; }
    public float Y { get => y; set => y = value; }
}
