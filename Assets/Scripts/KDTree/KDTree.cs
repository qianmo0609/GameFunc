using System.Collections.Generic;
using UnityEngine;

public class KDTree
{
    private KDTreeNode root;
    private int k = 2; //ά�ȣ�ʹ��2ά�ռ����

    public KDTreeNode ROOT { get { return root; } }

    public KDTree(List<Vector3> points)
    {
        root = BuildKDTree(points);
    }


    // ���� KD ��
    public KDTreeNode BuildKDTree(List<Vector3> points, int depth = 0)
    {
        if (points.Count == 0)
            return null;

        int splitDimension = depth % k;

        points.Sort((a, b) =>
        {
            if (splitDimension == 0) return a.x.CompareTo(b.x);
            else return a.z.CompareTo(b.z);
        });

        int medianIndex = points.Count / 2;
        KDTreeNode node = new KDTreeNode(points[medianIndex]);
        node.SplitDimension = splitDimension;

#if UNITY_EDITOR
        this.OnDrawLine(splitDimension, points[medianIndex]);
#endif

        List<Vector3> leftPoints = points.GetRange(0, medianIndex);
        List<Vector3> rightPoints = points.GetRange(medianIndex + 1, points.Count - medianIndex - 1);

        node.Left = BuildKDTree(leftPoints, depth + 1);
        node.Right = BuildKDTree(rightPoints, depth + 1);
        return node;
    }

    // ���� KD ��
    public void NearestNeighborSearch(Vector3 targetPoint, KDTreeNode currentNode, int depth, ref KDTreeNode nearest, float minDistance)
    {
        if (currentNode == null)
            return;

        int splitDimension = depth % k;
        float distance = Vector3.Distance(currentNode.Point, targetPoint);

        if (distance < minDistance)
        {
            minDistance = distance;
            nearest = currentNode;
        }

        KDTreeNode nextBranch = null;
        KDTreeNode oppositeBranch = null;

        if ((splitDimension == 0 && targetPoint.x < currentNode.Point.x) ||
            (splitDimension == 1 && targetPoint.z < currentNode.Point.z))
        {
            nextBranch = currentNode.Left;
            oppositeBranch = currentNode.Right;
        }
        else
        {
            nextBranch = currentNode.Right;
            oppositeBranch = currentNode.Left;
        }

        if (Mathf.Abs((splitDimension == 0 ? targetPoint.x : targetPoint.z) -
                     (splitDimension == 0 ? currentNode.Point.x : currentNode.Point.z)) < minDistance)
        {
            NearestNeighborSearch(targetPoint, oppositeBranch, depth + 1,ref nearest, minDistance);
        }

        NearestNeighborSearch(targetPoint,nextBranch, depth + 1, ref nearest, minDistance);
    }

    void OnDrawLine(int splitDimension,Vector3 pos)
    {
        if(splitDimension == 0)
        {
            //��x�ử��
            Debug.DrawLine(new Vector3(-100, 0, pos.z), new Vector3(100, 0, pos.z),Color.red,1000);
        }
        else
        {
            //��z�ử��
            Debug.DrawLine(new Vector3(pos.x, 0, -100), new Vector3(pos.x, 0, 100),Color.red,1000);
        }
    }
}
