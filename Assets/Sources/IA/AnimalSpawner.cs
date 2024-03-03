using game;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    [SerializeField] private AnimalType _animalType;
    [SerializeField] private float _timeBetweenSpawn = 5f;
    [SerializeField] private int _maxAnimalSpawned = 2;

    private float _timeSinceLastSpawn = 0f;
    private AnimalDataInfo _animalDataInfo = null; 
    private int _animalSpawned = 0;
    
    // Start is called before the first frame update
    void Awake()
    {
        //enabled = false; 
    }

    private void Start()
    {
        _animalDataInfo = GameManager.Instance.GetAnimalDatas().GetAnimalInfoByType(_animalType);
        TrySpawnNewAnimal();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeSinceLevelLoad > _timeSinceLastSpawn + _timeBetweenSpawn && _animalSpawned < _maxAnimalSpawned)
        {
            TrySpawnNewAnimal(); 
        }
    }

    private void TrySpawnNewAnimal()
    {
        Collider[] colliders = new Collider[1]; 
        if (Physics.OverlapSphereNonAlloc(transform.position, 2f, colliders) <= 0)
        {
            BaseIA baseIA = Instantiate(_animalDataInfo.Visual, transform.position, Quaternion.identity);
            baseIA.Initialize(this);
            _timeSinceLastSpawn = Time.timeSinceLevelLoad;
            _animalSpawned++;
        }
    }

    public void OnSpawnAnimalRemove()
    {
        _animalSpawned--; 
    }

    #if UNITY_EDITOR
    [ContextMenu("Randomize")]
    void Randomize()
    {
        _timeBetweenSpawn = Random.Range(10, 20);
    }
    #endif
}
