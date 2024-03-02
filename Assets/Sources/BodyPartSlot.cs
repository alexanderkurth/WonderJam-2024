using System;
using game;
using UnityEngine;

public class BodyPartSlot : MonoBehaviour
{
    public BodyPartType BodyPartType = BodyPartType.Head;

    private GameObject _instantiatedPart = null;

    public bool IsSet()
    {
        return _instantiatedPart != null;
    }

    public void SetBodyPart(AnimalType type)
    {
        AnimalDataInfo info = GameManager.Instance.GetAnimalDatas().GetAnimalInfoByType(type);

         if(_instantiatedPart != null)
         {
            Destroy(_instantiatedPart);
         }


         _instantiatedPart = Instantiate(info.GetBodyPartTemplate(BodyPartType), transform);
         _instantiatedPart.transform.localPosition = Vector3.zero;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f); 
    }
}
