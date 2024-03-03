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

    [Header("Sounds")]
    [SerializeField]
    private AK.Wwise.Event FootStepEvent = null;

    [SerializeField] GameObject m_FootstepPrefab;
    Queue<GameObject> m_Footsteps = new Queue<GameObject>(20);
    int m_MaxFootsteps = 20;
    int m_FootstepsCount = 0;
    public bool IsLeft => IsLeftLeg;

    [SerializeField] private float m_StepUpSpeed = 400;
    [SerializeField] private float m_StepDownSpeed = 400;
    [SerializeField] private float m_ImpulseForce = 200;

    private static float k_END_ANGLE_THRESHOLD = 10; // Stop at 10 degrees from the end

    // Cache motors
    private JointMotor2D m_StepUpMotor;
    private JointMotor2D m_StepDownMotor;
    private BodyScript m_BodyRef;
    private short m_IdentificationMask = -1;

    public bool IsInitialized => m_IdentificationMask != -1;
    public bool InitOnStart = false;

#if UNITY_EDITOR
    [ContextMenu("ReloadSpeeds")]
    void ReloadSpeeds()
    {
        m_StepUpMotor = new JointMotor2D
        {
            motorSpeed = IsLeftLeg ? m_StepDownSpeed : m_StepUpSpeed,
            maxMotorTorque = 500
        };
        m_StepDownMotor = new JointMotor2D
        {
            motorSpeed = IsLeftLeg ? m_StepUpSpeed : m_StepDownSpeed,
            maxMotorTorque = 500
        };
    }
#endif

    private void Start()
    {
        if (InitOnStart)
        {
            BodyScript body = m_RigidbodyToAttach.GetComponent<BodyScript>();
            SetBody(body);
        }
    }


    public void SetBody(BodyScript body)
    {
        m_BodyRef = body;
        m_IdentificationMask = m_BodyRef.ObtainMask();
        m_RigidbodyToAttach = body.GetComponent<Rigidbody2D>();

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

        StartLegTravel(stepUp: true);
    }

    void SpawnFootStep()
    {
        float fwdOffset = 0.01f;
        Vector3 positionForSpawn = new Vector3(m_GroundAnchorHinge.anchor.x, m_GroundAnchorHinge.anchor.y, 0);
        positionForSpawn = transform.TransformPoint(positionForSpawn);
        positionForSpawn.z = fwdOffset;

        // Get rotation from transform.left
        Quaternion quaternion = Quaternion.LookRotation(transform.forward, IsLeft ? transform.up : -transform.up);

        GameObject step = Instantiate(m_FootstepPrefab,positionForSpawn, quaternion);
        m_Footsteps.Enqueue(step);

        // Call wwise event
        FootStepEvent?.Post(step);

        if (m_FootstepsCount < m_MaxFootsteps)
        {
            m_FootstepsCount++;
        }
        else
        {
            GameObject toDestroy = m_Footsteps.Dequeue();
            Destroy(toDestroy);
        }
    }

    void AnchorLeg()
    {
        m_GroundAnchorHinge.enabled = true;
        // Spawn footstep
        SpawnFootStep();
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
        if (IsInitialized == false)
        {
            // Not initialized
            return;
        }

        if (Input.GetKeyUp(m_AnchorKey))
        {
            StartLegTravel(stepUp: true);
        }
        else if (Input.GetKeyDown(m_AnchorKey))
        {
            StartLegTravel(stepUp: false);
        }
    }

    public void OnDisable()
    {
        if (IsInitialized == false)
        {
            // Not initialized
            return;
        }
        StopAllCoroutines();
        StopMotor();
        ReleaseLeg();
    }

    public void StartLegTravel(bool stepUp)
    {
        Debug.Assert(IsInitialized, "Leg " + gameObject.name + " is not initialized");

        // Interrupt all coroutines
        StopAllCoroutines();
        // Stop the motor and leg
        StopMotor();
        ReleaseLeg();

        StartCoroutine(stepUp ? LegTravelUp() : LegTravelDown());
    }

    public Vector2 GetPivotLocation()
    {
        return m_MotorHinge.anchor;
    }

    IEnumerator LegTravelUp()
    { 
        StartMotor(m_StepUpMotor);
        yield return new WaitUntil(IsLeftLeg ? IsAngleNearBottomLimit : IsAngleNearTopLimit);
        StopMotor();
    }
    
    IEnumerator LegTravelDown()
    {
        // Travel Up until the limit - prevent the physics applied in between routines
        StartMotor(m_StepUpMotor);
        yield return new WaitUntil(IsLeftLeg ? IsAngleNearBottomLimit : IsAngleNearTopLimit);
        StopMotor();

        // Release the leg and apply an impulse
        AnchorLeg();
        m_RigidbodyToAttach.velocity = Vector2.zero;
        m_RigidbodyToAttach.AddRelativeForce(Vector2.up * m_ImpulseForce, ForceMode2D.Impulse);

        StartMotor(m_StepDownMotor);
        yield return new WaitUntil(IsLeftLeg ? IsAngleNearTopLimit : IsAngleNearBottomLimit);
        StopMotor();
        ReleaseLeg();
    }
}
