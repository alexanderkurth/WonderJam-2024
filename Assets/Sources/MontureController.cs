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
    [SerializeField] SpriteRenderer _saddleSprite;

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

    [ContextMenu("FillAll")]
    void FillAll()
    {
        while (NbSlotsEquipped < slots.Count)
        {
            AttachBodyPart(animalType);
        }
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

        slots.Clear();
        // Fill slots list
        slots.Add(headSlot);
        slots.Add(bodySlot);
        slots.Add(FrontLeftLegSlot);
        slots.Add(FrontRightLegSlot);
        slots.Add(BackLeftLegSlot);
        slots.Add(BackRightLegSlot);

        if(_saddleSprite != null)
        {
            _saddleSprite.color = GameManager.Instance.GetAnimalDatas().GetColorForTeam(TeamID);
        }
    }

    void OrganiseSlots(BodyScript body)
    {
        var bodyCollider = body.GetComponent<BoxCollider2D>();
        Vector3 bodySize = bodyCollider.size;
        bodySize.z = 0;
        Debug.Log("Body size: " + bodySize);

        // Padding dans le corps
        Vector3 yOffSet = Vector3.up * 0.15f;
        Vector3 xOffSet = Vector3.right * 0.1f;

        // Get top left, top right, bottom left and bottom right of the body
        Vector3 topLeft = new Vector3(-bodySize.x / 2, bodySize.y / 2, 0) - yOffSet + xOffSet;
        Vector3 topRight = new Vector3(bodySize.x / 2, bodySize.y / 2, 0) - yOffSet - xOffSet;
        Vector3 bottomLeft = new Vector3(-bodySize.x / 2, -bodySize.y / 2, 0) + yOffSet + xOffSet;
        Vector3 bottomRight = new Vector3(bodySize.x / 2, -bodySize.y / 2, 0) + yOffSet - xOffSet;
        Vector3 middleTop = new Vector3(0, bodySize.y / 2, 0);

        // You need to keep the z to keep the draw order of the slots
        topLeft += new Vector3(0, 0, FrontLeftLegSlot.transform.position.z);
        topRight += new Vector3(0, 0, FrontRightLegSlot.transform.position.z);
        bottomLeft += new Vector3(0, 0, BackLeftLegSlot.transform.position.z);
        bottomRight += new Vector3(0, 0, BackRightLegSlot.transform.position.z);
        middleTop += new Vector3(0, 0, headSlot.transform.position.z);

        // Get the global position of the slots with rotation
        topLeft = body.transform.TransformPoint(topLeft);
        topRight = body.transform.TransformPoint(topRight);
        bottomLeft = body.transform.TransformPoint(bottomLeft);
        bottomRight = body.transform.TransformPoint(bottomRight);
        middleTop = body.transform.TransformPoint(middleTop);
        Vector3 center = body.transform.TransformPoint(Vector3.zero);

        FrontLeftLegSlot.transform.position = topLeft;
        FrontRightLegSlot.transform.position = topRight;
        BackLeftLegSlot.transform.position = bottomLeft;
        BackRightLegSlot.transform.position = bottomRight;
        headSlot.transform.position = middleTop;
        m_BodyScript.transform.position = center;

        m_Saddle.connectedBody = body.GetComponent<Rigidbody2D>();
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
        BodyPartSlot newlyAttached = null;
        if (NbSlotsEquipped == slots.Count)
        {
            BodyPartSlot slot = slots[Random.Range(0, slots.Count)];
            slot.SetBodyPart(m_AnimalDatas, type);
            newlyAttached = slot;
            if (slot == bodySlot)
            {
                m_BodyScript = slot.GetBodyPart().GetComponent<BodyScript>();
                OrganiseSlots(m_BodyScript); // Set up distance between slots
            }
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

                    if (slot == bodySlot)
                    {
                        m_BodyScript = slot.GetBodyPart().GetComponent<BodyScript>();
                        OrganiseSlots(m_BodyScript); // Set up distance between slots
                    }
                    newlyAttached = slot;

                    break;
                }
            }
        }

        // Force reattach all slots to the body to calibrate joints, provided we have a body
        if (m_BodyScript != null)
        {
            if (newlyAttached && newlyAttached.HasBodyPart() && newlyAttached != bodySlot)
            {
                newlyAttached.AttachToBody(m_BodyScript);
                newlyAttached.GetBodyPart().GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
            if (headSlot.HasBodyPart())
            {
                headSlot.transform.rotation = m_BodyScript.transform.rotation;
                headSlot.AttachToBody(m_BodyScript);
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

    public void OnDrawGizmos()
    {
        // Editor only (not in game) - fill a local list of slots
        if (slots.Count == 0 && Application.isEditor)
        {
            slots.Add(headSlot);
            slots.Add(bodySlot);
            slots.Add(FrontLeftLegSlot);
            slots.Add(FrontRightLegSlot);
            slots.Add(BackLeftLegSlot);
            slots.Add(BackRightLegSlot);
        }

        foreach (var item in slots)
        {
            Vector3 pos = item.transform.localPosition;
            if (m_BodyScript != null)
            {
                Gizmos.color = Color.green;
                pos = m_BodyScript.transform.TransformPoint(pos);
            }
            else
            {
                Gizmos.color = Color.red;
                pos = transform.TransformPoint(pos);
            }
            Gizmos.DrawSphere(pos, 0.1f);
        }

    }
}
