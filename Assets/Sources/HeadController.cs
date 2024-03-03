using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D m_RigidbodyToAttach;

    [SerializeField]
    private HingeJoint2D m_HeadHinge;
    private BodyScript m_BodyRef;

    public void SetBody(BodyScript body)
    {
        m_BodyRef = body;
        m_RigidbodyToAttach = body.GetComponent<Rigidbody2D>();

        Debug.Assert(m_RigidbodyToAttach != null, "Please attach a rigidBody to the Leg " + gameObject.name);

        m_BodyRef = m_RigidbodyToAttach.GetComponent<BodyScript>();
        Debug.Assert(m_BodyRef != null, "Please attach a BodyScript to the rigidBody " + m_RigidbodyToAttach.name);

        m_HeadHinge.connectedBody = m_RigidbodyToAttach;
    }

    public Vector2 GetPivotLocation()
    {
        return m_HeadHinge.anchor;
    }
}
