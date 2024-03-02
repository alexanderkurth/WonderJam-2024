using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : Singleton<Inventory>
{
    [SerializeField]
    private float _baseValue;

    private float _currentCurrency;
    private Dictionary<ObjectType, List<ObjectData>> _inventory = new Dictionary<ObjectType, List<ObjectData>>();
    private const int MAX_ARMOR_COUNT = 4;

    public static UnityEvent<ObjectData> itemAddedToInventory;

    protected override void Awake()
    {
        base.Awake();

        // Init currenCurrency
        _currentCurrency = _baseValue;
    }

    private void Start()
    {
        ChangeCurrencyValue(0);
    }

    public void ChangeCurrencyValue(float value)
    {
        _currentCurrency += value;
        CurrencyInformations.Instance.SetCurrencyValue(_currentCurrency);

        if (_currentCurrency < 0)
        {
            GameMode.Instance.GameOver(GameMode.GameOverCondition.NotEnoughMoney);
        }
    }

    public float GetCurrentCurrency()
    {
        return _currentCurrency;
    }

    public void AddToInventory(ObjectData objectData)
    {
        if (_inventory.ContainsKey(objectData.ObjectType))
        {
            _inventory[objectData.ObjectType].Add(objectData);
        }
        else
        {
            List<ObjectData> availableObjects = new List<ObjectData> { objectData };
            _inventory.Add(objectData.ObjectType, availableObjects);
        }

        itemAddedToInventory?.Invoke(objectData);

        if (IsFullArmor())
        {
            GameMode.Instance.WinGame();
        }
    }

    public bool IsInInventory(ObjectData objectData)
    {
        if (_inventory.ContainsKey(objectData.ObjectType))
        {
            return _inventory[objectData.ObjectType].Contains(objectData);
        }

        return false;
    }

    public List<ObjectData> GetCurrentObjectListForType(ObjectType objectType)
    {
        if (_inventory.ContainsKey(objectType))
        {
            return _inventory[objectType];
        }

        return new List<ObjectData>();
    }

    public bool IsFullArmor()
    {
        ObjectType objectType = ObjectType.BlingArmorPart;
        if (_inventory.ContainsKey(objectType))
        {
            return _inventory[objectType].Count == MAX_ARMOR_COUNT;
        }

        return false;
    }

    public AvailableObject GetCurrentInstruments()
    {
        ObjectType objectType = ObjectType.MusicalInstrument;
        if (_inventory.ContainsKey(objectType))
        {
            return _inventory[objectType][Random.Range(0, _inventory[objectType].Count)].ObjectName;
        }
        return AvailableObject.None;
    }
}
