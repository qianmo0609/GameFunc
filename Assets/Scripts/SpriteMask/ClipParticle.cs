using UnityEngine;

public class ClipParticle : MonoBehaviour
{
    public RectTransform m_RectTransform;
    public Camera m_Camera;
    public Material m_Material;

    Vector3[] v;

    public float _minX;
    public float _maxX;
    public float _minY;
    public float _maxY;

    public void Start()
    {
        v = new Vector3[4];
        this.CalcVector();
    }

#if UNITY_EDITOR
    void Update()
    {
        if (m_RectTransform.hasChanged)
        {
            this.CalcVector();
        }
    }
#endif

    void CalcVector()
    {
        this.OnResetRect();
        m_RectTransform.GetWorldCorners(v);
        for (int i = 0; i < 4; i++)
        {
            //首先将UI坐标转换为屏幕坐标，再将屏幕坐标转换为世界坐标
            this.SetInfo(m_Camera.ScreenToWorldPoint(RectTransformUtility.WorldToScreenPoint(m_Camera, v[i])));
        }
        m_Material.SetVector("_Rect", new Vector4(_minX, _maxX, _minY, _maxY));
        m_RectTransform.hasChanged = false;
    }

    public void SetInfo(Vector3 pos)
    {
        if (_minX > pos.x)
        {
            _minX = pos.x;
        }
        if (_maxX < pos.x)
        {
            _maxX = pos.x;
        }
        if (_minY > pos.y)
        {
            _minY = pos.y;
        }
        if (_maxY < pos.y)
        {
            _maxY = pos.y;
        }
    }

    void OnResetRect()
    {
        _minX = float.PositiveInfinity;
        _maxX = float.NegativeInfinity;
        _minY = float.PositiveInfinity;
        _maxY = float.NegativeInfinity;
    }
}
