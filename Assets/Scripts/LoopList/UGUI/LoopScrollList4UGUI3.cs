using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LoopScrollList4UGUI3 : MonoBehaviour
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

    Transform topItem = null;  //��ϵ�һ������
    Transform bottomItem = null; //��׵�һ������

    int topIdx = -1;

    int currentDataoffSet = 0; //�����б���ʾ��һ���ʵ������Ҳ������������ʵ��һ�����Чƫ��

    float contentAnchoredPositionYMax = 0;
    float contentAnchoredPositionYMin = 0;

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
        topY = content.position.y + itemHeight / 2;//809.77f;
        bottomY = content.position.y - visibleItemCount * itemHeight - itemHeight / 2;//265.77f;
        content.sizeDelta = new Vector2(content.rect.width,dataList.Count * itemHeight);
        topItem = content.GetChild(0);
        bottomItem = content.GetChild(visibleItemCount);
        topIdx = 0;
        contentAnchoredPositionYMin = content.anchoredPosition.y;
        contentAnchoredPositionYMax = content.anchoredPosition.y + (dataList.Count - visibleItemCount)* itemHeight + itemHeight - scrollRect.viewport.rect.height % itemHeight;
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
        content.anchoredPosition = new Vector2(content.anchoredPosition.x,Mathf.Max(contentAnchoredPositionYMin, content.anchoredPosition.y));
        content.anchoredPosition = new Vector2(content.anchoredPosition.x, Mathf.Min(contentAnchoredPositionYMax,content.anchoredPosition.y));
        
        // ���Ƴ���һ��λ��ʱ�Ĵ���
        //���Ϲ���
        if (topItem.position.y >= topY)
        {
            //���������ľ�������м���itemHeight�ľ���
            int times = Mathf.CeilToInt((topItem.position.y - topY) / itemHeight);
            //�����ľ����в���һ��itemHeight�Ĳ��֣���Ҫ���ⲿ���ں��������д���
            float offset = (topItem.position.y - topY) % itemHeight;
            for (int i = 0; i < times; i++)
            {
                topItem = content.GetChild(topIdx);
                topItem.position = new Vector3(topItem.position.x, bottomY + (times - i - 1) * itemHeight + offset, topItem.position.z);
                topIdx++;
                topIdx %= (visibleItemCount + 1);
                topItem = content.GetChild(topIdx);
                bottomItem = content.GetChild((topIdx + visibleItemCount) % (visibleItemCount + 1));
                currentDataoffSet++;
                //����currentDataoffSet������Item�ľ�����Ϣ
                UpdateItem(bottomItem.gameObject, currentDataoffSet + visibleItemCount);
            }
        }
        // ���Ƶ���һ��λ��ʱ�Ĵ���
        else if (bottomItem.position.y <= bottomY)
        {
            int times = Mathf.CeilToInt(Mathf.Abs(bottomItem.position.y - bottomY) / itemHeight);
            float offset = Mathf.Abs(bottomItem.position.y - bottomY) % itemHeight;
            for (int i = 0; i < times; i++)
            {
                bottomItem = content.GetChild((topIdx + visibleItemCount) % (visibleItemCount + 1));
                bottomItem.position = new Vector3(bottomItem.position.x, topY - (times - i - 1) * itemHeight - offset, bottomItem.position.z);
                topIdx--;
                if (topIdx < 0)
                {
                    topIdx = visibleItemCount;
                }
                topItem = content.GetChild(topIdx);
                bottomItem = content.GetChild((topIdx + visibleItemCount) % (visibleItemCount + 1));
                currentDataoffSet--;
                //����currentDataoffSet������Item�ľ�����Ϣ
                UpdateItem(topItem.gameObject, currentDataoffSet);
            }
        }
    }

    // �� Scene ��ͼ�л��Ƹ�����
    private void OnDrawGizmos()
    {
        if (content == null) return;

        // ��ȡ Content �ľֲ����귶Χ
        Vector3[] contentCorners = new Vector3[4];
        content.GetLocalCorners(contentCorners);

        // ���ֲ�����ת��Ϊ��������
        Matrix4x4 matrix = content.transform.localToWorldMatrix;
        for (int i = 0; i < contentCorners.Length; i++)
        {
            contentCorners[i] = matrix.MultiplyPoint(contentCorners[i]);
        }

        // ���� Content �ı߽��
        Gizmos.color = Color.green;
        Gizmos.DrawLine(contentCorners[0], contentCorners[1]); // �ϱ�
        Gizmos.DrawLine(contentCorners[1], contentCorners[2]); // �ұ�
        Gizmos.DrawLine(contentCorners[2], contentCorners[3]); // �±�
        Gizmos.DrawLine(contentCorners[3], contentCorners[0]); // ���

        // ���ƹ�����ֵ�ߣ��ϱ߽���±߽磩
        Vector3 upperThresholdPos = content.position + Vector3.up * itemHeight;
        Vector3 lowerThresholdPos = content.position + Vector3.down * itemHeight;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(upperThresholdPos - Vector3.right * 50, upperThresholdPos + Vector3.right * 50); // ����ֵ��
        Gizmos.DrawLine(lowerThresholdPos - Vector3.right * 50, lowerThresholdPos + Vector3.right * 50); // ����ֵ��
    }
}
