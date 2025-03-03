using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using UnityEditorInternal.VersionControl;

public class TestLoopListNGUI : MonoBehaviour
{
    public UIScrollView scrollView;
    public UIGrid grid;
    public GameObject itemPrefab;
    public int itemCount;  // �����������
    public float itemHeight;  // �б���ĸ߶�

    private List<GameObject> visibleItems = new List<GameObject>();  // ��ǰ�ɼ����б���
    private Queue<GameObject> itemPool = new Queue<GameObject>();  // �б�������
    private List<object> dataList = new List<object>();  // �����б������� object ���Ϳɴ洢������������

    private int visibleItemCount;  // �ɼ������ڿ����ɵ�����б�������

    private Vector3 topY;
    private Vector3 bottomY;

    private bool isDrag;
    float currentPosition;
    float lastPosition;
    float startPosY;

    void Start()
    {
        UIPanel uiPanel = this.GetComponent<UIPanel>();
        //�����б��һ��λ��
        startPosY = uiPanel.height / 2 + this.transform.position.y;
        topY = grid.transform.TransformPoint(new Vector3(0, uiPanel.height / 2 + this.transform.position.y + itemHeight, 0));
        bottomY = grid.transform.TransformPoint(new Vector3(0, this.transform.position.y - uiPanel.height / 2, 0));
        isDrag = false;
        // ��ʼ�������б������ʾ��Ϊ���� 0 �� itemCount - 1
        for (int i = 0; i < itemCount; i++)
        {
            dataList.Add(i);
        }

        // ����ɼ������ڿ����ɵ�����б�������
        visibleItemCount = 5;//Mathf.FloorToInt(scrollView.panel.height / itemHeight);

        // ��ʼ����ʾ���б���
        InitializeVisibleItems();

        // ���������¼�
        scrollView.onDragStarted = onDragStart;
        scrollView.onDragFinished = onDragEnd;
    }

    void onDragStart()
    {
        isDrag = true;
    }

    void onDragEnd()
    {
        isDrag = false;
    }

    private void Update()
    {
        if (isDrag)
        {
            currentPosition = grid.transform.position.y;
            // �������Ϲ���
            if (currentPosition > lastPosition)
            {
                for (int i = 0;i < visibleItems.Count; i++)
                {
                    if (grid.transform.TransformPoint(visibleItems[i].transform.localPosition).y > topY.y)
                    {
                        visibleItems[i].transform.localPosition = new Vector3(0, visibleItems[i].transform.localPosition.y - visibleItemCount * 100, 0);
                        //�õ����һ���������ȡ���ݣ�Ȼ�����Item��ʾ
                        //GetLastVisibleIndex()
                    }
                }
            }
            // �������¹���
            else if (currentPosition < lastPosition)
            {
                for (int i = 0; i < visibleItems.Count; i++)
                {
                    if (grid.transform.TransformPoint(visibleItems[i].transform.localPosition).y < bottomY.y)
                    {
                        visibleItems[i].transform.localPosition = new Vector3(0, visibleItems[i].transform.localPosition.y + visibleItemCount * 100, 0);
                        //�õ���һ���������ȡ���ݣ�Ȼ�����Item��ʾ
                        //GetFirstVisibleIndex()
                    }
                }
            }
            lastPosition = currentPosition;
        }    
    }

    void InitializeVisibleItems()
    {
        for (int i = 0; i < visibleItemCount; i++)
        {
            GameObject item = GetItemFromPool();
            item.transform.SetParent(grid.transform, false);
            item.SetActive(true);
            item.gameObject.name = i.ToString(); 
            UpdateItem(item, i);
            visibleItems.Add(item);
        }
        grid.Reposition();
    }

    GameObject GetItemFromPool()
    {
        if (itemPool.Count > 0)
        {
            return itemPool.Dequeue();
        }
        else
        {
            return NGUITools.AddChild(grid.gameObject, itemPrefab);
        }
    }

    void UpdateItem(GameObject item, int index)
    {
        if (index < dataList.Count)
        {
            UILabel label = item.GetComponentInChildren<UILabel>();
            if (label != null)
            {
                label.text = dataList[index].ToString();
            }
        }
    }

    int GetFirstVisibleIndex()
    {
        return Mathf.FloorToInt(grid.transform.localPosition.y - startPosY / itemHeight);
    }

    int GetLastVisibleIndex()
    {
        return GetFirstVisibleIndex() + visibleItemCount - 1;
    }

    void RecycleItem(GameObject item)
    {
        item.SetActive(false);
        itemPool.Enqueue(item);
    }
}
