using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MadnessPostProcessManager : MonoBehaviour
{
    [SerializeField]
    Volume m_Volume;

    [SerializeField]
    Vignette m_Vignette;

    [SerializeField]
    ColorAdjustments m_ColorAdjustement;

    [SerializeField]
    int min;
    [SerializeField]
    int max;

    [SerializeField]
    FilmGrain m_FilmGrain;

    public Transform m_Target;

    [SerializeField]
    float DeisredR = 192;
    [SerializeField]
    float DeisredG = 112;
    [SerializeField]
    float DeisredB = 112;

    [SerializeField]
    private Camera m_Camera;


    void Start()
    {
        if(m_Camera == null)
        {
            m_Camera = Camera.main;
        }

        m_Volume.profile.TryGet<Vignette>(out m_Vignette);
        m_Volume.profile.TryGet<ColorAdjustments>(out m_ColorAdjustement);
        m_Volume.profile.TryGet<FilmGrain>(out m_FilmGrain);
    }

    private void Update()
    {
        Vector3 objectToScreenPos = m_Camera.WorldToScreenPoint(m_Target.position);
        m_Vignette.center.value = new Vector2(objectToScreenPos.x / Screen.width, objectToScreenPos.y / Screen.height);
    }

    public void UpdateVignette(float intensity)
    {
        m_Vignette.intensity.value = intensity;
    }

    public void UpdateColor(float intensity)
    {
        float R = (255 - (255 - DeisredR) * intensity) / 255;
        float G = (255 - (255 - DeisredG) * intensity) / 255;
        float B = (255 - (255 - DeisredB) * intensity) / 255; 

        m_ColorAdjustement.colorFilter.value = new Color(R,G,B, intensity);

        m_ColorAdjustement.saturation.value = -1 * (intensity * 100);
    }

    public void UpdateFilmGrain(float intensity)
    {
        m_FilmGrain.intensity.value = intensity;
    }
}
