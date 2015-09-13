using UnityEngine;
using System;

public class Grapher2 : MonoBehaviour
{

    private AudioSource aSource;
    public int maxSize = 128;
    [Range(10, 100)]
    public int resolution = 90;
    private Boolean highlighted = false;
    
    public Boolean getHighlighted()
    {
        return this.highlighted;
    }

    public void Highlight()
    {
        this.highlighted = true;
    }

    public void Unhighlight()
    {
        this.highlighted = false;
    }


    private int currentResolution;
    private ParticleSystem.Particle[] points;

    public void increaseVol()
    {
        AudioSource audio = this.GetComponent<AudioSource>();
        if (audio.volume < 0.90)
        {
            audio.volume+=0.1f;
        }
    }

    public void decreaseVol()
    {
        AudioSource audio = this.GetComponent<AudioSource>();
        if(audio.volume > 0.10)
        {
            audio.volume-=0.1f;
        }
    }
   void OnGUI()
    {
        Event e = Event.current;

        if (e.isKey && e.keyCode == KeyCode.KeypadPlus){
            AudioSource audio = this.GetComponent<AudioSource>();
            
        }
        if (e.isKey)
        {
            Debug.Log("Detected key code: " + e.keyCode);

            if (e.keyCode.ToString().Contains("Alpha") && e.keyCode.ToString()[5] >= '0' && e.keyCode.ToString()[5] <= '9')
            {
                String theName = this.gameObject.name;
                switch (theName)
                {
                    case ("Graph 1"):
                        if (e.keyCode == KeyCode.Alpha1)
                        {
                            this.Highlight();
                        }
                        else this.Unhighlight();
                        break;
                    case ("Graph 2"):
                        if (e.keyCode == KeyCode.Alpha2)
                        {
                            this.Highlight();
                        }
                        else
                        {
                            this.Unhighlight();
                        }
                        break;
                    case ("Graph 3"):
                        if (e.keyCode == KeyCode.Alpha3)
                        {
                            this.Highlight();
                        }
                        else this.Unhighlight();
                        break;
                    case ("Graph 4"):
                        if (e.keyCode == KeyCode.Alpha4)
                        {
                            this.Highlight();
                        }
                        else this.Unhighlight();
                        break;

                    case ("Graph 5"):
                        if (e.keyCode == KeyCode.Alpha5)
                        {
                            this.Highlight();
                        }
                        else this.Unhighlight();
                        break;
                    case ("Graph 6"):
                        if (e.keyCode == KeyCode.Alpha6)
                        {
                            this.Highlight();
                        }
                        else this.Unhighlight();
                        break;
                    default:
                        break;
                }
            }
            else if (e.keyCode == KeyCode.UpArrow)
            {
                if (this.getHighlighted())
                {
                    increaseVol();
                }
            }

            else if (e.keyCode == KeyCode.DownArrow)
            {
                if (this.getHighlighted())
                {
                    decreaseVol();
                }
            }
        }
    }

    
    private void CreatePoints()
    {
        float[] spectrum = aSource.GetSpectrumData(maxSize, 0, FFTWindow.BlackmanHarris);
        currentResolution = resolution;
        points = new ParticleSystem.Particle[resolution * resolution/4];
        float increment = 1f / (resolution - 1);
        int i = 0;
        for (int y = 0; y < resolution/8; y++)
        {
            for (int z = 0; z < resolution; z++)
            {

                Vector3 p = new Vector3(0f, y*increment, z * increment);
                points[i].position = p;
                points[i].color = new Color(0f, p.y, p.z, 1f);
                points[i++].size = 0.1f;
            } 
        }
    }

    void Awake()
    {
        Debug.Log("Awake");
        this.aSource = GetComponent<AudioSource>();
        
    }

    void Update()
    {

        Debug.Log("Updating");
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
            if (highlighted)
            {
                c = Color.yellow;
            }
            else
            {
                c = Color.green;
            }
            c.b = p.x;
            points[j].color = c;
            
        }
        


        //set position and color of points
     
        GetComponent<ParticleSystem>().SetParticles(points, points.Length);
    }

    private float AudioWave(Vector3 p, float t)
    {
        float[] spectrum = aSource.GetSpectrumData(maxSize, 0, FFTWindow.BlackmanHarris);
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
        if(volSquare>0 && volSquare > .001)
        {
            // return sum/4f*Mathf.Sin(sum* Mathf.PI * squareRadius * aSource.volume * aSource.volume - 1f) / (2f + 10f * squareRadius);
            return 0.5f + Mathf.Sin(sum * 1000 * Mathf.PI * squareRadius * aSource.volume * aSource.volume - 2f * t) / (2f + 100f * squareRadius);
        }
       else
        {
            return 0.2f * Mathf.Sin(0.2f * Mathf.PI * t);
        }

    }

    //For Debugging -gives generic sine wave (ignores audio file)
    private float Ripple(Vector3 p, float t)
    {
        p.y -= 0.5f;
        p.z -= 0.5f;
        float squareRadius = p.y * p.y + p.z * p.z;
        return Mathf.Sin(4f * Mathf.PI * squareRadius - 2f * t);
    }

}