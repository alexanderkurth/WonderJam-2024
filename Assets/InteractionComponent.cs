using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionComponent : MonoBehaviour
{
    [Header("Custom Events")]
    public Sprite m_InteractSprite;
    public string m_TargetTag;
    public Dictionary<string, GameObject> m_Map = new Dictionary<string, GameObject>();

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
        
        if(!m_Map.TryGetValue(m_TargetTag, out _ ))
        {
            m_Map.Add(m_TargetTag, collision.gameObject);
        }
        OnEnterEvent.Invoke();

        Debug.Log("OnTriggerEnter2D" + collision.name);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != m_TargetTag)
        {
            return;
        }

            OnStayEvent.Invoke();
        
        Debug.Log("OnTriggerExit2D" + collision.name);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag != m_TargetTag)
        {
            return;
        }

            OnExitEvent.Invoke();
        
        Debug.Log("OnTriggerStay2D" + collision.name);
    }
}
