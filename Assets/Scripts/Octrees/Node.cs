using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

    public List<Edge> edgeList = new List<Edge>();
    public Node path = null;
    public OctreeNode octreeNode;
    public float f, g, h;
    public Node cameFrom;

    public Node(OctreeNode n) {

        octreeNode = n;
        path = null;
    }

    public OctreeNode getNode() {

        return octreeNode;
    }
}
