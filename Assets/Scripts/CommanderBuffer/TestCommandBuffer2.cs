using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TestCommandBuffer2 : MonoBehaviour
{
    // ������ϵͳ
    [SerializeField] private ParticleSystem _rootParticleSystem;
    private ParticleSystemRenderer[] _allSystemRenderers = null;

    // ��ʼƫ����
    [SerializeField] private float _startingOffset = 0.0f;
    // ����Ƶ��
    [SerializeField] private float _updateRate = 1.0f / 12.0f;
    private float _accumulator = 0.0f;

    // �Ƿ��������
    [SerializeField] private bool _wrapParticles = false;
    // ������С
    [SerializeField] private float _wrapSize = 5.0f;
    // ������������
    [SerializeField] private Texture _wrapMaterialTexture = null;
    private List<Mesh> _meshes = new List<Mesh>();
    private MaterialPropertyBlock _mirrorBlock = null;

    // �������
    [SerializeField] private RenderTexture _outputTexture = null;
    // ����ʱ�Ƿ����
    [SerializeField] private bool _clearOnDraw = true;
    // �����ɫ
    [SerializeField] private Color _clearColor = Color.white;
    // ���������С
    [SerializeField] private float _virtualCameraSize = 5.0f;

    // ����������
    private const int MAX_ITERATIONS = 8;
    // ��ͼ����� Z ƫ����
    private const float VIEW_MATRIX_Z_OFFSET = -10.0f;
    // ͶӰ����Ľ��ü���
    private const float PROJECTION_MATRIX_NEAR = 0.1f;
    // ͶӰ�����Զ�ü���
    private const float PROJECTION_MATRIX_FAR = 10.0f;
    // ��������� Z λ��
    private const float DRAW_MESH_Z_POSITION = 5.0f;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
            return;
        }
        Initialize();
        mainCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, CreateCommandBuffer());
    }

    void OnDestroy()
    {
        CleanupMeshes();
        if (mainCamera != null)
        {
            mainCamera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, CreateCommandBuffer());
        }
    }

    void OnEnable()
    {
        Initialize();
        if (mainCamera != null)
        {
            mainCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, CreateCommandBuffer());
        }
    }

    void OnDisable()
    {
        if (mainCamera != null)
        {
            mainCamera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, CreateCommandBuffer());
        }
    }

    void Update()
    {
        if (mainCamera == null)
            return;

        _accumulator += Time.deltaTime;
        int iterations = 0;
        while ((_accumulator >= _updateRate) && (iterations < MAX_ITERATIONS))
        {
            SimulateParticles();
            BakeMeshes();
            _drawNow();
            _accumulator -= _updateRate;
            iterations++;
        }
    }

    /// <summary>
    /// ��ʼ��������������������ϵͳ���������Կ������
    /// </summary>
    private void Initialize()
    {
        _rootParticleSystem.Play();
        _rootParticleSystem.Pause();
        _mirrorBlock = new MaterialPropertyBlock();
        _mirrorBlock.SetTexture("_MainTex", _wrapMaterialTexture);
        _allSystemRenderers = _rootParticleSystem.GetComponentsInChildren<ParticleSystemRenderer>();
        _meshes = new List<Mesh>(_allSystemRenderers.Length);
        for (int i = 0; i < _allSystemRenderers.Length; i++)
        {
            _meshes.Add(new Mesh());
        }

        // ȷ���ڿ�ʼʱ�ۼ�����һ����ʼֵ
        _accumulator = _updateRate + _startingOffset;
    }

    /// <summary>
    /// ����������Դ
    /// </summary>
    private void CleanupMeshes()
    {
        foreach (Mesh m in _meshes)
        {
            Destroy(m);
        }
        _meshes.Clear();
    }

    /// <summary>
    /// ģ������ϵͳ
    /// </summary>
    private void SimulateParticles()
    {
        try
        {
            _rootParticleSystem.Simulate(_updateRate, true, false, false);
            Debug.Log("Particle system simulated successfully.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to simulate particle system: {e.Message}");
        }
    }

    /// <summary>
    /// �決����ϵͳ������
    /// </summary>
    private void BakeMeshes()
    {
        for (int i = 0; i < _allSystemRenderers.Length; i++)
        {
            try
            {
                _allSystemRenderers[i].BakeMesh(_meshes[i], false);
                Debug.Log($"Mesh {i} baked successfully.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to bake mesh {i}: {e.Message}");
            }
        }
    }

    /// <summary>
    /// ���Ƶ�ǰ����ϵͳ
    /// </summary>
    private void _drawNow()
    {
        CommandBuffer cmd = CreateCommandBuffer();
        if (cmd == null)
        {
            Debug.LogError("Failed to create command buffer.");
            return;
        }

        try
        {
            Graphics.ExecuteCommandBuffer(cmd);
            Debug.Log("Command buffer executed successfully.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to execute command buffer: {e.Message}");
        }
    }

    private CommandBuffer CreateCommandBuffer()
    {
        if (_outputTexture == null)
        {
            Debug.LogError("Output texture is null, cannot create command buffer.");
            return null;
        }

        CommandBuffer cmd = new CommandBuffer();

        SetupViewAndProjection(cmd);
        SetupRenderTarget(cmd);
        DrawMeshes(cmd);

        return cmd;
    }

    private void SetupViewAndProjection(CommandBuffer cmd)
    {
        cmd.SetViewMatrix(Matrix4x4.TRS(new Vector3(0.0f, 0.0f, VIEW_MATRIX_Z_OFFSET), Quaternion.identity, Vector3.one));
        cmd.SetProjectionMatrix(Matrix4x4.Ortho(-_virtualCameraSize, _virtualCameraSize, -_virtualCameraSize, _virtualCameraSize, PROJECTION_MATRIX_NEAR, PROJECTION_MATRIX_FAR));
    }

    private void SetupRenderTarget(CommandBuffer cmd)
    {
        cmd.SetRenderTarget(_outputTexture);
        if (_clearOnDraw)
        {
            cmd.ClearRenderTarget(true, true, _clearColor, 1.0f);
        }
    }

    private void DrawMeshes(CommandBuffer cmd)
    {
        int min = (_wrapParticles) ? (-1) : (0);
        int max = (_wrapParticles) ? (1) : (0);
        for (int y = min; y <= max; y++)
        {
            for (int x = min; x <= max; x++)
            {
                for (int i = 0; i < _meshes.Count; i++)
                {
                    cmd.DrawMesh(_meshes[i], Matrix4x4.TRS(new Vector3(x * _wrapSize, y * _wrapSize, DRAW_MESH_Z_POSITION),
                                                              Quaternion.identity, Vector3.one),
                                 _allSystemRenderers[i].sharedMaterial, 0, 0, _mirrorBlock);
                }
            }
        }
    }
}
