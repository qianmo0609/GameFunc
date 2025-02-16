using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// ���ڴ�������ϵͳ����Ⱦ�������������
/// </summary>
public class TestCommandBuffer : MonoBehaviour
{
    //���ø�����ϵͳ
    [SerializeField] private ParticleSystem _rootParticleSystem;
    //�洢������ϵͳ������ϵͳ����������ϵͳ��Ⱦ��
    private ParticleSystemRenderer[] _allSystemRenderers = null;

    //����������ʼƫ����
    [SerializeField] private float _startingOffset = 0.0f;
    //�������ø�������
    [SerializeField] private float _updateRate = 1.0f / 12.0f;
    //�ۼ��������ڸ���ʱ��
    private float _accumulator = 0.0f;

    //���������Ƿ��������Ӱ���Ч��
    [SerializeField] private bool _wrapParticles = false;
    //�������ð�����С
    [SerializeField] private float _wrapSize = 5.0f;
    //�������ð������ʵ�����
    [SerializeField] private Texture _wrapMaterialTexture = null;
    //����������ϵͳ��Ⱦ��������
    private List<Mesh> _meshes = new List<Mesh>();
    //�������ò�������
    private MaterialPropertyBlock _mirrorBlock = null;

    //�������������Ⱦ����
    [SerializeField] private RenderTexture _outputTexture = null;
    //���������ڻ���ʱ�Ƿ������ȾĿ��
    [SerializeField] private bool _clearOnDraw = true;
    //�������������ɫ
    [SerializeField] private Color _clearColor = Color.white;
    //����������������Ĵ�С
    [SerializeField] private float _virtualCameraSize = 5.0f;

    void Start()
    {
        //���Ÿ�����ϵͳ
        _rootParticleSystem.Play();
        //��ͣ������ϵͳ
        _rootParticleSystem.Pause();
        _mirrorBlock = new MaterialPropertyBlock();
        //���ò������Կ��������Ϊ�������ʵ�����
        _mirrorBlock.SetTexture("_MainTex", _wrapMaterialTexture);
        //��ȡ������ϵͳ������ϵͳ����������ϵͳ��Ⱦ��
        _allSystemRenderers = _rootParticleSystem.GetComponentsInChildren<ParticleSystemRenderer>();
        _meshes = new List<Mesh>(_allSystemRenderers.Length);
        for (int i = 0; i < _allSystemRenderers.Length; i++)
        {
            _meshes.Add(new Mesh());
        }

        if (_outputTexture == null)
        {
            Debug.LogError("Output texture is null!");
        }
        else
        {
            Debug.Log($"Output texture width: {_outputTexture.width}, height: {_outputTexture.height}");
        }

        //ȷ���ڿ�ʼʱ�ۼ�����ֵΪ�������ʼ�����ʼƫ����
        _accumulator = _updateRate + _startingOffset;
    }

    void OnDestroy()
    {
        foreach (Mesh m in _meshes)
        {
            Destroy(m);
        }
        _meshes.Clear();
    }

    void OnEnable()
    {
        _rootParticleSystem.Play();
        _rootParticleSystem.Pause();
        _accumulator = _updateRate + _startingOffset;
    }

    void Update()
    {
        if (Camera.main == null)
            return;

        // �ۼ�ʱ��
        _accumulator += Time.deltaTime;
        int iterations = 0;
        // ���ۼ�����ֵ���ڵ��ڸ��������ҵ�������С��8ʱ��ִ��ѭ��
        while ((_accumulator >= _updateRate) && (iterations < 8))
        {
            // ģ�������ϵͳ�ĸ���
            //_rootParticleSystem.Simulate(_updateRate, true, false, false);
            SimulateParticles();
            // Ϊÿ����Ⱦ���決����
            for (int i = 0; i < _allSystemRenderers.Length; i++)
            {
                //_allSystemRenderers[i].BakeMesh(_meshes[i], false);
                BakeMeshes();
            }
            // ����_drawNow�������л���
            _drawNow();
            // ��ȥ�������ʣ������ۼ�����ֵ
            _accumulator -= _updateRate;
            // ���ӵ�������
            iterations++;
        }
    }

    private void SimulateParticles()
    {
        try
        {
            _rootParticleSystem.Simulate(_updateRate, true, false, false);
            //Debug.Log("Particle system simulated successfully.");
        }
        catch (System.Exception e)
        {
            //Debug.LogError($"Failed to simulate particle system: {e.Message}");
        }
    }

    private void BakeMeshes()
    {
        for (int i = 0; i < _allSystemRenderers.Length; i++)
        {
            try
            {
                _allSystemRenderers[i].BakeMesh(_meshes[i], false);
                //Debug.Log($"Mesh {i} baked successfully.");
            }
            catch (System.Exception e)
            {
                //Debug.LogError($"Failed to bake mesh {i}: {e.Message}");
            }
        }
    }

    private void _drawNow()
    {
        CommandBuffer cmd = new CommandBuffer();

        // ������ͼ��������ȷ�������λ�úͷ���
        cmd.SetViewMatrix(Matrix4x4.TRS(new Vector3(0.0f, 0.0f, -10.0f), Quaternion.identity, Vector3.one));
        // ����ͶӰ��������ȷ�������ͶӰ��ʽ
        cmd.SetProjectionMatrix(Matrix4x4.Ortho(-_virtualCameraSize, _virtualCameraSize, -_virtualCameraSize, _virtualCameraSize, 0.1f, 10.0f));

        // ������ȾĿ��Ϊ�������
        cmd.SetRenderTarget(_outputTexture);
        // �����Ҫ�ڻ���ʱ�����ȾĿ�꣬�������ȾĿ��
        if (_clearOnDraw)
            cmd.ClearRenderTarget(true, true, _clearColor, 1.0f);

        // �����Ƿ��������Ӱ���Ч����ȷ��ѭ����Χ
        int min = (_wrapParticles) ? (-1) : (0);
        int max = (_wrapParticles) ? (1) : (0);

        // Ƕ��ѭ�������ڴ������Ӱ���Ч��
        for (int y = min; y <= max; y++)
        {
            //�����������񣬻���ÿ������
            for (int x = min; x <= max; x++)
            {
                for (int i = 0; i < _meshes.Count; i++)
                {
                    cmd.DrawMesh(_meshes[i], Matrix4x4.TRS(new Vector3(x * _wrapSize, y * _wrapSize, 5.0f),
                                                              Quaternion.identity, Vector3.one),
                                 _allSystemRenderers[i].sharedMaterial, 0, 0, _mirrorBlock);
                }
            }
        }

        // ִ����������е���������
        try
        {
            Graphics.ExecuteCommandBuffer(cmd);
            Debug.Log("Command buffer executed successfully.");
        }
        catch (System.Exception e)
        {
            //Debug.LogError($"Failed to execute command buffer: {e.Message}");
        }
    } 
}
