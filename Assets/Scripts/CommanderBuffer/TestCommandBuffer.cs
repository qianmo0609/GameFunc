using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 用于处理粒子系统的渲染和命令缓冲区操作
/// </summary>
public class TestCommandBuffer : MonoBehaviour
{
    //配置根粒子系统
    [SerializeField] private ParticleSystem _rootParticleSystem;
    //存储根粒子系统及其子系统的所有粒子系统渲染器
    private ParticleSystemRenderer[] _allSystemRenderers = null;

    //用于设置起始偏移量
    [SerializeField] private float _startingOffset = 0.0f;
    //用于设置更新速率
    [SerializeField] private float _updateRate = 1.0f / 12.0f;
    //累加器，用于跟踪时间
    private float _accumulator = 0.0f;

    //用于设置是否启用粒子包裹效果
    [SerializeField] private bool _wrapParticles = false;
    //用于设置包裹大小
    [SerializeField] private float _wrapSize = 5.0f;
    //用于设置包裹材质的纹理
    [SerializeField] private Texture _wrapMaterialTexture = null;
    //储所有粒子系统渲染器的网格
    private List<Mesh> _meshes = new List<Mesh>();
    //用于设置材质属性
    private MaterialPropertyBlock _mirrorBlock = null;

    //用于设置输出渲染纹理
    [SerializeField] private RenderTexture _outputTexture = null;
    //用于设置在绘制时是否清除渲染目标
    [SerializeField] private bool _clearOnDraw = true;
    //用于设置清除颜色
    [SerializeField] private Color _clearColor = Color.white;
    //用于设置虚拟相机的大小
    [SerializeField] private float _virtualCameraSize = 5.0f;

    void Start()
    {
        //播放根粒子系统
        _rootParticleSystem.Play();
        //暂停根粒子系统
        _rootParticleSystem.Pause();
        _mirrorBlock = new MaterialPropertyBlock();
        //设置材质属性块的主纹理为包裹材质的纹理
        _mirrorBlock.SetTexture("_MainTex", _wrapMaterialTexture);
        //获取根粒子系统及其子系统的所有粒子系统渲染器
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

        //确保在开始时累加器的值为更新速率加上起始偏移量
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

        // 累加时间
        _accumulator += Time.deltaTime;
        int iterations = 0;
        // 当累加器的值大于等于更新速率且迭代次数小于8时，执行循环
        while ((_accumulator >= _updateRate) && (iterations < 8))
        {
            // 模拟根粒子系统的更新
            //_rootParticleSystem.Simulate(_updateRate, true, false, false);
            SimulateParticles();
            // 为每个渲染器烘焙网格
            for (int i = 0; i < _allSystemRenderers.Length; i++)
            {
                //_allSystemRenderers[i].BakeMesh(_meshes[i], false);
                BakeMeshes();
            }
            // 调用_drawNow方法进行绘制
            _drawNow();
            // 减去更新速率，更新累加器的值
            _accumulator -= _updateRate;
            // 增加迭代次数
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

        // 设置视图矩阵，用于确定相机的位置和方向
        cmd.SetViewMatrix(Matrix4x4.TRS(new Vector3(0.0f, 0.0f, -10.0f), Quaternion.identity, Vector3.one));
        // 设置投影矩阵，用于确定相机的投影方式
        cmd.SetProjectionMatrix(Matrix4x4.Ortho(-_virtualCameraSize, _virtualCameraSize, -_virtualCameraSize, _virtualCameraSize, 0.1f, 10.0f));

        // 设置渲染目标为输出纹理
        cmd.SetRenderTarget(_outputTexture);
        // 如果需要在绘制时清除渲染目标，则清除渲染目标
        if (_clearOnDraw)
            cmd.ClearRenderTarget(true, true, _clearColor, 1.0f);

        // 根据是否启用粒子包裹效果，确定循环范围
        int min = (_wrapParticles) ? (-1) : (0);
        int max = (_wrapParticles) ? (1) : (0);

        // 嵌套循环，用于处理粒子包裹效果
        for (int y = min; y <= max; y++)
        {
            //遍历所有网格，绘制每个网格
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

        // 执行命令缓冲区中的所有命令
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
