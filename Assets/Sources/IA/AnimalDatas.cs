using System;
using System.Collections.Generic;
using UnityEngine;

public enum AnimalType
{
    Horse, 
    Cow, 
    Sheep,
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