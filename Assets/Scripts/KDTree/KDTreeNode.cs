using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KDTreeNode
{
    public double[] Point { get; set; }   //�洢��Χ�ڵ����е�
    public KDTreeNode Left { get; set; }  //���ӽڵ�
    public KDTreeNode Right { get; set; } //���ӽڵ�
    public int SplitDimension { get; set; }

    public KDTreeNode(double[] point)
    {
        Point = point;
    }
}

