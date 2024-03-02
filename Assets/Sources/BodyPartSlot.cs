using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using game;
using UnityEngine;

public class BodyPartSlot : MonoBehaviour
{
    public BodyPartType BodyPartType = BodyPartType.Head;

    private GameObject gameObject = null;

    public bool IsSet()
    {
        return gameObject != null;
    }

    public void SetBodyPart(AnimalType type)
    {
        AnimalDataInfo info = GameManager.Instance.GetAnimalDatas().GetAnimalInfoByType(type);

         if(gameObject != null)
         {
            Destroy(gameObject);
         }


         gameObject = Instantiate(info.GetBodyPartTemplate(BodyPartType), transform);
         gameObject.transform.localPosition = new UnityEngine.Vector3(0,0,0);
    }
}
