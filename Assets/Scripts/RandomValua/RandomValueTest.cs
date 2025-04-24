using System;
using UnityEngine;

public class RandomValueTest : MonoBehaviour
{
    int MAXTIMES = 10000;

    void Start()
    {
        //this.TestLCG();
        //this.TestXorshift();
        //this.TestPCG();
        this.TestMersenneTwister();
    }

    void TestLCG()
    {
        // ʹ��ʾ��
        var lcg = new LCG(DateTime.UtcNow.Ticks);
        int times = 0;
        for (int i = 0; i < MAXTIMES; i++)
        {
            //float randomNumber = UnityEngine.Random.Range(100f,200f);
            float randomNumber = lcg.Range(100f, 200f);
            //Debug.Log(randomNumber);
            if (randomNumber >= 150f)
            {
               times++;
            }
        }
        Debug.Log((float)times/MAXTIMES);
    }

    void TestMersenneTwister()
    {
        // ʹ��ʾ��
        var mt = new MersenneTwister((uint)DateTime.UtcNow.Ticks);
        int times = 0;
        for (int i = 0; i < MAXTIMES; i++)
        {
            float randomNumber = mt.Range(100f, 200f);
            if (randomNumber >= 150f)
            {
                times++;
            }
        }

        Debug.Log(times);
    }

    void TestPCG()
    {
        // ʹ��ʾ��
        var pcg = new PCG32((uint)DateTime.UtcNow.Ticks);
        int times = 0;
        for (int i = 0; i < MAXTIMES; i++)
        {
            float randomNumber = pcg.Range(100f, 200f);
            if (randomNumber >= 150f)
            {
                times++;
            }
        }

        Debug.Log(times);
    }

    void TestXorshift()
    {
        // ʹ��ʾ��
        var xorshift = new Xorshift32((int)DateTime.UtcNow.Ticks);//0xdeadbeef);
        int times = 0;
        for (int i = 0; i < MAXTIMES; i++)
        {
            float randomNumber = xorshift.Range(100f, 200f);
            //Debug.Log(randomNumber);
            if(randomNumber >= 150f)
            {
                times++;
            }
        }

        Debug.Log(times);
    }
}


public class LCG
{
    private float _state;
    private const float A = 1664525.0f;
    private const float C = 1013904223.0f;
    private const float M = float.MaxValue;

    public LCG(float seed) => _state = seed;

    float Next()
    {
        _state = (A * _state + C) % M;
        return _state / M;
    }

    public float Range(float min,float max)
    {
        if (min > max)
            throw new ArgumentException("min ����С�ڵ��� max");
        return (this.Next() * (max - min)) + min;
    }

    public uint Range(uint min, uint max)
    {
        if (min > max)
            throw new ArgumentException("min ����С�ڵ��� max");
        return (uint)(this.Next() * (max - min) + min);
    }

    public int Range(int min, int max)
    {
        if (min > max)
            throw new ArgumentException("min ����С�ڵ��� max");
        return (int)(this.Next() * (max - min) + min);
    }
}

public class MersenneTwister
{
    private const int N = 624;
    private readonly uint[] _mt = new uint[N];
    private int _index = 0;

    public MersenneTwister(uint seed)
    {
        _mt[0] = seed;
        for (int i = 1; i < N; i++)
        {
            _mt[i] = 1812433253U * (_mt[i - 1] ^ (_mt[i - 1] >> 30)) + (uint)i;
        }
    }

    public uint Next()
    {
        if (_index >= N) Twist();

        uint y = _mt[_index++];
        y ^= y >> 11;
        y ^= (y << 7) & 0x9d2c5680U;
        y ^= (y << 15) & 0xefc60000U;
        y ^= y >> 18;
        return y;
    }

    private void Twist()
    {
        for (int i = 0; i < N; i++)
        {
            uint y = (_mt[i] & 0x80000000U) | (_mt[(i + 1) % N] & 0x7FFFFFFFU);
            _mt[i] = _mt[(i + 397) % N] ^ (y >> 1) ^ ((y & 1) == 0 ? 0U : 0x9908B0DFU);
        }
        _index = 0;
    }

    public float Range(float min, float max)
    {
        if (min > max)
            throw new ArgumentException("min ����С�ڵ��� max");
        return ((float)this.Next() / uint.MaxValue * (max - min)) + min;
    }

    public uint Range(uint min, uint max)
    {
        if (min > max)
            throw new ArgumentException("min ����С�ڵ��� max");
        return (uint)((float)this.Next() / uint.MaxValue * (max - min) + min);
    }

    public int Range(int min, int max)
    {
        if (min > max)
            throw new ArgumentException("min ����С�ڵ��� max");
        return (int)((float)this.Next() / uint.MaxValue * (max - min) + min);
    }
}

public class PCG32
{
    private ulong _state;
    private const ulong Multiplier = 6364136223846793005UL;
    private const ulong Increment = 1442695040888963407UL;

    public PCG32(ulong seed) => _state = seed;

    public uint Next()
    {
        ulong oldState = _state;
        _state = oldState * Multiplier + Increment;
        uint xorShifted = (uint)(((oldState >> 18) ^ oldState) >> 27);
        int rot = (int)(oldState >> 59);
        return (xorShifted >> rot) | (xorShifted << (32 - rot));
    }

    public float Range(float min, float max)
    {
        if (min > max)
            throw new ArgumentException("min ����С�ڵ��� max");
        return ((float)this.Next()/uint.MaxValue * (max - min)) + min;
    }

    public uint Range(uint min, uint max)
    {
        if (min > max)
            throw new ArgumentException("min ����С�ڵ��� max");
        return (uint)((float)this.Next() / uint.MaxValue * (max - min) + min);
    }

    public int Range(int min, int max)
    {
        if (min > max)
            throw new ArgumentException("min ����С�ڵ��� max");
        return (int)((float)this.Next() / uint.MaxValue * (max - min) + min);
    }

    // ================= ������������ =================
    /// <summary>
    /// ���� [minInclusive, maxInclusive] �������������䣩
    /// </summary>
    public int NextInt(int minInclusive, int maxInclusive)
    {
        if (minInclusive > maxInclusive)
            throw new ArgumentException("min ����С�ڵ��� max");

        // ת��Ϊ�޷��ŷ�Χ������ģƫ��
        uint range = (uint)(maxInclusive - minInclusive + 1);
        uint random = Next();
        return (int)((random % range) + minInclusive);
    }

    // ================= �������������� =================
    /// <summary>
    /// ���� [0.0, 1.0) �ĸ�����������ҿ���
    /// </summary>
    public double NextDouble()
    {
        return Next() / 4294967296.0; // 2^32
    }

    /// <summary>
    /// ���� [minInclusive, maxExclusive) �ĸ�����������ҿ���
    /// </summary>
    public double NextDouble(double minInclusive, double maxExclusive)
    {
        if (minInclusive >= maxExclusive)
            throw new ArgumentException("max ������� min");

        return minInclusive + NextDouble() * (maxExclusive - minInclusive);
    }

    /// <summary>
    /// ���� [minInclusive, maxInclusive] �ĸ������������䣩
    /// </summary>
    public double NextDoubleInclusive(double minInclusive, double maxInclusive)
    {
        if (minInclusive > maxInclusive)
            throw new ArgumentException("min ����С�ڵ��� max");

        // ʹ�ø��߾��ȵ����Դȷ�������߽�
        return minInclusive + (Next() / 4294967295.0) * (maxInclusive - minInclusive);
    }
}

public class Xorshift32
{
    private int _state;

    public Xorshift32(int seed) => _state = seed;

    public int Next()
    {
        _state ^= _state << 13;
        _state ^= _state >> 17;
        _state ^= _state << 5;
        return _state;
    }


    public float Range(float min, float max)
    {
        if (min > max)
            throw new ArgumentException("min ����С�ڵ��� max");
        return (this.Next() * (max - min)) + min;
    }

    public uint Range(uint min, uint max)
    {
        if (min > max)
            throw new ArgumentException("min ����С�ڵ��� max");
        return (uint)(this.Next() * (max - min) + min);
    }

    public int Range(int min, int max)
    {
        if (min > max)
            throw new ArgumentException("min ����С�ڵ��� max");
        return (int)(this.Next() * (max - min) + min);
    }

    // ================= ������������ =================
    /// <summary>
    /// ���� [minInclusive, maxInclusive] �������������䣩
    /// </summary>
    public int NextInt(int minInclusive, int maxInclusive)
    {
        if (minInclusive > maxInclusive)
            throw new ArgumentException("min ����С�ڵ��� max");

        // ת��Ϊ�޷��Ÿ�������Χ������ģƫ��
        double range = (double)maxInclusive - minInclusive + 1;
        return (int)(NextDouble() * range) + minInclusive;
    }

    // ================= �������������� =================
    /// <summary>
    /// ���� [0.0, 1.0) �ĸ�����������ҿ���
    /// </summary>
    public double NextDouble()
    {
        return Next() / (double)(uint.MaxValue + 1UL); // ���� 2^32 ȷ��������1.0
    }

    /// <summary>
    /// ���� [minInclusive, maxExclusive) �ĸ�����������ҿ���
    /// </summary>
    public double NextDouble(double minInclusive, double maxExclusive)
    {
        if (minInclusive > maxExclusive)
            throw new ArgumentException("min ����С�ڵ��� max");

        return minInclusive + NextDouble() * (maxExclusive - minInclusive);
    }

    /// <summary>
    /// ���� [minInclusive, maxInclusive] �ĸ������������䣩
    /// </summary>
    public double NextDoubleInclusive(double minInclusive, double maxInclusive)
    {
        if (minInclusive > maxInclusive)
            throw new ArgumentException("min ����С�ڵ��� max");

        // ����Χ���ض���ȷ���������ֵ
        return Math.Min(
            minInclusive + (Next() / (double)uint.MaxValue) * (maxInclusive - minInclusive),
            maxInclusive
        );
    }
}
