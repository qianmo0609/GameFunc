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

    private float topY;
    private float bottomY;
    private int currentIdx;

    void Start()
    {
        // ��ʼ�������б����������һЩ����
        for (int i = 0; i < 100; i++)
        {
            dataList.Add(i);
        }

        // ����ɼ������ڿ����ɵ�����б�������
        visibleItemCount = Mathf.FloorToInt(scrollRect.viewport.rect.height / itemHeight) + 1;

        topY = content.anchoredPosition.y + itemHeight;

        currentIdx = 0;

        // ��ʼ����ʾ���б���
        InitializeVisibleItems();

        // ���������¼�
        scrollRect.onValueChanged.AddListener(OnScroll);
    }

    // ��ʼ����ʾ���б���
    void InitializeVisibleItems()
    {
        for (int i = 0; i < visibleItemCount + 1; i++)
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
        float currentY = content.anchoredPosition.y;

        // ���Ƴ���һ��λ��ʱ�Ĵ���
        // ���Ϲ���
        if (currentY > itemHeight)
        {
            Transform firstItem = content.GetChild(0);
            firstItem.SetAsLastSibling();
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, content.anchoredPosition.y - itemHeight);
        }
        // ���Ƶ���һ��λ��ʱ�Ĵ���
        else if (currentY < 0)
        {
            Transform lastItem = content.GetChild(content.childCount - 1);
            lastItem.SetAsFirstSibling();
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, content.anchoredPosition.y + itemHeight);
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
