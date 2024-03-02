using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyPessPrompt : MonoBehaviour
{
    [SerializeField]
    private Image m_InputImageDisplayer;

    //Keyboard Input Sprite
    [SerializeField]
    private Sprite m_KeyboardInputSprite;

    //Controller Input Sprite
    [SerializeField]
    private Sprite m_ControllerInputSprite;

    [SerializeField]
    private InputDeviceManager m_InputDeviceManager;

    [SerializeField]
    private InteractorComponent m_InteractorComponent;

    [SerializeField]
    private Camera m_Camera;

    [SerializeField]
    private float m_AnimationIntensity = 0.1f;

    public bool m_shouldDisplayKeyboardInputImage = false;

    [SerializeField]
    private Transform m_DisplayPosition;

    [SerializeField]
    private Vector3 m_DisplayOffset;

    private Vector3 m_Distortion;

    private void Start() {
        if(m_InputImageDisplayer == null)
        {
            m_InputImageDisplayer = GetComponent<Image>();        
        }
        if(m_InputDeviceManager == null)
        {
            m_InputDeviceManager = FindObjectOfType<InputDeviceManager>();
        }
        if(m_Camera == null)
        {
            m_Camera = Camera.main;
        }

        if(m_InteractorComponent == null)
        {
            m_InteractorComponent = FindObjectOfType<InteractorComponent>();
        }

        ResetPressButtonEffect();
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_KeyboardInputSprite == null && m_ControllerInputSprite == null)
        {
            Debug.LogError("Keyboard or Controller Input Sprite not set");
            return;
        }

        m_shouldDisplayKeyboardInputImage = m_InteractorComponent.IsTargetFilled();

        if(m_shouldDisplayKeyboardInputImage)
        {
            Vector3 targetPosition = m_Camera.WorldToScreenPoint(m_DisplayPosition.position);
            // Setting recttransform position
            transform.position = targetPosition + m_DisplayOffset;
            m_InputImageDisplayer.enabled = true;

            if (m_InputDeviceManager.m_isKeyboardAndMouse)
            {
                m_InputImageDisplayer.sprite = m_KeyboardInputSprite;
            }
            else
            {
                m_InputImageDisplayer.sprite = m_ControllerInputSprite;
            }
            AnimateButton(m_Distortion);
        }
        else
        {
            m_InputImageDisplayer.enabled = false;
            ResetPressButtonEffect();
        }
    }

    private void AnimateButton(Vector3 distortion)
    {
        float scale = Mathf.PingPong(Time.time, m_AnimationIntensity);
        m_InputImageDisplayer.transform.localScale = new Vector3(distortion.x - scale, distortion.y - scale, distortion.z - scale);
    }

    public void PressButtonEffect()
    {
        m_InputImageDisplayer.color = Color.green;
        m_Distortion = new Vector3(0.6f, 0.6f, 0.6f);
    }

    public void ResetPressButtonEffect()
    {
        m_Distortion = new Vector3(0.4f, 0.4f, 0.4f);
        m_InputImageDisplayer.color = Color.white;
    }
}
