using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIDrawCallDetector : EditorWindow
{
    private Canvas targetCanvas;
    private int drawCallCount;
    private Graphic[][] drawCallGraphics = null;
    private Vector2 scrollPosition;
    private List<Texture> drawCallTextures = null;

    // 用于存储每个 DrawCall 分组的信息，键是一个唯一标识，值是该分组下的 Graphic 组件列表
    private Dictionary<string, List<Graphic>> drawCallGroups = null;

    [MenuItem("Tools/UI DrawCall Detector")]
    public static void ShowWindow()
    {
        UIDrawCallDetector window = GetWindow<UIDrawCallDetector>("UI DrawCall Detector");
        window.Show();
    }

    Texture t;
    private void OnEnable()
    {
        t = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Res/Texture/yunshi.png");
    }

   
    private void OnGUI()
    {
        EditorGUILayout.LabelField("Drag UI Canvas here:");
        targetCanvas = (Canvas)EditorGUILayout.ObjectField(targetCanvas, typeof(Canvas), true);

        if (targetCanvas != null)
        {
            if (GUILayout.Button("Detect Draw Calls"))
            {
                DetectDrawCalls(targetCanvas);
            }

            EditorGUILayout.LabelField("Draw Calls: " + drawCallCount);
            EditorGUILayout.Space(20);

            if (drawCallGraphics != null)
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                for (int i = 0; i < drawCallCount; i++)
                {
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"DrawCALL {i + 1}");
                    EditorGUI.DrawPreviewTexture(new Rect(600,10 + i * 130,50,50), drawCallTextures[i]);
                    EditorGUILayout.EndHorizontal();

                    if (drawCallGraphics[i] != null)
                    {
                        //材质信息
                        Material material = drawCallGraphics[i][0].materialForRendering;
                        EditorGUILayout.LabelField($"Material: {material.name}");

                        // 包含的 UI 元素列表
                        EditorGUILayout.LabelField("Included UI Elements:");
                        for (int j = 0; j < drawCallGraphics[i].Length; j++)
                        {
                            EditorGUILayout.LabelField(drawCallGraphics[i][j].name);
                        }
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(50);
                }
                EditorGUILayout.EndScrollView();
            }
        }
    }

    private void DetectDrawCalls(Canvas canvas)
    {
        if (targetCanvas == null)
        {
            Debug.LogError("Target Canvas is null. Cannot calculate DrawCalls.");
            return;
        }

        if(drawCallGroups == null)
        {
            drawCallGroups = new Dictionary<string, List<Graphic>>();
            drawCallTextures = new List<Texture>();
        }

        drawCallGroups.Clear();

        Graphic[] graphics = canvas.GetComponentsInChildren<Graphic>();

        // 按渲染顺序对 Graphic 组件进行排序，确保计算结果符合实际渲染顺序
        var sortedGraphics = graphics.OrderBy(g => g.transform.GetSiblingIndex()).ToList();

        drawCallCount = 0;
        drawCallGraphics = new Graphic[graphics.Length][];

        int currentDrawCallIndex = -1;
        int atlasDifferentFlag = 0;

        foreach (Graphic graphic in graphics)
        {
            if (graphic == null || graphic.canvasRenderer.cull)
            {
                continue; // 跳过空组件或被剔除的组件
            }

            // 获取组件的渲染材质和主纹理
            Material material = graphic.materialForRendering;
            Texture mainTexture = material != null ? material.mainTexture : null;
            // 获取组件是否使用遮罩的信息
            bool isMasked = graphic.GetComponent<Mask>() != null;
            // 获取图集信息
            string atlasName = GetAtlasName(mainTexture)?? graphic.name + atlasDifferentFlag++;

            // 生成唯一的 DrawCall 标识，考虑材质、纹理、遮罩和图集情况
            string drawCallKey = GenerateDrawCallKey(material, mainTexture, isMasked,atlasName);

            if (!drawCallGroups.ContainsKey(drawCallKey))
            {
                drawCallCount++;
                drawCallGroups[drawCallKey] = new List<Graphic>();
                currentDrawCallIndex++;
                drawCallGraphics[currentDrawCallIndex] = new Graphic[1];
                drawCallGraphics[currentDrawCallIndex][0] = graphic;
                drawCallTextures.Add(graphic.mainTexture);
            }
            else
            {
                System.Array.Resize(ref drawCallGraphics[currentDrawCallIndex], drawCallGraphics[currentDrawCallIndex].Length + 1);
                drawCallGraphics[currentDrawCallIndex][drawCallGraphics[currentDrawCallIndex].Length - 1] = graphic;
            }

            drawCallGroups[drawCallKey].Add(graphic);
        }
    }

    // 生成唯一的 DrawCall 标识，根据材质、纹理、遮罩和图集情况
    private string GenerateDrawCallKey(Material material, Texture texture, bool isMasked, string atlasName)
    {
        string materialName = material != null ? material.name : "NullMaterial";
        string textureName = texture != null ? texture.name : "NullTexture";
        string maskInfo = isMasked ? "_Masked" : "";
        string atlasInfo = !string.IsNullOrEmpty(atlasName) ? $"_Atlas_{atlasName}" : "";
        return $"{materialName}_{textureName}{maskInfo}{atlasInfo}";
    }

    // 获取纹理所在的图集名称
    private string GetAtlasName(Texture texture)
    {
        if (texture == null)
        {
            return null;
        }
        // 这里假设你有一个方法可以获取纹理所在的图集名称
        // 例如，如果使用了 Unity 的 Sprite Atlas，可能需要通过特定的 API 来获取
        // 以下是一个简单的模拟，实际使用时需要替换为真实的实现
        // 可以根据项目中图集的管理方式来实现这个方法
        // 比如，通过纹理的路径、标签等信息来判断它是否属于某个图集
        if (texture.name.Contains("Atlas_"))
        {
            int startIndex = texture.name.IndexOf("Atlas_");
            int endIndex = texture.name.IndexOf("_", startIndex + 6);
            if (endIndex == -1)
            {
                endIndex = texture.name.Length;
            }
            return texture.name.Substring(startIndex, endIndex - startIndex);
        }
        return null;
    }
}
