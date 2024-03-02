using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyInformations : Singleton<CurrencyInformations>
{
    [SerializeField]
    private TextMeshProUGUI _currentCurrencyText;

    public void SetCurrencyValue(float value)
    {
        _currentCurrencyText.text = value.ToString();
    }
}
