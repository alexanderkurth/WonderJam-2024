using UnityEngine;
using UnityEngine.Events;

public class InteractionComponent : MonoBehaviour
{
    [SerializeField] CircleCollider2D m_Collider;

    [Header("Custom Events")]
    public UnityEvent OnEnterEvent;
    public UnityEvent OnExitEvent;
    public UnityEvent OnStayEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter2D" + collision.name);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("OnTriggerExit2D" + collision.name);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("OnTriggerStay2D" + collision.name);
    }
}
