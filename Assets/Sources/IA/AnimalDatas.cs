using System;
using System.Collections.Generic;
using UnityEngine;

public enum AnimalType
{
    Horse, 
    Cow, 
    Sheep,
}

public enum BodyPartType
{
    Head,
    LeftLeg,
    RightLeg,
    Body
}

[Serializable]
public class BodyPartGO
{
    public BodyPartType Type = BodyPartType.Head;
    public GameObject Template = null;
}

[Serializable]
public class AnimalDataInfo 
{
    public AnimalType AnimalType;
    public BaseIA Visual;
    public float MinSpeed;
    public float MaxSpeed;
    public Vector2 IdleTimeRandomRange;
    public Vector2 TimeBetweenIdle;
    public List<BodyPartGO> BodyPartsInfo;

    public GameObject GetBodyPartTemplate(BodyPartType type)
    {
        BodyPartGO part = BodyPartsInfo.Find(element => element.Type == type);
        if(part != null)
        {
            return part.Template;
        }
        return null;
    }
}

[CreateAssetMenu(fileName = "AnimalDatas", menuName = "ScriptableObjects/AnimalDatas", order = 1)]
public class AnimalDatas: ScriptableObject
{
    public List<AnimalDataInfo> AnimalDataInfos = new List<AnimalDataInfo>();

    public AnimalDataInfo GetAnimalInfoByType(AnimalType animalType)
    {
        return AnimalDataInfos.Find((animal) => animal.AnimalType == animalType);
    }
}