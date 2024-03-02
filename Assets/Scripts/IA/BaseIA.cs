using System;
using game;
using UnityEngine;
using Random = UnityEngine.Random;

public class BaseIA : MonoBehaviour
{
    public AnimalType AnimalType;
    public float WanderAngleModifier = 8.0f;
    public Transform WanderTarget;
    
    private float _nextAngle = 0;
    private float _speed;

    private void Start()
    {
        AnimalDataInfo animalDataInfo = GameManager.Instance.GetAnimalDatas().GetAnimalInfoByType(AnimalType);
        _speed = Random.Range(animalDataInfo.MinSpeed, animalDataInfo.MaxSpeed);
    }

    private void Update()
    {
        Wander();
    }

    private void Wander()
    {
        _nextAngle += Random.Range(-WanderAngleModifier, WanderAngleModifier);
        if (Mathf.Abs(_nextAngle) > 90)
        {
            _nextAngle /= 10;
        }

        transform.RotateAround(transform.position, Vector3.up, _nextAngle * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, WanderTarget.position, Time.deltaTime * _speed);
    }
}
