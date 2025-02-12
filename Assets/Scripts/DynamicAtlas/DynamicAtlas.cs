using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicAtlas : MonoBehaviour
{
    // ͼ���Ŀ��
    public int atlasWidth = 1024;
    // ͼ���ĸ߶�
    public int atlasHeight = 1024;
    // Ҫ�ϲ��������б�
    public List<Texture2D> texturesToMerge;
    public RawImage displayImg;
    // �������ɵ�ͼ������
    private Texture2D atlasTexture;
    // ÿ��������ͼ���е�λ�úʹ�С��Ϣ
    private List<Rect> textureRects;

    void Start()
    {
        // ��ʼ��ͼ��
        InitializeAtlas();
        // �ϲ�����
        MergeTextures();
        // ���²���
        //UpdateMaterial();
        displayImg.texture = atlasTexture;
    }

    void InitializeAtlas()
    {
        // ����һ���հ׵�ͼ������
        atlasTexture = new Texture2D(atlasWidth, atlasHeight, TextureFormat.RGBA32, false);
        // ����ɫ����
        Color[] fillColorArray = atlasTexture.GetPixels();
        for (int i = 0; i < fillColorArray.Length; ++i)
        {
            fillColorArray[i] = Color.white;
        }
        atlasTexture.SetPixels(fillColorArray);
        atlasTexture.Apply();

        textureRects = new List<Rect>();
    }

    void MergeTextures()
    {
        // ʹ��Unity��PackTextures��������������
        textureRects = new List<Rect>(atlasTexture.PackTextures(texturesToMerge.ToArray(), 2, atlasWidth));
    }

    void UpdateMaterial()
    {
        // ��ȡ��ǰ�����Renderer���
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // ��ȡ����
            Material material = renderer.material;
            // ���ò��ʵ�������Ϊͼ������
            material.mainTexture = atlasTexture;

            // ʾ������ӡÿ�������UV��Ϣ
            for (int i = 0; i < texturesToMerge.Count; i++)
            {
                Rect uvRect = textureRects[i];
                Debug.Log($"Texture {i} UV: ({uvRect.x}, {uvRect.y}, {uvRect.width}, {uvRect.height})");
            }
        }
    }
}
