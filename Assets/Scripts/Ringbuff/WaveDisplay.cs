using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveDisplay : MonoBehaviour
{
    LineRenderer lineRenderer;
    public int posCount ; //采样个数
    SlidingAverage slidingAverage;
    public float frequency = .5f; //频率
    public float amplitude = 1f; //振幅


    // Start is called before the first frame update
    void Start()
    {
        this.lineRenderer = GetComponent<LineRenderer>();
        slidingAverage = new SlidingAverage(10, 0);
        this.Draw();
    }

    void Draw()
    {
        float xStart = 0f;
        float tau = 2 * Mathf.PI;
        float xFinish = tau;

        this.lineRenderer.positionCount = posCount;

        for (int i = 0; i < posCount; i++) {
            float progress = (float)i / (posCount - 1);
            float x = Mathf.Lerp(xStart, xFinish, progress);
            float y = amplitude * Mathf.Sin(tau * frequency * x) + Random.Range(0f,1f);
            slidingAverage.pushValue(y);
            //this.lineRenderer.SetPosition(i, new Vector3(x, y, 0));
            this.lineRenderer.SetPosition(i, new Vector3(x, slidingAverage.getSmoothedValue(), 0));
        }
    }

    private void Update()
    {
        //this.Draw();
    }
}

public class SlidingAverage
{
    float[] buffer;
    float sum;
    int lastIndex;
    public SlidingAverage(int num_samples, float initial_value)
    {
        buffer = new float[num_samples];
        lastIndex = 0;
        reset(initial_value);
    }

    public void reset(float value)
    {
        sum = value * buffer.Length;
        for (int i = 0; i < buffer.Length; ++i)
            buffer[i] = value;
    }

    public void pushValue(float value)
    {
        sum -= buffer[lastIndex]; // subtract the oldest sample from the sum 
        sum += value; // add the new sample 
        buffer[lastIndex] = value; // store the new sample 
                                   // advance the index and wrap it around 
        lastIndex += 1;
        if (lastIndex >= buffer.Length) lastIndex = 0;
    }

    public float getSmoothedValue()
    {
        return sum / buffer.Length;
    }
}
