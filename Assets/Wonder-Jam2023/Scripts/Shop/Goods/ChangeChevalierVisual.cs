using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChangeChevalierVisual : MonoBehaviour
{
    [Header("Sprite Renderer")]
    [SerializeField]
    private SpriteRenderer _torsoSprite;
    [SerializeField]
    private SpriteRenderer _headSprite;
    [SerializeField]
    private SpriteRenderer _armSprite;
    [SerializeField]
    private SpriteRenderer _legRightSprite;
    [SerializeField]
    private SpriteRenderer _legLeftSprite;

    [Header("Instruments sprites")]

    [SerializeField]
    private SpriteRenderer _violineSprite;

    [SerializeField]
    private SpriteRenderer _fluteSprite;

    [SerializeField]
    private SpriteRenderer _trumpetSprite;

    [SerializeField]
    private SpriteRenderer _cornemuseSprite;

    [Header("Images")]
    [SerializeField]
    private Image _torsoUImage;
    [SerializeField]
    private Image _headUImage;
    [SerializeField]
    private Image _armUImage;
    [SerializeField]
    private Image _legUImage;

    [Header("Color")]
    [SerializeField]
    Color _armoredModularColor;

    private void Start()
    {
        if (Inventory.itemAddedToInventory == null)
        {
            Inventory.itemAddedToInventory = new UnityEvent<ObjectData>();
        }

        Inventory.itemAddedToInventory.AddListener(ChangeSprite);

        foreach (ObjectData item in Inventory.Instance.GetCurrentObjectListForType(ObjectType.BlingArmorPart))
        {
            ChangeSprite(item);
        }
        
        foreach (ObjectData item in Inventory.Instance.GetCurrentObjectListForType(ObjectType.MusicalInstrument))
        {
            ChangeSprite(item);
        }
    }

    private void ChangeSprite(ObjectData item)
    {
        switch (item.ObjectName)
        {
            case AvailableObject.ArmorArm:
                _armSprite.sprite = item.ObjectSprite;
                _armSprite.color = _armoredModularColor;
                if (_armUImage)
                {
                    _armUImage.color = _armoredModularColor;
                    _armUImage.sprite = item.ObjectSpriteShop;
                }
                break;

            case AvailableObject.ArmorLegs:
                _legRightSprite.sprite = item.ObjectSprite;
                _legLeftSprite.color = _armoredModularColor;
                if (_legUImage)
                {
                    _legUImage.color = _armoredModularColor;
                    _legUImage.sprite = item.ObjectSpriteShop;
                }
                break;

            case AvailableObject.ArmorChest:
                _torsoSprite.sprite = item.ObjectSprite;
                _torsoSprite.color = _armoredModularColor;
                if (_torsoUImage)
                {
                    _torsoUImage.color = _armoredModularColor;
                    _torsoUImage.sprite = item.ObjectSpriteShop;
                }
                break;

            case AvailableObject.ArmorHead:
                _headSprite.sprite = item.ObjectSprite;
                _headSprite.color = _armoredModularColor;
                if (_headUImage)
                {
                    _headUImage.color = _armoredModularColor;
                    _headUImage.sprite = item.ObjectSpriteShop;
                }
                break;

            case AvailableObject.Flute:
                _fluteSprite.enabled = true;
                break;
            case AvailableObject.Violin:
                _violineSprite.enabled = true;
                break;
            case AvailableObject.Trumpet:
                _trumpetSprite.enabled = true;
                break;
            case AvailableObject.Cornemuse:
                _cornemuseSprite.enabled = true;
                break;
            case AvailableObject.Wine:
                break;
            case AvailableObject.Cheese:
                break;
            case AvailableObject.Wood:
                break;
            case AvailableObject.ButterKnife:
                break;
            case AvailableObject.OldSocks:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
