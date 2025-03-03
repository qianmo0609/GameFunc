using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// ѭ���б������
public class LoopScrollList4UGUI : MonoBehaviour
{
    // ������ͼ���
    public ScrollRect scrollRect;
    // �����������������б���
    public RectTransform content;
    // �б����Ԥ����
    public GameObject itemPrefab;
    // �б��������
    public int itemCount;
    // �б���ĸ߶�
    public float itemHeight;
    // �ɼ������ڿ����ɵ�����б�������
    private int visibleItemCount;
    // ��ǰ��ʾ���б���
    private List<GameObject> visibleItems = new List<GameObject>();
    // �����б�
    private List<int> dataList = new List<int>();
    // ��һ�ι���λ��
    private float lastScrollPosition;

    void Start()
    {
        // ��ʼ�������б�
        for (int i = 0; i < itemCount; i++)
        {
            dataList.Add(i);
        }

        // ����ɼ������ڿ����ɵ�����б�������
        visibleItemCount = Mathf.FloorToInt(scrollRect.viewport.rect.height / itemHeight);

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
            GameObject item = Instantiate(itemPrefab, content);
            item.transform.localPosition = new Vector3(0, -i * itemHeight, 0);
            UpdateItem(item, i);
            visibleItems.Add(item);
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
        Debug.Log(content.anchoredPosition.y);
        //float currentScrollPosition = scrollRect.verticalNormalizedPosition;
        //if (currentScrollPosition > lastScrollPosition)
        //{
        //    // ���Ϲ���
        //    while (content.anchoredPosition.y > itemHeight)
        //    {
        //        MoveLastItemToTop();
        //        content.anchoredPosition -= new Vector2(0, itemHeight);
        //    }
        //}
        //else if (currentScrollPosition < lastScrollPosition)
        //{
        //    // ���¹���
        //    while (content.anchoredPosition.y < -itemHeight)
        //    {
        //        MoveFirstItemToBottom();
        //        content.anchoredPosition += new Vector2(0, itemHeight);
        //    }
        //}
        //lastScrollPosition = currentScrollPosition;
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
        lastItem.transform.localPosition = new Vector3(0, 0, 0);
        UpdateItem(lastItem, newIndex);
        for (int i = 1; i < visibleItems.Count; i++)
        {
            visibleItems[i].transform.localPosition -= new Vector3(0, itemHeight, 0);
        }
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
        firstItem.transform.localPosition = new Vector3(0, -visibleItemCount * itemHeight, 0);
        UpdateItem(firstItem, newIndex);
        for (int i = 0; i < visibleItems.Count - 1; i++)
        {
            visibleItems[i].transform.localPosition += new Vector3(0, itemHeight, 0);
        }
    }

    // ��ȡ��һ���ɼ��б��������
    int GetFirstVisibleIndex()
    {
        float offset = content.anchoredPosition.y / itemHeight;
        int index = Mathf.FloorToInt(offset);
        return index;
    }

    // ��ȡ���һ���ɼ��б��������
    int GetLastVisibleIndex()
    {
        return GetFirstVisibleIndex() + visibleItemCount - 1;
    }
}
