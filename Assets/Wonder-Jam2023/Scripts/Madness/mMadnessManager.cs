using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class mMadnessManager : Singleton<mMadnessManager>
{
    [SerializeField]
    public int m_IncreaseShakingAmount = 4;

    [SerializeField]
    int increaseMadnessLevel = 2;

    [SerializeField]
    private float m_MadnessCurrentLevel = 0;

    [SerializeField]
    private int m_MadnessMaxLevel = 100;

    [SerializeField]
    private int m_StartShakingLevel = 1;

    [Range(0, 20)]
    [SerializeField]
    private int m_MaxShakingIntensity = 20;

    [Range(0, 10)]
    [SerializeField]
    private float mCurrentShakingIntensity = 1;

    public int GetIncrease() { return increaseMadnessLevel; }

    [System.Serializable]
    public class MadnessAudio
    {
        [Header("HeartBeat")]
        public int m_StartHeartBeatLevel = 50;
        public AudioSource m_HeartBeatAudioSource;
        public float m_HeartBeatMinPitch = 0.5f;
        public float m_HeartBeatMaxPitch = 3.0f;

        [Header("Breath")]
        public int m_StartBreathLevel = 50;
        public AudioSource m_BreathAudioSource;
        public AudioClip[] m_BreathAudioClips;
    }

    [SerializeField]
    private MadnessAudio m_MadnessAudio;

    private Coroutine coroutine;
    private Coroutine coroutine2;


    public float GetCurrentMadnessLevel()
    {
        return m_MadnessCurrentLevel;
    }
    public int GetMaxMadnessLevel()
    {
        return m_MadnessMaxLevel;
    }

    public void IncreaseMadnessLevel(int amount = 1)
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        float ratio = (float)m_MadnessCurrentLevel / (float)m_MadnessMaxLevel;

        if(ratio > 0.80)
        {
            coroutine = StartCoroutine(LerpMadness(1));
        }
        else
        {
            coroutine = StartCoroutine(LerpMadness(amount));
        }

        if (m_MadnessCurrentLevel > m_MadnessMaxLevel)
        {
            GameMode.Instance.GameOver(GameMode.GameOverCondition.Madness);
        }
    }

    public void ReduceMadnessLevel(int amount = 1)
    {
        m_MadnessCurrentLevel -= amount;
        if (m_MadnessCurrentLevel < 0)
        {
            m_MadnessCurrentLevel = 0;
        }
    }

    public void ReduceShakingIntentisy(int amount = 1)
    {
        mCurrentShakingIntensity -= amount;
    }

    private void Start()
    {
        if (m_MadnessAudio.m_HeartBeatAudioSource == null)
        {
            m_MadnessAudio.m_HeartBeatAudioSource = GetComponents<AudioSource>()[0];
        }

        if (m_MadnessAudio.m_BreathAudioSource == null)
        {
            // Get first audio source in game object
            m_MadnessAudio.m_BreathAudioSource = GetComponents<AudioSource>()[1];
        }
    }

    private void Update()
    {
        MadnessPostProcessManager madnessPostProcessManager = GetComponent<MadnessPostProcessManager>();
        madnessPostProcessManager.UpdateVignette((float)m_MadnessCurrentLevel / (float)m_MadnessMaxLevel);
        madnessPostProcessManager.UpdateColor((float)m_MadnessCurrentLevel / (float)m_MadnessMaxLevel);
        madnessPostProcessManager.UpdateFilmGrain((float)m_MadnessCurrentLevel / (float)m_MadnessMaxLevel);

        if (m_MadnessCurrentLevel > m_StartShakingLevel)
        {
            // Should be between 0 and mCurrentShakingIntensity
            float cameraShakingIntensity = ((float)(m_MadnessCurrentLevel - m_StartShakingLevel) / (float)(m_MadnessMaxLevel - m_StartShakingLevel)) * (float)mCurrentShakingIntensity;
            CameraShake cameraShake = GetComponent<CameraShake>();
            cameraShake.SetShakeFrequency(cameraShakingIntensity);
        }
        else
        {
            CameraShake cameraShake = GetComponent<CameraShake>();
            cameraShake.SetShakeFrequency(0);
        }

        float heartBeatScale = (float)(m_MadnessCurrentLevel - m_MadnessAudio.m_StartHeartBeatLevel) / (float)(m_MadnessMaxLevel - m_MadnessAudio.m_StartHeartBeatLevel);

        if (m_MadnessCurrentLevel > m_MadnessAudio.m_StartHeartBeatLevel - 1)
        {
            // Add audio volume based on current madness level
            m_MadnessAudio.m_HeartBeatAudioSource.volume = heartBeatScale;
            // increase audio speed based on current madness level
            m_MadnessAudio.m_HeartBeatAudioSource.pitch = Mathf.Lerp(m_MadnessAudio.m_HeartBeatMinPitch, m_MadnessAudio.m_HeartBeatMaxPitch, heartBeatScale);
        }

        if(m_MadnessCurrentLevel > m_MadnessAudio.m_StartBreathLevel - 1 )
        {
            float breathBeatScale = (float)(m_MadnessCurrentLevel - m_MadnessAudio.m_StartBreathLevel) / (float)(m_MadnessMaxLevel - m_MadnessAudio.m_StartBreathLevel);

            // Plays a random breath sound effect with a exponentially increase chance of playing
            if (Random.Range(0, 100) < (breathBeatScale * 100))
            {
                if (m_MadnessAudio.m_BreathAudioSource.isPlaying == false)
                {
                    m_MadnessAudio.m_BreathAudioSource.PlayOneShot(m_MadnessAudio.m_BreathAudioClips[Random.Range(0, m_MadnessAudio.m_BreathAudioClips.Length)]);
                }
            }
        }
    }

    public void StartPeakMadness()
    {
        if(coroutine2 == null)
        {
            coroutine2 = StartCoroutine(PeakMadness());
        }
        else
        {
            StopCoroutine(coroutine2);
            coroutine2 = null;
        }
    }

    public IEnumerator PeakMadness()
    {
        float lerp = 0f;

        float targetIntensity = (mCurrentShakingIntensity + m_IncreaseShakingAmount < m_MaxShakingIntensity + 1) ? mCurrentShakingIntensity + m_IncreaseShakingAmount : m_MaxShakingIntensity;
        float baseIntensity = mCurrentShakingIntensity;

        while (lerp < 1.0f)
        {
            lerp += Time.unscaledDeltaTime / 1.0f;

            mCurrentShakingIntensity = Mathf.Lerp(baseIntensity, targetIntensity, lerp); ;
            yield return null;
        }

        yield return new WaitForSeconds(0.25f);

        while (lerp > 0f)
        {
            lerp -= Time.unscaledDeltaTime / 1.0f;

            mCurrentShakingIntensity = Mathf.Lerp(baseIntensity, targetIntensity, lerp); ;
            yield return null;
        }
        mCurrentShakingIntensity += 1.0f;
        coroutine2 = null;

        yield break;
    }

    public void Reset()
    {
        m_MadnessCurrentLevel = m_StartShakingLevel / 2;
        mCurrentShakingIntensity = 1;
        m_MadnessAudio.m_HeartBeatAudioSource.volume = 0f;

    }

    private IEnumerator LerpMadness(float amount)
    {
        float lerp = 0f;
        float currentLevel = m_MadnessCurrentLevel;

        while (lerp < 1f)
        {
            lerp += Time.unscaledDeltaTime / 1.0f;

            m_MadnessCurrentLevel = Mathf.Lerp(currentLevel, currentLevel+ amount, lerp);

            yield return null;
        }
        coroutine = null;

        yield break;
    }
}