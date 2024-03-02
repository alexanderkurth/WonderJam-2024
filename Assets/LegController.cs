using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D m_RigidbodyToAttach;

    [SerializeField]
    private HingeJoint2D m_GroundAnchorHinge;

    [SerializeField]
    private HingeJoint2D m_MotorHinge;

    [SerializeField]
    private KeyCode m_AnchorKey;

    [SerializeField]
    private bool IsLeftLeg = false;

    [SerializeField] private float m_StepUpSpeed = 400;
    [SerializeField] private float m_StepDownSpeed = 400;

    private static float k_END_ANGLE_THRESHOLD = 10; // Stop at 10 degrees from the end

    // Cache motors
    private JointMotor2D m_StepUpMotor;
    private JointMotor2D m_StepDownMotor;
    private BodyScript m_BodyRef;
    private short m_IdentificationMask;

    private void Awake()
    {
        Debug.Assert(m_AnchorKey != KeyCode.None, "Please set a key for the Leg " + gameObject.name);
        Debug.Assert(m_RigidbodyToAttach != null, "Please attach a rigidBody to the Leg " + gameObject.name);
        m_MotorHinge.connectedBody = m_RigidbodyToAttach;

        m_BodyRef = m_RigidbodyToAttach.GetComponent<BodyScript>();
        Debug.Assert(m_BodyRef != null, "Please attach a BodyScript to the rigidBody " + m_RigidbodyToAttach.name);
        m_IdentificationMask = m_BodyRef.ObtainMask();

        m_GroundAnchorHinge.enabled = false;
        m_MotorHinge.useMotor = false;

        // Negative speed means up
        m_StepUpSpeed *= -1;

        m_StepUpMotor = new JointMotor2D
        {
            motorSpeed = IsLeftLeg ? m_StepDownSpeed : m_StepUpSpeed,
            maxMotorTorque = 1000
        };
        m_StepDownMotor = new JointMotor2D
        {
            motorSpeed = IsLeftLeg ? m_StepUpSpeed : m_StepDownSpeed,
            maxMotorTorque = 1000
        };
    }

    void AnchorLeg()
    {
        m_GroundAnchorHinge.enabled = true;
    }
    void ReleaseLeg()
    {
        m_GroundAnchorHinge.enabled = false;
    }

    private void StartMotor(JointMotor2D motor)
    {
        m_MotorHinge.motor = motor;
        m_MotorHinge.useMotor = true;
        m_BodyRef.RaiseMask(m_IdentificationMask);
    }
    private void StopMotor()
    {
        m_MotorHinge.useMotor = false;
        m_RigidbodyToAttach.velocity = Vector2.zero;
        m_BodyRef.LowerMask(m_IdentificationMask);
    }

    bool IsAngleNearTopLimit()
    {
        return Mathf.Abs(m_MotorHinge.limits.min - m_MotorHinge.jointAngle) < k_END_ANGLE_THRESHOLD;
    }
    bool IsAngleNearBottomLimit()
    {
        return Mathf.Abs(m_MotorHinge.limits.max - m_MotorHinge.jointAngle) < k_END_ANGLE_THRESHOLD;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(m_AnchorKey))
        {
            StartLegTravel(stepUp: true);
        }
        else if (Input.GetKeyUp(m_AnchorKey))
        {
            StartLegTravel(stepUp: false);
        }
    }

    void StartLegTravel(bool stepUp)
    {
        // Interrupt all coroutines
        StopAllCoroutines();
        // Stop the motor and leg
        AnchorLeg();
        StopMotor();

        StartCoroutine(stepUp ? LegTravelUp() : LegTravelDown());
    }

    IEnumerator LegTravelUp()
    {
        ReleaseLeg();
        StartMotor(m_StepUpMotor);
        yield return new WaitUntil(IsLeftLeg ? IsAngleNearBottomLimit : IsAngleNearTopLimit);
        StopMotor();
    }
    
    IEnumerator LegTravelDown()
    {
        AnchorLeg();
        StartMotor(m_StepDownMotor);
        yield return new WaitUntil(IsLeftLeg ? IsAngleNearTopLimit : IsAngleNearBottomLimit);
        StopMotor();
    }
}
