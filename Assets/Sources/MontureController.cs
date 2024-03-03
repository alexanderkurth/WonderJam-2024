using System.Collections;
using System.Collections.Generic;
using game;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class MontureController : MonoBehaviour
{ 
    public List<BodyPartSlot> slots;
    private int NbSlotsEquipped = 0;

    public TeamID TeamID = TeamID.Team1;

    // Start is called before the first frame update

    void Start()
    {
        NbSlotsEquipped = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
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
