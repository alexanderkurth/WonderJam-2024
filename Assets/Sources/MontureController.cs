using game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MontureController : MonoBehaviour
{
    [SerializeField] BodyPartSlot headSlot;
    [SerializeField] BodyPartSlot bodySlot;
    [SerializeField] BodyPartSlot FrontLeftLegSlot;
    [SerializeField] BodyPartSlot FrontRightLegSlot;
    [SerializeField] BodyPartSlot BackLeftLegSlot;
    [SerializeField] BodyPartSlot BackRightLegSlot;
    [SerializeField] FixedJoint2D m_Saddle;
    [SerializeField] GameObject _outline;

    // List to reference all slots
    private List<BodyPartSlot> slots = new List<BodyPartSlot>();

    private int NbSlotsEquipped = 0;

    BodyScript m_BodyScript;
    private AnimalDatas m_AnimalDatas;

    public TeamID TeamID;
    public AnimalType animalType;

    public void SetOutlineVisibility(bool b)
     {
        _outline.SetActive(b);
    }

#if UNITY_EDITOR
    [ContextMenu("TestBodyPart")]
    void TestAttachBodyPart()
    {
        AttachBodyPart(animalType);
    }
#endif

    public Vector3 GetSaddlePosition()
    {
        return m_Saddle.transform.position;
    }
    public Transform GetSaddleTransform()
    {
        return m_Saddle.transform;
    }

    private void Start()
    {
        InteractionManager2.Instance?.AddSaddle(this);

        NbSlotsEquipped = 0;

        m_AnimalDatas = GameManager.Instance?.GetAnimalDatas();

        // Fill slots list
        slots.Add(headSlot);
        slots.Add(bodySlot);
        slots.Add(FrontLeftLegSlot);
        slots.Add(FrontRightLegSlot);
        slots.Add(BackLeftLegSlot);
        slots.Add(BackRightLegSlot);
    }

    void OrganiseSlots(BodyScript body)
    {
        // Get the collider of the body as AABB
        var bodyCollider = body.GetComponent<BoxCollider2D>();
        body.transform.localPosition = Vector3.zero;
        bodySlot.transform.position = bodyCollider.bounds.center;

        Vector3 bodyCenter = bodyCollider.bounds.center;
        bodyCenter.z = 0;
        Vector3 bodySize = bodyCollider.bounds.size;
        bodySize.z = 0;

        // Padding dans le corps
        Vector3 yOffSet = Vector3.up * 0.15f;
        Vector3 xOffSet = Vector3.right * 0.1f;

        // Get top left, top right, bottom left and bottom right of the body
        Vector3 topLeft = bodyCenter + new Vector3(-bodySize.x / 2, bodySize.y / 2, 0) - yOffSet + xOffSet;
        Vector3 topRight = bodyCenter + new Vector3(bodySize.x / 2, bodySize.y / 2, 0) - yOffSet - xOffSet;
        Vector3 bottomLeft = bodyCenter + new Vector3(-bodySize.x / 2, -bodySize.y / 2, 0) + yOffSet + xOffSet;
        Vector3 bottomRight = bodyCenter + new Vector3(bodySize.x / 2, -bodySize.y / 2, 0) + yOffSet - xOffSet;

        // You need to keep the z to keep the draw order of the slots
        // Set the position of the slots
        FrontLeftLegSlot.transform.position = topLeft + new Vector3(0, 0, FrontLeftLegSlot.transform.position.z);
        FrontRightLegSlot.transform.position = topRight + new Vector3(0, 0, FrontRightLegSlot.transform.position.z);
        BackLeftLegSlot.transform.position = bottomLeft + new Vector3(0, 0, BackLeftLegSlot.transform.position.z);
        BackRightLegSlot.transform.position = bottomRight + new Vector3(0, 0, BackRightLegSlot.transform.position.z);

        headSlot.transform.position = bodyCenter + new Vector3(0, bodySize.y / 2, 0) + new Vector3(0, 0, headSlot.transform.position.z);
    }

    private void OnDestroy()
    {
        if (InteractionManager2.Instance != null)
        {
            InteractionManager2.Instance.m_Saddles.Remove(this);
        }
    }

    public void AttachBodyPart(AnimalType type)
    {
        // If all slots are equipped, change a random one
        if(NbSlotsEquipped == slots.Count)
        {
            int partToChangeIndex = Random.Range(0, slots.Count);
            slots[partToChangeIndex].SetBodyPart(m_AnimalDatas, type);
        }
        else
        {
            // Fill body part slots
            foreach (BodyPartSlot slot in slots)
            {
                if (!slot.HasBodyPart())
                {
                    slot.SetBodyPart(m_AnimalDatas, type);
                    NbSlotsEquipped++;
                    break;
                }
            }
        }

        if (bodySlot.HasBodyPart() && bodySlot.GetBodyPart().TryGetComponent(out BodyScript m_BodyScript))
        {
            m_Saddle.connectedBody = m_BodyScript.GetComponent<Rigidbody2D>();
            Debug.Assert(m_BodyScript != null, "No BodyScript found in children of MontureController");
            OrganiseSlots(m_BodyScript); // Set up distance between slots

            foreach (BodyPartSlot slot in slots)
            {
                if (slot.HasBodyPart())
                {
                    slot.AttachToBody(m_BodyScript);
                }
            }
        }
    }
    
    public void TriggerLegMovement(bool isTeamFirstPlayer, bool isHold, bool is4PlayerBehavior)
    { 
        foreach (BodyPartSlot bodyPartSlot in slots)
        {
            switch (bodyPartSlot.BodyPartType)
            {
                case BodyPartType.FrontLeftLeg:
                case BodyPartType.BackLeftLeg:
                {
                    if (isTeamFirstPlayer 
                        && (!GameManager.Instance.IsTwoPlayerMod || is4PlayerBehavior))
                    {
                        continue;
                    }

                    if (isHold)
                    {
                        bodyPartSlot.m_InstantiatedPart.GetComponent<LegController>().StartLegTravel(stepUp: true);
                    }
                    else
                    {
                        bodyPartSlot.m_InstantiatedPart.GetComponent<LegController>().StartLegTravel(stepUp: false);
                    }
                }
                    break;
                case BodyPartType.FrontRightLeg:
                case BodyPartType.BackRightLeg:
                {
                    if (isTeamFirstPlayer && is4PlayerBehavior)
                    {
                        if (isHold)
                        {
                            bodyPartSlot.m_InstantiatedPart.GetComponent<LegController>().StartLegTravel(stepUp: true);
                        }
                        else
                        {
                            bodyPartSlot.m_InstantiatedPart.GetComponent<LegController>().StartLegTravel(stepUp: false);
                        }
                    }
                }
                    break;
            }
        }
    }

    public bool IsReadyToMount()
    {
        foreach (BodyPartSlot bodyPartSlot in slots)
        {
            if (!bodyPartSlot.HasBodyPart())
            {
                return false;
            }
        }

        return true;
    }
}
