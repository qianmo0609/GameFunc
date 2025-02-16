using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomScrollList : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform itemPrefab;
    public float itemSpacing = 10f;
    public int itemCount = 10;

    private List<RectTransform> itemList = new List<RectTransform>();
    private float totalHeight;

    void Start()
    {
        // �����ܸ߶�
        totalHeight = (itemPrefab.rect.height + itemSpacing) * itemCount - itemSpacing;
        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, totalHeight);

        // �����б���
        for (int i = 0; i < itemCount; i++)
        {
            RectTransform item = Instantiate(itemPrefab, scrollRect.content);
            item.anchoredPosition = new Vector2(0, -(item.rect.height + itemSpacing) * i);
            itemList.Add(item);
        }

        // ���������¼�
        scrollRect.onValueChanged.AddListener(OnScroll);
    }

    void OnScroll(Vector2 value)
    {
        float currentY = scrollRect.content.anchoredPosition.y;
        float minY = -totalHeight + scrollRect.viewport.rect.height;

        // ȷ��content���ᳬ���߽�
        if (currentY > 0)
        {
            scrollRect.content.anchoredPosition = new Vector2(scrollRect.content.anchoredPosition.x, 0);
            return;
        }
        else if (currentY < minY)
        {
            scrollRect.content.anchoredPosition = new Vector2(scrollRect.content.anchoredPosition.x, minY);
            return;
        }

        // �����б����λ��
        for (int i = 0; i < itemList.Count; i++)
        {
            float targetY = -(itemList[i].rect.height + itemSpacing) * i;
            float diff = targetY - itemList[i].anchoredPosition.y;

            if (i == 0 && diff > 0 && scrollRect.content.anchoredPosition.y > 0)
            {
                // ��һ��item�ƶ�����Եֹͣ
                itemList[i].anchoredPosition = new Vector2(itemList[i].anchoredPosition.x, 0);
            }
            else if (i > 0 && diff > 0)
            {
                // ����item������һ��itemһ������ֹͣ
                itemList[i].anchoredPosition = new Vector2(itemList[i].anchoredPosition.x, itemList[i - 1].anchoredPosition.y - itemList[i].rect.height - itemSpacing);
            }
            else
            {
                // ����item��β��������
                itemList[i].anchoredPosition = new Vector2(itemList[i].anchoredPosition.x, targetY);
            }
        }
    }
}
