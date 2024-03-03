using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class InteractorComponent : MonoBehaviour
{
    private InteractableComponent m_Target;
    public InteractableComponent Target { get { return m_Target; } }
    public float offScreenCooldown = 1f;

    private Coroutine _isOffScreenCo = null;

    [SerializeField]
    public KeyPessPrompt m_KeyPressPrompt;

    // Start is called before the first frame update
    void Start()
    {
        if (m_KeyPressPrompt == null)
        {
            m_KeyPressPrompt = FindObjectOfType<KeyPessPrompt>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameMode.Instance.isGameOver) return;
        CheckOffScreen();
    }

    public void RegisterInteractable(InteractableComponent target)
    {
        if (m_Target != null)
        {
            RemoveInteractable(target);
        }

        m_Target = target;
        target.OnInteractionAdded();
    }

    public bool IsTargetFilled()
    {
        return m_Target != null;
    }

    public Vector3 GetTargetPosition()
    {
        if (m_Target != null)
        {
            return m_Target.transform.position;
        }
        Debug.LogError("Target is null");
        return Vector3.zero;
    }

    public void RemoveInteractable(InteractableComponent target)
    {
        m_Target = null;
        target.OnInteractionRemoved();
    }

    public void TriggerInteraction()
    {
        //Debug.Log("INTERACTION ADDED");
        if (m_Target != null)
        {
            m_Target.TriggerInteraction();
            m_KeyPressPrompt.PressButtonEffect();
        }
    }

    public void OnInteractionChanged(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (m_Target != null)
            {
                TriggerInteraction();
                //Pressed
            }
        }
    }

    public void CheckOffScreen()
    {
        if (GameMode.Instance.GameState != GameState.InProgress)
        {
            return;
        }

        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        bool isOffScreen = !Screen.safeArea.Contains(pos);
        if (isOffScreen)
        {
            if (_isOffScreenCo == null)
            {
                _isOffScreenCo = StartCoroutine(CheckOffScreenCo());
            }
        }
        else
        {
            if (_isOffScreenCo != null)
            {
                StopCoroutine(_isOffScreenCo);
                _isOffScreenCo = null;
            }
        }
    }

    private IEnumerator CheckOffScreenCo()
    {
        if (Random.Range(0, 2) > 0)
        {
            GameMode.Instance.DisplayMessage(MessageEnum.OutOfScreenWarning, 0f, true);
        }

        yield return new WaitForSeconds(offScreenCooldown);
        GameMode.Instance.GameOver(GameMode.GameOverCondition.OutOfScreen);
    }
}
