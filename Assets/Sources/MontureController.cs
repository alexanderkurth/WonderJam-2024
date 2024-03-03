using game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MontureController : MonoBehaviour
{ 
    public List<BodyPartSlot> slots;
    private int NbSlotsEquipped = 0;

    BodyScript m_BodyScript;
    private AnimalDatas m_AnimalDatas;

    public TeamID TeamID;

    #if UNITY_EDITOR
    [ContextMenu("TestBodyPart")]
    void TestAttachBodyPart()
    {
        AttachBodyPart(AnimalType.Horse);
    }
#endif

    private void Start()
    {
        InteractionManager2.Instance.AddSaddle(this);

        NbSlotsEquipped = 0;
        Debug.Assert(slots.Count > 0, "No slots found in MontureController");
        m_AnimalDatas = GameManager.Instance?.GetAnimalDatas();
        Debug.Assert(m_AnimalDatas != null, "No AnimalDatas - no GameManager");
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

        //m_BodyScript = GetComponentInChildren<BodyScript>();
        //Debug.Assert(m_BodyScript != null, "No BodyScript found in children of MontureController");

        //foreach (BodyPartSlot slot in slots)
        //{
        //    slot.AttachToBody(m_BodyScript);
        //}
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
                    if (isTeamFirstPlayer)
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
                case BodyPartType.FrontRightLeg:
                case BodyPartType.BackRightLeg:
                {
                    if (isTeamFirstPlayer && (!GameManager.Instance.IsTwoPlayerMod || !is4PlayerBehavior))
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
            }
        }
    }
}
