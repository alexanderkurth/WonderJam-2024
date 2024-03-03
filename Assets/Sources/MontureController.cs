using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class MontureController : MonoBehaviour
{ 
    public List<BodyPartSlot> slots;
    private int NbSlotsEquipped = 0;

    void Start()
    {
        InteractionManager2.Instance.AddSaddle(this);

        NbSlotsEquipped = 0;
    }

    private void OnDestroy()
    {
        if(InteractionManager2.Instance != null)
        {
             InteractionManager2.Instance.m_Saddles.Remove(this);
        }
    }

    #if UNITY_EDITOR
    [ContextMenu("TestBodyPart")]
    void TestAttachBodyPart()
    {
        AttachBodyPart(AnimalType.Horse);
    }
    #endif

    public void AttachBodyPart(AnimalType type)
    {
        if(NbSlotsEquipped == slots.Count)
        {
            int partToChangeIndex = Random.Range(0, slots.Count);
            slots[partToChangeIndex].SetBodyPart(type);
        }
        
        foreach(BodyPartSlot slot in slots)
        {
            if(!slot.IsSet())
            {
                slot.SetBodyPart(type);
                NbSlotsEquipped++;
                break; 
            }
        }
    }
}
