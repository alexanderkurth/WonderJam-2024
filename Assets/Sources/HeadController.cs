using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D m_RigidbodyToAttach;

    [SerializeField]
    private FixedJoint2D m_HeadJoint;

    private void Start()
    {
        if (m_RigidbodyToAttach != null)
        {
            SetBody(m_RigidbodyToAttach);
        }
    }

    public void SetBody(Rigidbody2D body)
    {
        m_RigidbodyToAttach = body;

        Debug.Assert(m_RigidbodyToAttach != null, "Please attach a valid rigidBody to " + gameObject.name);
        m_HeadJoint.connectedBody = m_RigidbodyToAttach;
    }

    public Vector2 GetPivotLocation()
    {
        return m_HeadJoint.anchor;
    }
}
