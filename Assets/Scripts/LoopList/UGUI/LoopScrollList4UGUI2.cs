using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LoopScrollList4UGUI2 : MonoBehaviour
{
    // ������ͼ���
    public ScrollRect scrollRect;
    // �����������������б���
    public RectTransform content;
    // �б����Ԥ����
    public GameObject itemPrefab;
    // �����б������б�����ʾ������Ϊ����
    public List<int> dataList = new List<int>();
    // �б���ĸ߶�
    public float itemHeight;
    // �ɼ������ڿ����ɵ�����б�������
    private int visibleItemCount;
    // ��ǰ��ʾ���б���
    private List<GameObject> visibleItems = new List<GameObject>();
    // �б������أ����ڸ���
    private Queue<GameObject> itemPool = new Queue<GameObject>();

    void Start()
    {
        // ��ʼ�������б����������һЩ����
        for (int i = 0; i < 100; i++)
        {
            dataList.Add(i);
        }

        // ����ɼ������ڿ����ɵ�����б�������
        visibleItemCount = Mathf.FloorToInt(scrollRect.viewport.rect.height / itemHeight) + 1;

        // ��ʼ����ʾ���б���
        InitializeVisibleItems();

        // ���������¼�
        scrollRect.onValueChanged.AddListener(OnScroll);
    }

    // ��ʼ����ʾ���б���
    void InitializeVisibleItems()
    {
        for (int i = 0; i < visibleItemCount; i++)
        {
            GameObject item = GetItemFromPool();
            item.transform.SetParent(content, false);
            item.SetActive(true);
            UpdateItem(item, i);
            visibleItems.Add(item);
        }
    }

    // �Ӷ�����л�ȡ�б������Ϊ���򴴽��µ�
    GameObject GetItemFromPool()
    {
        if (itemPool.Count > 0)
        {
            return itemPool.Dequeue();
        }
        else
        {
            return Instantiate(itemPrefab);
        }
    }

    // �����б������ʾ����
    void UpdateItem(GameObject item, int index)
    {
        if (index < dataList.Count)
        {
            Text text = item.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = "Item " + dataList[index].ToString();
            }
        }
    }

    // �����¼�������
    void OnScroll(Vector2 scrollPosition)
    {
        // ���㵱ǰ��ʾ�������ʼ����
        int startIndex = Mathf.FloorToInt(-content.anchoredPosition.y / itemHeight);

        // �������Ϲ���
        if (scrollPosition.y > 0 && startIndex > 0)
        {
            while (startIndex > 0)
            {
                MoveLastItemToTop();
                startIndex--;
            }
        }
        // �������¹���
        else if (scrollPosition.y < 0 && startIndex + visibleItemCount < dataList.Count)
        {
            while (startIndex + visibleItemCount < dataList.Count)
            {
                MoveFirstItemToBottom();
                startIndex++;
            }
        }
    }

    // �����һ���б����ƶ�������
    void MoveLastItemToTop()
    {
        GameObject lastItem = visibleItems[visibleItems.Count - 1];
        visibleItems.RemoveAt(visibleItems.Count - 1);
        visibleItems.Insert(0, lastItem);

        int newIndex = GetFirstVisibleIndex() - 1;
        if (newIndex < 0)
        {
            newIndex = dataList.Count - 1;
        }

        lastItem.transform.SetSiblingIndex(0);
        UpdateItem(lastItem, newIndex);
    }

    // ����һ���б����ƶ����ײ�
    void MoveFirstItemToBottom()
    {
        GameObject firstItem = visibleItems[0];
        visibleItems.RemoveAt(0);
        visibleItems.Add(firstItem);

        int newIndex = GetLastVisibleIndex() + 1;
        if (newIndex >= dataList.Count)
        {
            newIndex = 0;
        }

        firstItem.transform.SetSiblingIndex(visibleItems.Count - 1);
        UpdateItem(firstItem, newIndex);
    }

    // ��ȡ��һ���ɼ��б��������
    int GetFirstVisibleIndex()
    {
        return Mathf.FloorToInt(-content.anchoredPosition.y / itemHeight);
    }

    // ��ȡ���һ���ɼ��б��������
    int GetLastVisibleIndex()
    {
        return GetFirstVisibleIndex() + visibleItemCount - 1;
    }

    // ���б���ٿɼ�ʱ�������������
    void RecycleItem(GameObject item)
    {
        item.SetActive(false);
        itemPool.Enqueue(item);
    }
}
