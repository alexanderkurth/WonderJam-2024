using game;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using System.Diagnostics;

public class BaseIA : MonoBehaviour
{
    [SerializeField] private AnimalType _animalType;
    [SerializeField] private float _obstacleDistance = 2f;
    [SerializeField] private float _wanderAngleModifier = 8.0f;
    [SerializeField] private Transform _wanderTarget;

    private bool _isIdle = false;
    private bool _isGrab = false;
    private bool _isMerge = false;
    private float _idleTime = 0f;
    private float _timeSinceLastIdle = 0f;
    private float _nextAngle = 0;
    private float _speed;
    private Vector2 _idleRangeTime;
    private Vector2 _timeBetweenIdle;
    private AnimalSpawner _animalSpawner = null;
    private AnimalDataInfo _animalDataInfo = null; 

    public void Initialize(AnimalSpawner animalSpawner)
    {
        _animalSpawner = animalSpawner;
        transform.rotation = animalSpawner.transform.rotation;
        transform.position = animalSpawner.transform.position;
    }
    
    private void Start()
    {
        _animalDataInfo = GameManager.Instance.GetAnimalDatas().GetAnimalInfoByType(_animalType);
        _speed = Random.Range(_animalDataInfo.MinSpeed, _animalDataInfo.MaxSpeed);
        _idleRangeTime = _animalDataInfo.IdleTimeRandomRange;
        _timeBetweenIdle = _animalDataInfo.TimeBetweenIdle; 
        StartCoroutine(TryIdle());
    }

    private void Update()
    {
        if (_isGrab || _isMerge)
        {
            return; 
        }

        if (_isIdle)
        {
            if (_timeSinceLastIdle + _idleTime < Time.timeSinceLevelLoad)
            {
                ChoseNewDirection();
                _isIdle = false;
                StartCoroutine(TryIdle());
            }

            return;
        }

        Wander();
    }

    private void ChoseNewDirection()
    {     
        _nextAngle += Random.Range(90, 270);
        
        transform.RotateAround(transform.position, Vector3.forward, _nextAngle);
    }

    private void Wander()
    {
        _nextAngle += Random.Range(-_wanderAngleModifier, _wanderAngleModifier);
        if (Mathf.Abs(_nextAngle) > 90)
        {
            _nextAngle /= 10;
        }

        transform.RotateAround(transform.position, Vector3.forward, _nextAngle * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, _wanderTarget.position, Time.deltaTime * _speed);

        TryGetForwardCollision();
    }
    

    private IEnumerator TryIdle()
    {
        yield return new WaitForSeconds(Random.Range(_timeBetweenIdle.x, _timeBetweenIdle.y)); 
        Idle();
    }

    public void OnGrab()
    {
        _isGrab = true; 
        //TODO 
    }

    public void OnMerge()
    {
        _isGrab = false;
        _isMerge = true; 
        _animalSpawner.OnSpawnAnimalRemove();
       MontureController.Instance.AttachBodyPart(_animalDataInfo.AnimalType);
    }

    private void TryGetForwardCollision()
    {
        Vector3 dir = _wanderTarget.position - transform.position;
        dir.Normalize();
        if (Physics.Raycast(transform.position, dir, _obstacleDistance))
        {
            Idle();
        }
    }

    private void Idle()
    {
        _isIdle = true;
        _idleTime = Random.Range(_idleRangeTime.x, _idleRangeTime.y);
        _timeSinceLastIdle = Time.timeSinceLevelLoad;
        StopAllCoroutines();
    }
}
