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
        //得到当前设备的宽高比
        float windowAspect = (float)Screen.width / Screen.height;
        //将当前宽高比与设计宽高比相除，得到要放缩的倍数
        float scaleHeight = windowAspect / targetAspectRatio;

        //如果小于1，则代表当前屏幕的高度是高于设计高度的，需要在高度的两侧增加黑边
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
