using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    public GameObject m_CameraParent;
    public Transform m_Target;
    public Vector3 _offset = default;

    private bool _isActive = true;
    
    // Start is called before the first frame update
    void Start()
    {
        if (m_CameraParent == null){
            m_CameraParent = this.gameObject;
        }
        
        if(m_Target == null){
            _isActive = false;
            return;
        }

        Vector3 targetPos = m_Target.position;
        targetPos += _offset;
        m_CameraParent.transform.position = targetPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isActive)
        {
            return;
        }

        Vector3 targetPos = m_Target.position;
        targetPos += _offset;
        m_CameraParent.transform.position = targetPos;
    }

    public void SetCameraActive(bool isActive)
    {
        _isActive = isActive;
    }
}
