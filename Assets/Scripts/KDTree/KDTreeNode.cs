using UnityEngine;

public class KDTreeNode
{
    public Vector3 Point { get; set; }   //�洢��Χ�ڵ����е�
    public KDTreeNode Left { get; set; }  //���ӽڵ�
    public KDTreeNode Right { get; set; } //���ӽڵ�
    public int SplitDimension { get; set; } //���ά��

    public KDTreeNode(Vector3 point)
    {
        Point = point;
        Left = null;
        Right = null;
    }
}

