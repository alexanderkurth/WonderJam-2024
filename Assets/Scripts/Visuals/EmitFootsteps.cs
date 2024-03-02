using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitFootsteps : MonoBehaviour
{
    [SerializeField]
    GameObject m_FootstepPrefab;

    [SerializeField]
    float m_SpawnDist = 10.0f;

    [SerializeField]
    float m_PeriodRefreshSecs = 0.5f;

    [SerializeField]
    Transform m_LeftSpawnPos;
    [SerializeField]
    Transform m_RightSpawnPos;

    [SerializeField]
    bool m_IsQuadriped = false;

    Vector3 m_LastPos;

    int m_counter = 0;
    GameObject parentHolder = null;

    void Awake()
    {
        m_LastPos = transform.position;
        StartCoroutine(FootstepSpawn());
        parentHolder = new GameObject("FootstepContainer");
        parentHolder.transform.parent = null; //reparent to root
    }

    void SpawnFootstep()
    {
        m_counter++;

        if(m_IsQuadriped)
        {
            Transform front = m_counter % 2 == 0 ? m_LeftSpawnPos : m_RightSpawnPos;
            GameObject stepFront = Instantiate(m_FootstepPrefab, front.position + new Vector3(0, 0, 1), transform.rotation);
            stepFront.transform.parent = parentHolder.transform;
        }
        Transform back = m_counter % 2 == 1 ? m_LeftSpawnPos : m_RightSpawnPos;
        GameObject step = Instantiate(m_FootstepPrefab, back.position, transform.rotation);
        step.transform.parent = parentHolder.transform;
    }

    IEnumerator FootstepSpawn()
    {
        while (true)
        {
            //Wait .5sec and check distance
            float diff = Vector3.Distance(transform.position, m_LastPos);

            if (Mathf.Abs(diff) > m_SpawnDist)
            {
                SpawnFootstep();
                m_LastPos = transform.position;
            }
            yield return new WaitForSeconds(m_PeriodRefreshSecs);
        }
    }
}