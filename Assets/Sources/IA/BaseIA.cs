using game;
using UnityEngine;
using Random = UnityEngine.Random;

public class BaseIA : MonoBehaviour
{
    [SerializeField] private AnimalType _animalType;
    [SerializeField] private float _wanderAngleModifier = 8.0f;
    [SerializeField] private Transform _wanderTarget;
    
    private bool _isGrab = false;
    private bool _isMerge = false;
    private float _nextAngle = 0;
    private float _speed;
    private AnimalSpawner _animalSpawner = null;

    public void Initialize(AnimalSpawner animalSpawner)
    {
        _animalSpawner = animalSpawner; 
    }
    
    private void Start()
    {
        AnimalDataInfo animalDataInfo = GameManager.Instance.GetAnimalDatas().GetAnimalInfoByType(_animalType);
        _speed = Random.Range(animalDataInfo.MinSpeed, animalDataInfo.MaxSpeed);
    }

    private void Update()
    {
        if (_isGrab || _isMerge)
        {
            return; 
        }
        
        Wander();
    }

    private void Wander()
    {
        _nextAngle += Random.Range(-_wanderAngleModifier, _wanderAngleModifier);
        if (Mathf.Abs(_nextAngle) > 90)
        {
            _nextAngle /= 10;
        }

        transform.RotateAround(transform.position, Vector3.up, _nextAngle * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, _wanderTarget.position, Time.deltaTime * _speed);
    }

    public void OnGrab()
    {
        _isGrab = true; 
    }

    public void OnMerge()
    {
        _isGrab = false;
        _isMerge = true; 
        _animalSpawner.OnSpawnAnimalRemove(); 
        //TODO : Apply Effect on animal + Destroy gameobject 
    }
}
