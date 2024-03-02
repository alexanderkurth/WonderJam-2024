using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemVisual : MonoBehaviour
{
    [SerializeField]
    private Image _mObjectImage;

    public void SetImageSprite(Sprite newSprite)
    {
        _mObjectImage.sprite = newSprite;
    }
}
