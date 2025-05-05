using UnityEngine;

[System.Serializable]
public class Constraint
{
    public float a;
    public float b;
    public float c;
    public bool isLessThanOrEqual;

    public bool IsSatisfied(Vector2 point)
    {
        float value = a * point.x + b * point.y;
        return isLessThanOrEqual ? (value <= c) : (value >= c);
    }
}
