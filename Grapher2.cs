using UnityEngine;

public class Grapher2 : MonoBehaviour
{
    private AudioSource aSource;
    public int maxSize=256;




    public enum FunctionOption
    {
        Linear,
        Exponential,
        Parabola,
        Sine,
        Ripple,
        AudioWave
    }

    private delegate float FunctionDelegate(Vector3 p, float t);

    public FunctionOption function;

    [Range(10, 90)]
    public int resolution = 10;

    private int currentResolution;
    private ParticleSystem.Particle[] points;

    private void CreatePoints()
    {
        float[] spectrum = aSource.GetSpectrumData(maxSize, 0, FFTWindow.BlackmanHarris);
        currentResolution = resolution;
        points = new ParticleSystem.Particle[resolution * resolution];
        float increment = 1f / (resolution - 1);
        int i = 0;
        for (int y = 0; y < resolution; y++)
        {
            for (int z = 0; z < resolution; z++)
            {

                Vector3 p = new Vector3(0f, y*increment, z * increment);
                points[i].position = p;
                points[i].color = new Color(0f, p.y, p.z);
                points[i++].size = 0.1f;
            } 
        }
    }

    void Awake()
    {
        this.aSource = GetComponent<AudioSource>();
    }

    void Update()
    {

        float[] spectrum = aSource.GetSpectrumData(maxSize, 0, FFTWindow.BlackmanHarris);
        if (currentResolution != resolution || points == null)
        {
            CreatePoints();
        }
      //  FunctionDelegate f = functionDelegates[(int)function];
        float t = Time.timeSinceLevelLoad;
        for (int j = 0; j < points.Length; j++)
        {
            Vector3 p = points[j].position;
            p.x = AudioWave(p, t);
            points[j].position = p;
            Color c = points[j].color;
            c.g = p.x;
            points[j].color = c;
        }
        GetComponent<ParticleSystem>().SetParticles(points, points.Length);
        
        int i = 1;
       /* while (i < 8191)
        {
            Debug.DrawLine(new Vector3(i - 1, spectrum[i]*100 + 10, 0), new Vector3(i, spectrum[i + 1]*100 + 10, 0), Color.red);
            Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, -1), new Vector3(i, Mathf.Log(spectrum[i]) + 10, -1), Color.cyan);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 2), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 2), Color.yellow);
            i++;
        }*/
    }

    private float Linear(Vector3 p, float t)
    {
        return p.x;
    }

    private float Exponential(Vector3 p, float t)
    {
        return p.x * p.x;
    }

    private float Parabola(Vector3 p, float t)
    {
        p.x = 2f * p.x - 1f;
        p.z = 2f * p.z - 1f;
        return 1f - p.x * p.x * p.z * p.z;
    }

    private float Sine(Vector3 p, float t)
    {
        return 0.50f +
            0.25f * Mathf.Sin(4 * Mathf.PI * p.x + 4 * t) * Mathf.Sin(2 * Mathf.PI * p.z + t) +
            0.10f * Mathf.Cos(3 * Mathf.PI * p.x + 5 * t) * Mathf.Cos(5 * Mathf.PI * p.z + 3 * t) +
            0.15f * Mathf.Sin(Mathf.PI * p.x + 0.6f * t);
    }

    private float AudioWave(Vector3 p, float t)
    {
        float[] spectrum = aSource.GetSpectrumData(maxSize, 0, FFTWindow.BlackmanHarris);
        int y = (int)p.y;
        int z = (int) p.z;
        p.y -= 0.1f;
        p.z -= 0.1f;
        float vol = aSource.volume;
        float volSquare = Mathf.Pow(vol, 2);
        float squareRadius = p.y * p.y + p.z * p.z;
        float sum = 0;
        for(int i = 0; i<spectrum.Length; i++)
        {
            sum += spectrum[i];
        }
        sum *= volSquare;
        return 0.5f + Mathf.Sin( sum*1000* Mathf.PI * squareRadius*aSource.volume*aSource.volume - 2f * t) / (2f + 100f * squareRadius);

    }

    private float Ripple(Vector3 p, float t)
    {
        p.x -= 0.05f;
        p.z -= 0.05f;
        float squareRadius = p.x * p.x + p.z * p.z;
        return 0.5f + Mathf.Sin(15f * Mathf.PI * squareRadius - 2f * t) / (2f + 100f * squareRadius);
    }

}