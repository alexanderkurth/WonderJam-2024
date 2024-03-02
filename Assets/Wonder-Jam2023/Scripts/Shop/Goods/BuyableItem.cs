using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public enum AvailableObject
{
    ArmorArm = 0,
    ArmorLegs = 1,
    ArmorChest = 2,
    ArmorHead = 3,
    Flute = 50,
    Violin,
    Trumpet,
    Cornemuse,
    Wine = 100,
    Cheese,
    Wood,
    ButterKnife,
    OldSocks,
    None,
}

public enum ObjectType
{
    BlingArmorPart,
    MusicalInstrument,
    Miscellaneous
}

[Serializable]
public struct ObjectData
{
    public AvailableObject ObjectName;
    public ObjectType ObjectType;
    public List<int> ObjectPrices;
    public Sprite ObjectSprite;
    public Sprite ObjectSpriteShop;
    public bool CanBeBoughtMultipleTime;
}

public class BuyableItem : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _mObjectBuyPS = null;
    [SerializeField]
    private AudioSource _mObjectBuySound = null;
    [SerializeField]
    private TextMeshPro _mTmpPrice = null;

    private int _mObjectPrice = 1500000000;
    public int ObjectPrice { get => _mObjectPrice; }
    private ObjectData _objectData;
    public ObjectData ObjectData { get => _objectData; }

    public void IntializeObject(ObjectData objectData)
    {
        _objectData = objectData;
        _mObjectPrice = SetObjectPrice(objectData.ObjectPrices);
        _mTmpPrice.text = _mObjectPrice.ToString();
        _mTmpPrice.color = IsBuyable() ? new Color(0f,70f,16f) : Color.red;
    
        ToggleVisual(true);
    }

    public bool IsBuyable()
    {
        return Inventory.Instance.GetCurrentCurrency() >= _mObjectPrice;
    }
    
    private int SetObjectPrice(List<int> price)
    {
        int priceCount = price.Count;
        int randomPriceIndex = Random.Range(0, priceCount);

        return price[randomPriceIndex];
    }

    private void ToggleVisual(bool toEnable)
    {
        _mTmpPrice.enabled = toEnable;
    }

    public int GetObjectPrice()
    {
        return _mObjectPrice;
    }

    public virtual void BuyItem()
    {
        Inventory.Instance.ChangeCurrencyValue(-_mObjectPrice);
        Inventory.Instance.AddToInventory(_objectData);
        _mObjectBuyPS.Play(true);
        _mObjectBuySound.Play();
        ToggleVisual(false);
        SendMessageFromType();

        //Et le quick fix de prod dégueux
        transform.parent.gameObject.SetActive(false);
    }

    private void SendMessageFromType()
    {
        switch (_objectData.ObjectType)
        {
            case ObjectType.BlingArmorPart:
                {
                    HUDCanvas.Instance.DisplayMessage(MessageEnum.BuyArmorPiece, _objectData.ObjectName);
                }
                break;
            case ObjectType.MusicalInstrument:
                {
                    HUDCanvas.Instance.DisplayMessage(MessageEnum.BuyAnInstrument, _objectData.ObjectName);
                }
                break;
            case ObjectType.Miscellaneous:
                {
                    HUDCanvas.Instance.DisplayMessage(MessageEnum.BuyRandomPiece, _objectData.ObjectName);
                }
                break;
        }
    }

}
