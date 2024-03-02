using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    private Camera m_Camera;

    [SerializeField]
    [Range(0, 10)]
    private float m_ShakeFrequency = 0;

    [SerializeField]
    private bool m_CanShake = true;

    private bool m_HasFinishedShaking = true;

    private Vector3 m_OriginalPosition;

    private void Start()
    {
        if(m_Camera == null){
            m_Camera = Camera.main;
        }
        m_OriginalPosition = m_Camera.transform.localPosition;
    }

    public void ResetPosition()
    {
        m_Camera.transform.localPosition = m_OriginalPosition;
    }

    private void ShakeCamera()
    {
        float x = m_OriginalPosition.x + Random.insideUnitSphere.x * Time.deltaTime * Mathf.Pow(2,m_ShakeFrequency);
        float y = m_OriginalPosition.y + Random.insideUnitSphere.y * Time.deltaTime * Mathf.Pow(2, m_ShakeFrequency);
        float z = m_OriginalPosition.z + Random.insideUnitSphere.z * Time.deltaTime * 1 / Mathf.Pow(2, m_ShakeFrequency);

        Vector3 pos = new Vector3(x, y, z);

        // Set m_Camera gameobject position to the new position
        m_Camera.transform.localPosition = pos;
    }

    public void SetShakeFrequency(float frequency)
    {
        m_ShakeFrequency = frequency;
    }

    private void Update()
    {
        if (m_CanShake  && m_ShakeFrequency != 0)
        {
            ShakeCamera();
        } 
        else if(m_HasFinishedShaking)
        {
            ResetPosition();
            m_HasFinishedShaking = false;
        }
    }
}