using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatio : MonoBehaviour
{
    public float targetAspectRatio = 16f / 9f; // The desired aspect ratio, e.g., 16:9
    private Camera _camera;


    void Start()
    {
        _camera = GetComponent<Camera>();

    }

    void SetCameraAspect()
    {
        //�õ���ǰ�豸�Ŀ�߱�
        float windowAspect = (float)Screen.width / Screen.height;
        //����ǰ��߱�����ƿ�߱�������õ�Ҫ�����ı���
        float scaleHeight = windowAspect / targetAspectRatio;

        //���С��1�������ǰ��Ļ�ĸ߶��Ǹ�����Ƹ߶ȵģ���Ҫ�ڸ߶ȵ��������Ӻڱ�
        if (scaleHeight < 1.0f)
        {
            Rect rect = _camera.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            _camera.rect = rect;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = _camera.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            _camera.rect = rect;
        }
    }

    private void Update()
    {
        SetCameraAspect();

    }
}
