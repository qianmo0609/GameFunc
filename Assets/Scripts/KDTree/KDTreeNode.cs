using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KDTreeNode
{
    public double[] Point { get; set; }   //存储范围内的所有点
    public KDTreeNode Left { get; set; }  //左子节点
    public KDTreeNode Right { get; set; } //右子节点
    public int SplitDimension { get; set; }

    public KDTreeNode(double[] point)
    {
        Point = point;
    }
}

