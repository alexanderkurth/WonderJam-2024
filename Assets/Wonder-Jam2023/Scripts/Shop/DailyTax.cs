using TMPro;
using UnityEngine;

public class DailyTax : Singleton<DailyTax>
{
    private const float DAILY_TAX = 20f;
    private const float TAX_MULTIPLIER = 0.20f;
    private float _currentTax = 0f;

    [SerializeField] 
    private TextMeshProUGUI _currentTaxText;
    [SerializeField]
    private AudioSource _mAudioTaxPay = null;
    
    private float GetDailyTax()
    {
        _currentTax = -Mathf.Round(DAILY_TAX + (DAILY_TAX * TAX_MULTIPLIER * GameMode.Instance.dayCount));
        return _currentTax;
    }

    public void DeductTax()
    {
        float currentTax = GetDailyTax();
        Inventory.Instance.ChangeCurrencyValue(currentTax);
        _mAudioTaxPay.Play();
    }

    public void DisplayTax()
    {
        _currentTaxText.text = GetDailyTax().ToString();
    }
}
