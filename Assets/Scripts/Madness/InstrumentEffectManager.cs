using UnityEngine;

public class InstrumentEffectManager : MonoBehaviour
{
    private float m_NextActionTime = 0.0f;
    private mMadnessManager m_MadnessManager;

    [SerializeField]
    private float m_MusicEffectTickSeconds = 1.0f;

    [SerializeField]
    private int m_MadnessMaxAmountPerSecond = 1;

    [SerializeField]
    private float m_MaxDistanceToTrigger = 170.0f;

    [SerializeField]
    private GameObject m_Chevalier;

    [SerializeField]
    private GameObject m_Ecuyer;

    [SerializeField]
    private AvailableObject currentInstrument = AvailableObject.None;

    [System.Serializable]
    class InstrumentMusics
    {
        public AudioSource m_MusicAudioSource;
        public AudioClip m_ClassicMusic;
        public AudioClip m_FluteMusic;
        public AudioClip m_ViolinMusic;
        public AudioClip m_TrumpetMusic;
        public AudioClip m_CornemuseMusic;
    }

    [SerializeField]
    private InstrumentMusics m_InstrumentMusics;

    private void Start()
    {
        if (m_MadnessManager == null)
            m_MadnessManager = mMadnessManager.Instance;

        if (m_Chevalier == null)
            m_Chevalier = FindObjectOfType<ChevalierMove>().gameObject;

        if (m_Ecuyer == null)
            m_Ecuyer = GameObject.Find("PlayerArmature");

        if (m_InstrumentMusics.m_MusicAudioSource == null)
            m_InstrumentMusics.m_MusicAudioSource = GetComponents<AudioSource>()[2];

        SetCurrentInstrument();
    }

    private void SetCurrentInstrument()
    {
        Inventory inventory = Inventory.Instance;
        currentInstrument = inventory.GetCurrentInstruments();
        if (currentInstrument == AvailableObject.None) return;

        switch (currentInstrument)
        {
            case AvailableObject.Flute:
                m_MadnessMaxAmountPerSecond = Mathf.Abs(m_MadnessMaxAmountPerSecond) * -1;
                // Play flute sound
                m_InstrumentMusics.m_MusicAudioSource.clip = m_InstrumentMusics.m_FluteMusic;
                break;
            case AvailableObject.Violin:
                m_MadnessMaxAmountPerSecond = Mathf.Abs(m_MadnessMaxAmountPerSecond) * -1;
                // Play violin sound
                m_InstrumentMusics.m_MusicAudioSource.clip = m_InstrumentMusics.m_ViolinMusic;
                break;
            case AvailableObject.Trumpet:
                m_MadnessMaxAmountPerSecond = Mathf.Abs(m_MadnessMaxAmountPerSecond);
                // Play trumpet sound
                m_InstrumentMusics.m_MusicAudioSource.clip = m_InstrumentMusics.m_TrumpetMusic;
                break;
            case AvailableObject.Cornemuse:
                m_MadnessMaxAmountPerSecond = Mathf.Abs(m_MadnessMaxAmountPerSecond);
                // Play cornemuse sound
                m_InstrumentMusics.m_MusicAudioSource.clip = m_InstrumentMusics.m_CornemuseMusic;
                break;
            case AvailableObject.None:
            default:
                // Play classic sound
                //m_InstrumentMusics.m_MusicAudioSource.clip = m_InstrumentMusics.m_ClassicMusic;
                break;
        }
        m_InstrumentMusics.m_MusicAudioSource.Play();
    }

    private void Update()
    {
        if (m_MadnessManager == null || currentInstrument == AvailableObject.None) return;

        GameMode gameMode = GameMode.Instance;
        if (gameMode == null || gameMode.GameState == GameState.InShop) return;

        if (Time.time > m_NextActionTime)
        {
            m_NextActionTime += m_MusicEffectTickSeconds;
            // Take the distance between the player and the chevalier world to screen
            Vector3 chevalierPosScreen = Camera.main.WorldToScreenPoint(m_Chevalier.transform.position);
            Vector3 ecuyerPosScreen = Camera.main.WorldToScreenPoint(m_Ecuyer.transform.position);
            float distance = Vector3.Distance(chevalierPosScreen, ecuyerPosScreen);
            if (distance < m_MaxDistanceToTrigger)
            {
                float scalar = Mathf.Round((1.0f - (distance / m_MaxDistanceToTrigger)) * 100.0f) / 100.0f;
                int madnessAmount = Mathf.RoundToInt(Mathf.Abs(m_MadnessMaxAmountPerSecond * scalar));
                if (m_MadnessMaxAmountPerSecond < 0)
                {
                    m_MadnessManager.ReduceMadnessLevel(Mathf.RoundToInt(madnessAmount));
                }
                else
                {
                    m_MadnessManager.IncreaseMadnessLevel(Mathf.RoundToInt(madnessAmount));
                }
            }

        }
    }
}