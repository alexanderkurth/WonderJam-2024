using System;
using System.Collections.Generic;
using game;
using UnityEngine;

public enum AnimalType
{
    Horse, 
    Frog, 
    Pig,
}

public enum BodyPartType
{
    Head,
    FrontLeftLeg,
    FrontRightLeg,
    BackLeftLeg,
    BackRightLeg,
    Body
}

[Serializable]
public class BodyPartGO
{
    public BodyPartType Type = BodyPartType.Head;
    public GameObject Template = null;
}

[Serializable]
public class TeamColors
{
    public TeamID TeamID = TeamID.Team1;
    public Color Color = Color.red;
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

    public List<TeamColors> TeamColors;

    public AnimalDataInfo GetAnimalInfoByType(AnimalType animalType)
    {
        return AnimalDataInfos.Find((animal) => animal.AnimalType == animalType);
    }

    public Color GetColorForTeam(TeamID id)
    {
        TeamColors tc = TeamColors.Find(element => element.TeamID == id);
        if(tc != null)
        {
            return tc.Color;
        }

        return Color.white;
    }
}