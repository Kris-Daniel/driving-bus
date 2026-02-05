using UnityEngine;
using UnityEngine.VFX;

public class WeatherService: MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] float RainIntensity;
    [SerializeField, Range(0f, 1f)] float SnowIntensity;
    [SerializeField, Range(0f, 1f)] float HailIntensity;

    [SerializeField] VisualEffect RainVFX;
    [SerializeField] VisualEffect SnowVFX;
    [SerializeField] VisualEffect HailVFX;

    float PreviousRainIntensity;
    float PreviousHailIntensity;
    float PreviousSnowIntensity;

    void Start()
    {
        InitWeatherVFX();
    }

    void InitWeatherVFX()
    {
        RainVFX.SetFloat("Intensity", RainIntensity);
        HailVFX.SetFloat("Intensity", HailIntensity);
        SnowVFX.SetFloat("Intensity", SnowIntensity);
    }

    void Update()
    {
        if (!Mathf.Approximately(RainIntensity, PreviousRainIntensity))
        {
            PreviousRainIntensity = RainIntensity;
            RainVFX.SetFloat("Intensity", RainIntensity);
        }
        if (!Mathf.Approximately(HailIntensity, PreviousHailIntensity))
        {
            PreviousHailIntensity = HailIntensity;
            HailVFX.SetFloat("Intensity", HailIntensity);
        }
        if (!Mathf.Approximately(SnowIntensity, PreviousSnowIntensity))
        {
            PreviousSnowIntensity = SnowIntensity;
            SnowVFX.SetFloat("Intensity", SnowIntensity);
        }
    }
}