using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ObjectsDefinition", order = 1)]
public class ObjectsDefinition : ScriptableObject
{
    [SerializeField]
    private List<ObjectData> ObjectsDatas = new List<ObjectData>();    

    public List<ObjectData> GetObjectsFromType(ObjectType objectType)
    {
        return ObjectsDatas.FindAll(x => x.ObjectType == objectType);
    }
}
