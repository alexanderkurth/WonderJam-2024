using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSceneManager : Singleton<ShopSceneManager>
{
    [SerializeField]
    private Transform _mKnightTransform = null;

    [SerializeField]
    private Transform _mKnightStartPivot = null;

    [SerializeField]
    private Transform _mShopPositionPivot = null;

    [SerializeField]
    private bool _mPlayOnStart = false;
    [SerializeField]
    private float _mMoveToShopTime = 2f;
    [SerializeField]
    private Vector2 _mNumberOfObjectToLookAt = new Vector2(2, 5);
    [SerializeField]
    private Vector2 _mHesitationTimePerObject = new Vector2(0.25f, 0.5f);
    [SerializeField]
    private float _mSpendingMoveToStartTime = 4f;
    [SerializeField]
    private float _mNoSpendingMoveToStartTime = 4f;

    private void Start()
    {
        HUDCanvas.Instance.ToggleShopPreview(false);
        if (_mPlayOnStart)
        {
            Vector3 rotation = _mKnightTransform.rotation.eulerAngles;
            rotation.y = 180f;
            _mKnightTransform.rotation = Quaternion.Euler(rotation);
            StartCoroutine(ShoppingLoop());
        }
    }

    public void StartShoppingLoop()
    {
        Vector3 rotation = _mKnightTransform.rotation.eulerAngles;
        rotation.y = 180f;
        _mKnightTransform.rotation = Quaternion.Euler(rotation);
        StartCoroutine(ShoppingLoop());
    }

    private IEnumerator ShoppingLoop()
    {
        yield return MoveKnightToPointCoroutine(_mShopPositionPivot.position, _mMoveToShopTime);

        List<BuyableItem> buyableObjects = Shop.Instance.GetBuyableObjects();

        yield return StartCoroutine(HesitationFlow(buyableObjects));

        yield return new WaitForSeconds(2f);

        GameMode.Instance.StartNewDay();

        yield break;
    }

    private IEnumerator MoveKnightToPointCoroutine(Vector3 point, float duration)
    {
        float lerp = 0f;
        Vector3 initialPosition = _mKnightTransform.position;

        while (lerp<1f)
        {
            lerp += Time.deltaTime / duration;

            Vector3 newPosition = Vector3.Lerp(initialPosition, point, lerp);
            _mKnightTransform.position = newPosition;

            yield return null;
        }

        yield break;
    }

    private IEnumerator HesitationFlow(List<BuyableItem> buyableObjects)
    {
        int objectCount = buyableObjects.Count;
        bool objectIsChosen = objectCount > 0 ? false : true;
        Vector3 rotation = Vector3.zero;
        BuyableItem selectedItem = GetSelectedItem(buyableObjects);

        yield return new WaitForSeconds(Random.Range(_mHesitationTimePerObject.x, _mHesitationTimePerObject.y));

        if (objectCount > 0)
        {
            HUDCanvas.Instance.ToggleShopPreview(true);

            int index = 0, numberOfTry = 0;
            List<int> indexes = GenerateIndexToLookAt(objectCount, buyableObjects.IndexOf(selectedItem));

            for (int i = 0; i < indexes.Count; i++)
            {
                BuyableItem lookedAtItem = buyableObjects[indexes[i]];
                HUDCanvas.Instance.ShopItemVisual.SetImageSprite(lookedAtItem.ObjectData.ObjectSpriteShop);

                Vector3 direction = (lookedAtItem.transform.position - _mKnightTransform.position).normalized;
                _mKnightTransform.rotation = Quaternion.LookRotation(direction);

                //C'est dégueux mais on corrige le X pour rester flat.
                rotation = _mKnightTransform.rotation.eulerAngles;
                rotation.x = 0f;
                _mKnightTransform.rotation = Quaternion.Euler(rotation);

                yield return new WaitForSeconds(Random.Range(_mHesitationTimePerObject.x, _mHesitationTimePerObject.y));

                index = (index + 1) % objectCount;
                numberOfTry++;
            }
        }


        rotation = _mKnightTransform.rotation.eulerAngles;
        rotation.y = 0f;
        _mKnightTransform.rotation = Quaternion.Euler(rotation);
        HUDCanvas.Instance.ToggleShopPreview(false);

        if (selectedItem != null)
        {
            selectedItem.BuyItem();
            yield return StartCoroutine(MoveKnightToPointCoroutine(_mKnightStartPivot.position, _mSpendingMoveToStartTime));
        }
        else
        {
            yield return StartCoroutine(MoveKnightToPointCoroutine(_mKnightStartPivot.position, _mNoSpendingMoveToStartTime));
        }

        yield break;
    }

    private List<int> GenerateIndexToLookAt(int objectCount, int selectedItemIndex)
    {
        List<int> toReturn = new List<int>();
        int numberOfObjectToLookAt = objectCount > 1 ? Mathf.RoundToInt(Random.Range(_mNumberOfObjectToLookAt.x, _mNumberOfObjectToLookAt.y)) : 1;
        int selectIndex = Random.Range(0, objectCount);

        for (int i = 0; i < numberOfObjectToLookAt; i++)
        {
            toReturn.Add(selectIndex);

            int valueToAdd = Random.Range(0, 2) > 0 ? 1 : -1;
            selectIndex = (selectIndex + valueToAdd) < 0 ? (selectIndex + 1) : (selectIndex + valueToAdd) % objectCount;
        }

        if(toReturn[toReturn.Count-1] != selectedItemIndex)
        {
            toReturn.Add(selectedItemIndex);
        }

        return toReturn;
    }

    private BuyableItem GetSelectedItem(List<BuyableItem> items)
    {
        int totalPrice = 0;

        foreach (BuyableItem item in items)
        {
            totalPrice += item.ObjectPrice;
        }

        List<float> percents = new List<float>();
        foreach (BuyableItem item in items)
        {
            float percent = percents.Count > 0 ? percents[percents.Count-1] + (item.ObjectPrice / (float)totalPrice) : (item.ObjectPrice / (float)totalPrice);
            percents.Add(percent);
        }

        float chosenPercent = Random.Range(0f, 1f);

        for (int i = 0; i < percents.Count; i++)
        {
            if(percents[i] > chosenPercent || i+1 == percents.Count)
            {
                return items[i];
            }
        }

        return null;
    }
}
