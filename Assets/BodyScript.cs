using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyScript : MonoBehaviour
{
    private Rigidbody2D m_Rigidbody;

    // The goal of this script is to keep track of the active motors
    // and stop the body when no motors are active

    // As a counter is not viable due to the repeated usage and eventual stops,
    // We use a mask with one given identifier per leg.
    private short m_ActiveMotorMask = 0;
    private short m_RegisteredLegsCount = 0;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        Debug.Assert(m_Rigidbody != null, "Please attach a rigidBody to the Body " + gameObject.name);
    }

    // Legs will call this to get a mask, and then raise/lower it during their lifetime
    // TODO: Support unregistering legs! (not needed for the prototype)
    public short ObtainMask()
    {
        return (short)(1 << m_RegisteredLegsCount++);
    }

    public void RaiseMask(short mask)
    {
        m_ActiveMotorMask |= mask;
    }

    public void LowerMask(short mask)
    {
        m_ActiveMotorMask &= (short)~mask;
        if (m_ActiveMotorMask == 0)
        {
            m_Rigidbody.velocity = Vector2.zero;
        }
    }
}
