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

    // ���ڴ洢ÿ�� DrawCall �������Ϣ������һ��Ψһ��ʶ��ֵ�Ǹ÷����µ� Graphic ����б�
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
                        //������Ϣ
                        Material material = drawCallGraphics[i][0].materialForRendering;
                        EditorGUILayout.LabelField($"Material: {material.name}");

                        // ������ UI Ԫ���б�
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

        // ����Ⱦ˳��� Graphic �����������ȷ������������ʵ����Ⱦ˳��
        var sortedGraphics = graphics.OrderBy(g => g.transform.GetSiblingIndex()).ToList();

        drawCallCount = 0;
        drawCallGraphics = new Graphic[graphics.Length][];

        int currentDrawCallIndex = -1;
        int atlasDifferentFlag = 0;

        foreach (Graphic graphic in graphics)
        {
            if (graphic == null || graphic.canvasRenderer.cull)
            {
                continue; // ������������޳������
            }

            // ��ȡ�������Ⱦ���ʺ�������
            Material material = graphic.materialForRendering;
            Texture mainTexture = material != null ? material.mainTexture : null;
            // ��ȡ����Ƿ�ʹ�����ֵ���Ϣ
            bool isMasked = graphic.GetComponent<Mask>() != null;
            // ��ȡͼ����Ϣ
            string atlasName = GetAtlasName(mainTexture)?? graphic.name + atlasDifferentFlag++;

            // ����Ψһ�� DrawCall ��ʶ�����ǲ��ʡ��������ֺ�ͼ�����
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

    // ����Ψһ�� DrawCall ��ʶ�����ݲ��ʡ��������ֺ�ͼ�����
    private string GenerateDrawCallKey(Material material, Texture texture, bool isMasked, string atlasName)
    {
        string materialName = material != null ? material.name : "NullMaterial";
        string textureName = texture != null ? texture.name : "NullTexture";
        string maskInfo = isMasked ? "_Masked" : "";
        string atlasInfo = !string.IsNullOrEmpty(atlasName) ? $"_Atlas_{atlasName}" : "";
        return $"{materialName}_{textureName}{maskInfo}{atlasInfo}";
    }

    // ��ȡ�������ڵ�ͼ������
    private string GetAtlasName(Texture texture)
    {
        if (texture == null)
        {
            return null;
        }
        // �����������һ���������Ի�ȡ�������ڵ�ͼ������
        // ���磬���ʹ���� Unity �� Sprite Atlas��������Ҫͨ���ض��� API ����ȡ
        // ������һ���򵥵�ģ�⣬ʵ��ʹ��ʱ��Ҫ�滻Ϊ��ʵ��ʵ��
        // ���Ը�����Ŀ��ͼ���Ĺ���ʽ��ʵ���������
        // ���磬ͨ�������·������ǩ����Ϣ���ж����Ƿ�����ĳ��ͼ��
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
