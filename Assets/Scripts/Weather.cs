using UnityEngine;

public class Weather : MonoBehaviour
{
    public static Weather Instance { get; set; }

    public Color lightColor, darkColor, backColor;

    public Color[] colorTab;
    //1 - Raining
    public ParticleSystem rain;
    //ParticleSystem.EmissionModule rainEmi;
    //2 - Snowing
    public ParticleSystem snow;
    //3 - Stars
    public ParticleSystem star;
    //4 - Lightning
    public ParticleSystem lightning;

    //Profile --> {colorIndex, particleType, emissionParam}
    int i;
    short[] profile;
    readonly short[][] profileTab = {
        new short [] { 6, 4 },
        new short [] { 0, 1 },
        new short [] { 3, 2 },
        new short [] { 9, 3 }
    };

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        i = 0;
        profile = profileTab[i];

        backColor = colorTab[profile[0]];
        lightColor = colorTab[profile[0] + 1];
        darkColor = colorTab[profile[0] + 2];

        Camera.main.backgroundColor = backColor;

        //rainEmi = rain.emission;
        //rain.Play();
    }

    public void Play()
    {
        switch (profile[1])
        {
            case 1://Raining
                //if (rain.isPlaying)
                //{
                //    rainEmi.rateOverTime = 50;
                //    rain.Play();
                //}
                //else
                //{
                    rain.Play();
                //}
                break;
            case 2://Snowing
                snow.Play();
                break;
            case 3://Stars
                star.Play();
                break;
            case 4://Lightning
                lightning.Play();
                break;
        }
    }

    public void Stop()
    {
        switch (profile[1])
        {
            case 1:
                rain.Stop();
                rain.Clear();
                break;
            case 2:
                snow.Stop();
                snow.Clear();
                break;
            case 3:
                star.Stop();
                star.Clear();
                break;
            case 4:
                lightning.Stop();
                lightning.Clear();
                break;
        }
    }

    public void Retry()
    {
        i++;
        if (i == 4)
            i = 0;

        profile = profileTab[i];

        backColor = colorTab[profile[0]];
        lightColor = colorTab[profile[0] + 1];
        darkColor = colorTab[profile[0] + 2];

        Camera.main.backgroundColor = backColor;

        Play();
    }
}
