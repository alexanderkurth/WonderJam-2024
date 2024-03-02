using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChevalierMove : MonoBehaviour
{
    [SerializeField, Range(0, 10)]
    public float m_Speed = 1.0f;
    [SerializeField]
    private float m_LearpMin = -50.0f;
    [SerializeField]
    private float m_LearpMax = 50.0f;
    [SerializeField]
    private float m_LerpDuration = 3.0f;
    [SerializeField]
    private GameObject m_TargetPivot;
    [SerializeField]
    private GameObject m_VisualizeRoot;

    private Vector3 m_Direction;
    private float m_TimeElapsed;
    private bool m_CanMove = false;
    [SerializeField]
    private Animator m_Animator;

    private float m_TimeBeforeStart = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        if(m_Direction == Vector3.zero)
        {
            m_Direction = new Vector3(0,1,0);
        }

        StartCoroutine(StartMovement());
    }

    public void SetStartTimer(float time)
    {
        m_TimeBeforeStart = time;
    }

    IEnumerator StartMovement()
    {
        yield return new WaitForSeconds(m_TimeBeforeStart);
        m_CanMove = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!m_CanMove)
        {
            return;
        }

        float lerpValue = 0.0f;
        if (m_TimeElapsed < m_LerpDuration)
        {
            lerpValue = Mathf.Lerp(m_LearpMin, m_LearpMax, m_TimeElapsed / m_LerpDuration);
            m_TimeElapsed += Time.deltaTime;
        }
        else
        {
            m_TimeElapsed = 0.0f;
            m_LearpMin = -m_LearpMin;
            lerpValue = m_LearpMax;
            m_LearpMax = -m_LearpMax;
        }

        //m_Direction = new Vector3(m_TargetPivot.transform.localPosition.x + lerpValue, m_TargetPivot.transform.localPosition.y, m_TargetPivot.transform.localPosition.z).normalized;
        //m_Direction = new Vector3(m_TargetPivot.transform.localPosition.x, m_TargetPivot.transform.localPosition.y  + lerpValue, m_TargetPivot.transform.localPosition.z).normalized;

        transform.Translate(Vector3.up * Time.fixedDeltaTime * m_Speed);
        //m_VisualizeRoot.transform.forward = m_Direction;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            m_Animator.SetTrigger("Attack");
            EnemyMove enemy = other.GetComponent<EnemyMove>();
            enemy.m_IsOnGround = true;
            enemy.FallToGround();
        }
    }
}
