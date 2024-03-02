using UnityEngine;
using UnityEngine.Events;

public class InteractionComponent : MonoBehaviour
{
    [Header("Custom Events")]
    public Sprite m_InteractSprite;
    public string m_TargetTag;

    public InteractionComponent m_Target;

    [Header("Custom Events")]
    public UnityEvent OnEnterEvent;
    public UnityEvent OnExitEvent;
    public UnityEvent OnStayEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag != m_TargetTag)
        {
            return;
        }
        m_Target = collision.GetComponent<InteractionComponent>();
        if(m_Target && m_Target.OnEnterEvent != null)
        {
            m_Target.OnEnterEvent.Invoke();
        }

        Debug.Log("OnTriggerEnter2D" + collision.name);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != m_TargetTag)
        {
            return;
        }

        if (m_Target && m_Target.OnStayEvent != null)
        {
            m_Target.OnStayEvent.Invoke();
        }
        Debug.Log("OnTriggerExit2D" + collision.name);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag != m_TargetTag)
        {
            return;
        }

        if (m_Target && m_Target.OnExitEvent != null)
        {
            m_Target.OnExitEvent.Invoke();
        }
        m_Target = null;

        Debug.Log("OnTriggerStay2D" + collision.name);
    }
}
