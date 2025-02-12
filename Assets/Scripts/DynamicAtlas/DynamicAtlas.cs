using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicAtlas : MonoBehaviour
{
    // 图集的宽度
    public int atlasWidth = 1024;
    // 图集的高度
    public int atlasHeight = 1024;
    // 要合并的纹理列表
    public List<Texture2D> texturesToMerge;
    public RawImage displayImg;
    // 最终生成的图集纹理
    private Texture2D atlasTexture;
    // 每个纹理在图集中的位置和大小信息
    private List<Rect> textureRects;

    void Start()
    {
        // 初始化图集
        InitializeAtlas();
        // 合并纹理
        MergeTextures();
        // 更新材质
        //UpdateMaterial();
        displayImg.texture = atlasTexture;
    }

    void InitializeAtlas()
    {
        // 创建一个空白的图集纹理
        atlasTexture = new Texture2D(atlasWidth, atlasHeight, TextureFormat.RGBA32, false);
        // 填充白色像素
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
        // 使用Unity的PackTextures方法进行纹理打包
        textureRects = new List<Rect>(atlasTexture.PackTextures(texturesToMerge.ToArray(), 2, atlasWidth));
    }

    void UpdateMaterial()
    {
        // 获取当前对象的Renderer组件
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // 获取材质
            Material material = renderer.material;
            // 设置材质的主纹理为图集纹理
            material.mainTexture = atlasTexture;

            // 示例：打印每个纹理的UV信息
            for (int i = 0; i < texturesToMerge.Count; i++)
            {
                Rect uvRect = textureRects[i];
                Debug.Log($"Texture {i} UV: ({uvRect.x}, {uvRect.y}, {uvRect.width}, {uvRect.height})");
            }
        }
    }
}
