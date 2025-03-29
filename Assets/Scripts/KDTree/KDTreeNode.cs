using UnityEngine;

public class KDTreeNode
{
    public Vector3 Point { get; set; }   //存储范围内的所有点
    public KDTreeNode Left { get; set; }  //左子节点
    public KDTreeNode Right { get; set; } //右子节点
    public int SplitDimension { get; set; } //拆分维度

    public KDTreeNode(Vector3 point)
    {
        Point = point;
        Left = null;
        Right = null;
    }
}

