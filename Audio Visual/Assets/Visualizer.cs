using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualizer : MonoBehaviour
{
    const int SAMPLE_SIZE = 512;

    public float rmsValue; // avg power output of sound
    public float dbValue; // sound value during certain frame
    public float pitchValue;

    public float backgroundIntensity;
    public float backgroundSmoothSpeed = 0.5f;
    public Material backgroundMaterial;
    public Color minColor;
    public Color maxColor;

    public float maxVisualScale = 25.0f; // max size of visuals
    public float sizeMultiplier = 50.0f;
    public float smoothSpeed = 10.0f; // reduces size of visual over time
    public float keepPercentage = 50f;

    public float circleRadius = 10.0f;

    private AudioSource source; // audio file
    private float[] samples;
    private float[] spectrum;
    private float sampleRate;

    private Transform[] visualList;
    private float[] visualScale;
    public int amtOfVisual = 10; // amount of objects being shown

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        samples = new float[SAMPLE_SIZE];
        spectrum = new float[SAMPLE_SIZE];
        sampleRate = AudioSettings.outputSampleRate; // read from file itself

        //SpawnLine();
        SpawnCircle();
    }

    // Update is called once per frame
    void Update()
    {
        AnalyzeSound();
        UpdateVisual();
        UpdateBackground();
    }

    void AnalyzeSound()
    {
        source.GetOutputData(samples, 0); // samples is being modified

        // Get RMS value
        int i = 0;
        float sum = 0;
        for (i = 0; i < SAMPLE_SIZE; i++)
        {
            sum += samples[i] * samples[i];
        }
        rmsValue = Mathf.Sqrt(sum / SAMPLE_SIZE);

        // Get DB value
        dbValue = 20 * Mathf.Log10(rmsValue / 0.1f);

        // Get sound spectrum
        source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        // Get pitch value
        float maxV = 0;
        int maxN = 0;
        for (i = 0; i < SAMPLE_SIZE; i++)
        {
            if (!(spectrum[i] > maxV) || !(spectrum[i] > 0.0f))
                continue;

            maxV = spectrum[i];
            maxN = i;
        }

        float freqN = maxN;
        if (maxN > 0 && maxN < SAMPLE_SIZE - 1)
        {
            float dL = spectrum[maxN - 1] / spectrum[maxN];
            float dR = spectrum[maxN + 1] / spectrum[maxN];
            freqN = 0.5f * (dR * dR - dL * dL);
        }
        pitchValue = freqN * (sampleRate / 2) / SAMPLE_SIZE;
    }

    void SpawnLine()
    {
        visualScale = new float[amtOfVisual];
        visualList = new Transform[amtOfVisual];

        for (int i = 0; i < amtOfVisual; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            visualList[i] = go.transform;
            visualList[i].position = Vector3.right * i; // positions primitives 1m apart
        }
    }

    void SpawnCircle()
    {
        visualScale = new float[amtOfVisual];
        visualList = new Transform[amtOfVisual];
        Vector3 center = Vector3.zero;

        for (int i = 0; i < amtOfVisual; i++)
        {
            float ang = i * 1.0f / amtOfVisual;
            ang *= Mathf.PI * 2;
            float x = center.x + Mathf.Cos(ang) * circleRadius;
            float y = center.y + Mathf.Sin(ang) * circleRadius;

            Vector3 pos = center + new Vector3(x, y, 0);
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            go.transform.position = pos;
            go.transform.rotation = Quaternion.LookRotation(Vector3.forward, pos);
            visualList[i] = go.transform;
        }
    }

    void UpdateVisual()
    {
        int visualIndex = 0;
        int spectrumIndex = 0;
        int averageSize = (int) ((SAMPLE_SIZE * (keepPercentage / 100))/ amtOfVisual);

        while (visualIndex < amtOfVisual)
        {
            int j = 0;
            float sum = 0;
            while (j < averageSize)
            {
                sum += spectrum[spectrumIndex];
                spectrumIndex++;
                j++;
            }

            // if value is lower next frame, slowly go down
            float scaleY = sum / averageSize * sizeMultiplier;
            visualScale[visualIndex] -= Time.deltaTime * smoothSpeed;
            if (visualScale[visualIndex] < scaleY)
                visualScale[visualIndex] = scaleY;

            if (visualScale[visualIndex] > maxVisualScale)
                visualScale[visualIndex] = maxVisualScale;

            visualList[visualIndex].localScale = Vector3.one + Vector3.up * visualScale[visualIndex];
            visualIndex++;
        }
    }

    void UpdateBackground()
    {
        backgroundIntensity -= Time.deltaTime * backgroundSmoothSpeed;
        if (backgroundIntensity < dbValue / 20)
            backgroundIntensity = dbValue / 20;

        backgroundMaterial.color = Color.Lerp(maxColor, minColor, -backgroundIntensity);
    }
}
