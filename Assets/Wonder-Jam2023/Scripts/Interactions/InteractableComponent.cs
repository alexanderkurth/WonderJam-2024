using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableComponent : MonoBehaviour
{
    [Tooltip("In seconds")]
    [SerializeField]
    private float m_MashTotalTime = 2.0f;

    [Tooltip("In seconds")]
    [SerializeField]
    public float m_MaxTimeBewteenMash = 0.2f;

    [SerializeField]
    UnityEvent m_OnInteractionSucessEvent;

    [SerializeField]
    public float m_LootableMoney = 0.0f;

    private bool m_IsInteractionStarted = false;
    private float m_MashTimer = 0.0f;
    private bool m_InteractionDone = false;
    private float m_TimeSinceLastInput = 0.0f;

    [SerializeField]
    private Color m_AllyColor;

    [SerializeField]
    private Color m_EnnemiColor;

    [SerializeField]
    private bool m_IsAlly = false;

    [SerializeField]
    private SpriteRenderer[] m_Sprites;

    [SerializeField]
    private GameObject m_BloodFX;

    public AllyBehavior AllyCompIfAlly;

    private InteractorComponent m_tempInteractor;

    public void Start()
    {
        foreach (var sprite in m_Sprites)
        {
            sprite.color = m_IsAlly ? m_AllyColor : m_EnnemiColor;
        }
    }

    public void Update()
    {
        if (m_InteractionDone)
            return;

        if (m_IsInteractionStarted)
        {
            m_MashTimer += Time.deltaTime;
            m_TimeSinceLastInput += Time.deltaTime;

            if (m_TimeSinceLastInput >= m_MaxTimeBewteenMash)
            {
                OnInteractionFailed();
                return;
            }

            if (m_MashTimer >= m_MashTotalTime)
            {
                OnInteractionSucessfull();
            }
        }
    }

    public void OnInteractionAdded()
    {
    }

    public void OnInteractionRemoved()
    {
        m_IsInteractionStarted = false;
    }

    public void TriggerInteraction()
    {
        if (m_InteractionDone)
            return;

        m_IsInteractionStarted = true;
        m_TimeSinceLastInput = 0.0f;

        if (!m_BloodFX.active)
        {
            m_BloodFX.SetActive(true);
        }
    }

    private void OnInteractionFailed()
    {
        m_MashTimer = 0.0f;
        m_TimeSinceLastInput = 0.0f;

        m_IsInteractionStarted = false;

        if(m_tempInteractor != null)
        {
            m_tempInteractor.m_KeyPressPrompt.ResetPressButtonEffect();
        }
    }

    private void OnInteractionSucessfull()
    {
        RemoveInteractable();

        m_BloodFX.SetActive(false);

        m_InteractionDone = true;
        m_OnInteractionSucessEvent.Invoke();

        if (!m_IsAlly)
        {
            mMadnessManager.Instance.StartPeakMadness();

            mMadnessManager.Instance.IncreaseMadnessLevel(mMadnessManager.Instance.GetIncrease());

            Inventory.Instance.ChangeCurrencyValue(m_LootableMoney);

            GameMode.Instance.OnEnemyKilled();
        }
        else
        {
            mMadnessManager.Instance.ReduceMadnessLevel(mMadnessManager.Instance.GetIncrease() / 2);
            mMadnessManager.Instance.ReduceShakingIntentisy();

            if (AllyCompIfAlly != null)
            {
                AllyCompIfAlly.ChangeState();
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            return;

        if (m_InteractionDone)
            return;

        m_tempInteractor = other.GetComponent<InteractorComponent>();

        if (m_tempInteractor)
        {
            m_tempInteractor.RegisterInteractable(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player")
            return;

        m_tempInteractor = other.GetComponent<InteractorComponent>();
        RemoveInteractable();
    }

    private void RemoveInteractable()
    {
        if (m_tempInteractor)
        {
            if (m_tempInteractor.Target == this)
            {
                m_tempInteractor.RemoveInteractable(this);
            }
        }
    }
    private void ForceRemoveInteractable()
    {
        if (m_tempInteractor)
        {
            m_tempInteractor.RemoveInteractable(this);
        }
    }
}
