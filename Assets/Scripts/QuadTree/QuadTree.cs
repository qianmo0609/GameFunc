using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public struct QuadObj
{
    public GameObject obj;
    public Bounds bounds;
    public QuadObj(GameObject obj,Bounds bounds)
    {
        this.obj = obj;
        this.bounds = bounds;
    }
}

public class QuadTree
{
    private int MAX_OBJECTS = 10;
    private int MAX_LEVELS = 5;
    private int level;
    private List<QuadObj> objects;
    private Bounds bounds;
    private QuadTree[] nodes;

    public QuadTree(int pLevel, Bounds pBounds)
    {
        level = pLevel;
        objects = new List<QuadObj>();
        bounds = pBounds;
        nodes = new QuadTree[4];
    }

    public void clear()
    {
        objects.Clear();
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i] != null)
            {
                nodes[i].clear();
                nodes[i] = null;
            }
        }
    }

    /* 
     * ���ֽڵ�
     */
    private void split()
    {
        int subWidth = (int)this.bounds.extents.x;
        int subHeight = (int)this.bounds.extents.z;
        
        Vector3 size = new Vector3(subWidth, 1, subHeight);
        nodes[0] = new QuadTree(level + 1, new Bounds(this.bounds.center + new Vector3(subWidth / 2, 0, subHeight / 2), size));
        nodes[1] = new QuadTree(level + 1, new Bounds(this.bounds.center + new Vector3(subWidth / 2, 0, -subHeight / 2), size));
        nodes[2] = new QuadTree(level + 1, new Bounds(this.bounds.center + new Vector3(-subWidth / 2, 0, -subHeight / 2), size));
        nodes[3] = new QuadTree(level + 1, new Bounds(this.bounds.center + new Vector3(-subWidth / 2, 0, subHeight / 2), size));
    }

    /* 
     ��������뵽�Ĳ����С�����ڵ㳬��������������ֲ��������ȷ�����������ĸ��ڵ㡣-1����ζ�Ŷ�������ȫ�����ӽڵ��У�����Ǹ��ڵ��һ����
    */
    private int getIndex(Bounds pRect)
    {
         int index = -1;
         float verticalMidpoint = this.bounds.center.x;
         float horizontalMidpoint = this.bounds.center.z;
         // �����Ƿ�����ȫ����2��3����
         bool topQuadrant = (pRect.center.z < horizontalMidpoint && pRect.center.z + pRect.extents.z < horizontalMidpoint);
        // �����Ƿ�����ȫ����1���� 
        bool bottomQuadrant = (pRect.center.z > horizontalMidpoint);
         // �����Ƿ�����ȫ����3��4���� 
         if (pRect.center.x < verticalMidpoint && pRect.center.x + pRect.extents.x < verticalMidpoint)
         {
             if (topQuadrant)
             {
                 index = 2;
             }
             else if (bottomQuadrant)
             {
                 index = 3;
             }
         }
        // �����Ƿ�����ȫ����1��2���� 
        else if (pRect.center.x > verticalMidpoint)
         {
             if (topQuadrant)
             {
                 index = 1;
             }
             else if (bottomQuadrant)
             {
                 index = 0;
             }
         }
         return index;
    }

    /* 
     ��������뵽�Ĳ����С�����ڵ㳬��������������ֲ�������ж��󵽶�Ӧ�Ľڵ㡣
    */
    public void insert(QuadObj obj)
    {
        if (nodes[0] != null)
        {
            int index = getIndex(obj.bounds);
            if (index != -1)
            {
                nodes[index].insert(obj);
                return;
            }
        }
        objects.Add(obj);
        if (objects.Count > MAX_OBJECTS && level < MAX_LEVELS)
        {
            if (nodes[0] == null)
            {
                split();
            }

            int count = objects.Count;
            for (int j = count - 1; j >= 0; j--)
            {
                int index = getIndex(objects[j].bounds);
                if (index != -1)
                {
                    objects.RemoveAt(j);
                    nodes[index].insert(obj);
                }
            }            
        }
    }

    /* 
     * �������з��������Ķ���
     */
    public List<GameObject> getNearObjs(List<GameObject> returnObjects, Bounds pRect)
    {
        int index = getIndex(pRect);
        if (index != -1 && nodes[0] != null)
        {
            nodes[index].getNearObjs(returnObjects, pRect);
        }
        returnObjects.AddRange(objects);
        return returnObjects;
    }

#if UNITY_EDITOR
    public void DrawBounds()
    {
        Gizmos.color = Color.green;
        if (level == 0)
        {
            Gizmos.DrawLine(new Vector3(this.bounds.center.x - this.bounds.extents.x, 0, this.bounds.center.z - this.bounds.extents.z), new Vector3(this.bounds.center.x + this.bounds.extents.x, 0, this.bounds.center.z - this.bounds.extents.z));
            Gizmos.DrawLine(new Vector3(this.bounds.center.x - this.bounds.extents.x, 0, this.bounds.center.z + this.bounds.extents.z), new Vector3(this.bounds.center.x + this.bounds.extents.x, 0, this.bounds.center.z + this.bounds.extents.z));
            Gizmos.DrawLine(new Vector3(this.bounds.center.x - this.bounds.extents.x, 0, this.bounds.center.z - this.bounds.extents.z), new Vector3(this.bounds.center.x - this.bounds.extents.x, 0, this.bounds.center.z + this.bounds.extents.z));
            Gizmos.DrawLine(new Vector3(this.bounds.center.x + this.bounds.extents.x, 0, this.bounds.center.z - this.bounds.extents.z), new Vector3(this.bounds.center.x + this.bounds.extents.x, 0, this.bounds.center.z + this.bounds.extents.z));
        }

        if (this.nodes.Length > 0)
        {
            if (this.nodes[0] != null)
            {
                Gizmos.DrawLine(new Vector3(this.bounds.center.x - this.bounds.extents.x, 0, this.bounds.center.z), new Vector3(this.bounds.center.x + this.bounds.extents.x, 0, this.bounds.center.z));
                Gizmos.DrawLine(new Vector3(this.bounds.center.x, 0, this.bounds.center.z - this.bounds.extents.z), new Vector3(this.bounds.center.x, 0, this.bounds.center.z + this.bounds.extents.z));
            }
            
            for (int i = 0; i < this.nodes.Length; i++)
            {
                this.nodes[i]?.DrawBounds();
            }
        }
    }
#endif
}
