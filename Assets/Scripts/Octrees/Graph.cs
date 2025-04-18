using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph {

    public List<Edge> edges = new List<Edge>();
    public List<Node> nodes = new List<Node>();

    public Graph() {

    }

    public void AddNode(OctreeNode otn) {

        if (findNode(otn.id) == null) {

            Node node = new Node(otn);
            nodes.Add(node);
        }
    }

    public void AddEdge(OctreeNode fromNode, OctreeNode toNode) {

        Node from = findNode(fromNode.id);
        Node to = findNode(toNode.id);

        if (from != null && to != null) {

            Edge e = new Edge(from, to);
            edges.Add(e);
            from.edgeList.Add(e);
            Edge f = new Edge(to, from);
            edges.Add(f);
            to.edgeList.Add(f);
        }
    }

    public Node findNode(int otn_id) {

        foreach (Node n in nodes) {

            if (n.getNode().id == otn_id) {

                return n;
            }
        }
        return null;
    }


    public void Draw() {

        for (int i = 0; i < edges.Count; ++i) {

            Debug.DrawLine(edges[i].startNode.octreeNode.nodeBounds.center,
                edges[i].endNode.octreeNode.nodeBounds.center, Color.red);
        }

        for (int i = 0; i < nodes.Count; ++i) {

            Gizmos.color = new Color(1.0f, 1.0f, 0.0f);
            Gizmos.DrawWireSphere(nodes[i].octreeNode.nodeBounds.center, 0.25f);
        }
    }

    public bool AStar(OctreeNode startNode, OctreeNode endNode, List<Node> pathList) {

        pathList.Clear();
        Node start = findNode(startNode.id);
        Node end = findNode(endNode.id);

        if (start == null || end == null) return false;

        List<Node> open = new List<Node>();
        List<Node> closed = new List<Node>();

        float tentative_g_score = 0.0f;
        bool tentative_is_better;

        start.g = 0.0f;
        start.h = Vector3.SqrMagnitude(startNode.nodeBounds.center - endNode.nodeBounds.center);
        start.f = start.h;

        open.Add(start);

        while (open.Count > 0) {

            int i = lowestF(open);
            Node thisNode = open[i];

            if (thisNode.octreeNode.id == endNode.id) {

                reconstructPath(start, end, pathList);
                return true;
            }
            open.RemoveAt(i);
            closed.Add(thisNode);

            Node neighbour;

            foreach (Edge e in thisNode.edgeList) {

                neighbour = e.endNode;
                neighbour.g = thisNode.g + Vector3.SqrMagnitude(thisNode.octreeNode.nodeBounds.center - neighbour.octreeNode.nodeBounds.center);

                if (closed.IndexOf(neighbour) > -1) continue;

                tentative_g_score = thisNode.g + Vector3.SqrMagnitude(thisNode.octreeNode.nodeBounds.center - neighbour.octreeNode.nodeBounds.center);

                if (open.IndexOf(neighbour) == -1) {

                    open.Add(neighbour);
                    tentative_is_better = true;
                } else if (tentative_g_score < neighbour.g) {

                    tentative_is_better = true;
                } else {

                    tentative_is_better = false;
                }

                if (tentative_is_better) {

                    neighbour.cameFrom = thisNode;
                    neighbour.g = tentative_g_score;
                    neighbour.h = Vector3.SqrMagnitude(thisNode.octreeNode.nodeBounds.center - neighbour.octreeNode.nodeBounds.center);

                    neighbour.f = neighbour.g + neighbour.h;
                }
            }
        }
        return false;
    }

    private void reconstructPath(Node startId, Node endId, List<Node> pathList) {

        pathList.Clear();
        pathList.Add(endId);
        var p = endId.cameFrom;

        while (p != startId && p != null) {

            pathList.Insert(0, p);
            p = p.cameFrom;
        }
        pathList.Insert(0, startId);
    }

    private int lowestF(List<Node> l) {

        float lowestf = 0.0f;
        int count = 0;
        int iteratorCount = 0;
        for (int i = 0; i < l.Count; ++i) {

            if (i == 0) {

                lowestf = l[i].f;
                iteratorCount = count;
            } else if (l[i].f <= lowestf) {

                lowestf = l[i].f;
                iteratorCount = count;
            }
            count++;
        }
        return iteratorCount;
    }
}
