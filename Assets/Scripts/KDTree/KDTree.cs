using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class KDTree
{
    private KDTreeNode root;

    // ¹¹½¨ KD Ê÷
    public KDTreeNode BuildKDTree(List<double[]> points, int depth = 0)
    {
        if (points.Count == 0)
            return null;

        int dimension = points[0].Length;
        int splitDimension = depth % dimension;

        points.Sort((a, b) => a[splitDimension].CompareTo(b[splitDimension]));

        int medianIndex = points.Count / 2;
        double[] medianPoint = points[medianIndex];

        root = new KDTreeNode(medianPoint);
        root.SplitDimension = splitDimension;

        List<double[]> leftPoints = points.GetRange(0, medianIndex);
        List<double[]> rightPoints = points.GetRange(medianIndex + 1, points.Count - medianIndex - 1);

        root.Left = BuildKDTree(leftPoints, depth + 1);
        root.Right = BuildKDTree(rightPoints, depth + 1);

        return root;
    }

    // ËÑË÷ KD Ê÷
    public KDTreeNode SearchKDTree(double[] targetPoint, KDTreeNode currentNode, int depth = 0)
    {
        if (currentNode == null)
            return null;

        int dimension = targetPoint.Length;
        int splitDimension = depth % dimension;

        if (targetPoint[splitDimension] < currentNode.Point[splitDimension])
            return SearchKDTree(targetPoint, currentNode.Left, depth + 1);
        else
            return SearchKDTree(targetPoint, currentNode.Right, depth + 1);
    }
}
