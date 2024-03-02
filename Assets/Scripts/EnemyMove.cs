using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [SerializeField]
    public GameObject m_Target;
    [SerializeField, Range(0, 10)]
    private float m_Speed = 1.0f;
    [SerializeField]
    private ParticleSystem m_ParticleSystem;
    [SerializeField]
    public GameObject m_Interactable;
    [SerializeField]
    public GameObject m_EnemySpriteBody;
    [SerializeField]
    private Collider m_AliveCollider;

    private Vector3 m_Direction;

    public bool m_IsOnGround = false;

    public AudioSource ourge;

    // Start is called before the first frame update
    void Start()
    {
        if (m_Target == null)
        {
            m_Direction = Vector3.forward;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!m_IsOnGround)
        {
            if (m_Target != null)
            {
                m_Direction = (m_Target.transform.position - transform.position).normalized;
            }

            transform.Translate(m_Direction * Time.fixedDeltaTime * m_Speed);

            m_EnemySpriteBody.transform.forward = m_Direction;
        }
    }

    public void FallToGround()
    {
        m_EnemySpriteBody.SetActive(false);
        Vector3 rotation = m_Interactable.transform.rotation.eulerAngles;
        rotation.y = Random.Range(0, 360f);
        m_Interactable.transform.rotation = Quaternion.Euler(rotation);
        m_Interactable.SetActive(true);
        m_AliveCollider.enabled = false;

        ourge.Play();
    }
}
