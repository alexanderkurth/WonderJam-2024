using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : Singleton<Shop>
{
    [System.Serializable]
    private struct ShopItemSelection
    {        
        public Transform ItemParentRoot;
        public ObjectType ItemObjectType;
    }

    [SerializeField]
    private GameObject _mBuyableItemPrefab;

    [SerializeField]
    private List<ShopItemSelection> _mShopItemSelection = new List<ShopItemSelection>();

    [SerializeField]
    private Dictionary<BuyableItem, ShopItemSelection> _mBuyableItems = new Dictionary<BuyableItem, ShopItemSelection>();

    [SerializeField]
    private ObjectsDefinition _mObjectsDefinition = null;

    private void Start()
    {
        CreateShopItems(); 
    }

    private void CreateShopItems()
    {
        foreach (ShopItemSelection item in _mShopItemSelection)
        {
            GameObject newGO = Instantiate(_mBuyableItemPrefab, item.ItemParentRoot);
            BuyableItem buyableItem = newGO.GetComponent<BuyableItem>();

            _mBuyableItems.Add(buyableItem, item);
        }
    }

    public void InitializeShopOfTheDay()
    {
        foreach (KeyValuePair<BuyableItem, ShopItemSelection> item in _mBuyableItems)
        {
            if (SelectBuyableItem(item.Value.ItemObjectType, out ObjectData objectData))
            {
                item.Key.IntializeObject(objectData);
            }
            else
            {
                item.Key.gameObject.SetActive(false);
            }
        }
    }

    public bool SelectBuyableItem(ObjectType objectType, out ObjectData objectDataToReturn)
    {
        List<ObjectData> objectSelection = _mObjectsDefinition.GetObjectsFromType(objectType);
        int selectionCount = objectSelection.Count - 1;

        for (int i = selectionCount; i >= 0; i--)
        {
            ObjectData objectData = objectSelection[i];
            if (objectData.CanBeBoughtMultipleTime == true || !Inventory.Instance.IsInInventory(objectData))
            {
                continue;   
            }

            objectSelection.RemoveAt(i);
        }

        selectionCount = objectSelection.Count;

        if(selectionCount <= 0)
        {
            objectDataToReturn = new ObjectData();
            return false;
        }

        objectDataToReturn = objectSelection[Random.Range(0, selectionCount)];

        return true;
    }

    public List<BuyableItem> GetBuyableObjects()
    {
        List<BuyableItem> buyableItemToReturn = new List<BuyableItem>();

        foreach (KeyValuePair<BuyableItem, ShopItemSelection> item in _mBuyableItems)
        {
            if(item.Key.IsBuyable())
            {
                buyableItemToReturn.Add(item.Key);
            }
        }

        return buyableItemToReturn;
    }
}
