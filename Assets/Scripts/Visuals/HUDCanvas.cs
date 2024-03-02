using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameMode;

public enum MessageEnum
{
    DelayStartGame,
    RandomSentenceInGame,
    FinishingAnEnemy,
    BuyRandomPiece,
    BuyArmorPiece,
    BuyAnInstrument,
    OutOfScreenWarning,
    OutOfScreenLose,
    MadnessWarning,
    MadnessLose,
    NotEnoughMoneyLose,
    Victory
}

[System.Serializable]
public struct MessageDatas
{
    public MessageEnum MessageName;
    public List<string> Messages;
    public List<AudioClip> AudioClip;
}

public class HUDCanvas : Singleton<HUDCanvas>   
{
    [SerializeField]
    private GameObject _mMessageObject = null;

    [SerializeField]
    private TextMeshProUGUI _mMessageText = null;

    [SerializeField]
    private Vector2 _mMinMaxDisplayTimeValue = new Vector2(2, 5);

    [SerializeField]
    private Vector2 _mMinMaxStringSizeForMaxDisplayTime = new Vector2(20, 200);

    [SerializeField]
    private List<MessageDatas> _mMessageDatas = new List<MessageDatas>();

    [SerializeField]
    private AudioSource _mVocalAudioSource = new AudioSource();

    [SerializeField]
    private ShopItemVisual _mShopPreview = null;

    [SerializeField]
    private TextMeshProUGUI _mDayCount = null;

    public ShopItemVisual ShopItemVisual { get => _mShopPreview; }

    private Coroutine _mMessageCoroutine = null;

    public void ToggleShopPreview(bool toEnable)
    {
        _mShopPreview.gameObject.SetActive(toEnable);
    }

    public bool DisplayMessage(MessageEnum messageToDisplay, float delayBeforeDisplay = 0f)
    {
        if (_mMessageCoroutine == null)
        {
            _mMessageCoroutine = StartCoroutine(DisplayMessageCoroutine(GetMessageForEnum(messageToDisplay, out AudioClip audioClip), delayBeforeDisplay, audioClip));
            
            return true;
        }

        return false;
    }

    public void DisplayMessage(MessageEnum messageEnum, AvailableObject objectName)
    {
        if (_mMessageCoroutine == null)
        {
            _mMessageCoroutine = StartCoroutine(DisplayMessageCoroutine(GetMessageForEnum(messageEnum, objectName, out AudioClip audioClip), 0f, audioClip));
        }
    }

    public void SendVictoryMessage()
    {
        if (_mMessageCoroutine != null)
        {
            StopCoroutine(_mMessageCoroutine);
        }
        _mMessageCoroutine = StartCoroutine(DisplayMessageCoroutine(GetMessageForEnum(MessageEnum.Victory, out AudioClip audioClip), 0f, audioClip));
    }

    public void SendGameOverMessage(GameOverCondition gameOverCondition)
    {
        if (_mMessageCoroutine != null)
        {
            StopCoroutine(_mMessageCoroutine);
        }

        MessageEnum messageEnum = MessageEnum.OutOfScreenLose;
        switch (gameOverCondition)
        {
            case GameOverCondition.OutOfScreen:
                {
                    messageEnum = MessageEnum.OutOfScreenLose;
                }
                break;
            case GameOverCondition.Madness:
                {
                    messageEnum = MessageEnum.MadnessLose;
                }
                break;
            case GameOverCondition.NotEnoughMoney:
                {
                    messageEnum = MessageEnum.NotEnoughMoneyLose;
                }
                break;
            default:
                break;
        }

        _mMessageCoroutine = StartCoroutine(DisplayMessageCoroutine(GetMessageForEnum(messageEnum, out AudioClip audioClip), 0f, audioClip));
    }

    private IEnumerator DisplayMessageCoroutine(string text, float delayBeforeDisplay, AudioClip audio = null)
    {
        if(text == string.Empty)
        {
            yield break;
        }

        yield return new WaitForSecondsRealtime(delayBeforeDisplay);

        float lerp = 0f;

        _mMessageObject.SetActive(true);

        if(_mVocalAudioSource != null && audio != null)
        {
            _mVocalAudioSource.Stop();
            _mVocalAudioSource.clip = audio;
            _mVocalAudioSource.Play();
        }

        while (lerp < 1f)
        {
            lerp += Time.unscaledDeltaTime / 0.5f;

            _mMessageText.text = text.Substring(0, Mathf.FloorToInt(Mathf.Lerp(0, text.Length, lerp)));

            yield return null;
        }

        yield return new WaitForSecondsRealtime(Mathf.Lerp(_mMinMaxDisplayTimeValue.x, _mMinMaxDisplayTimeValue.y, Mathf.InverseLerp(_mMinMaxStringSizeForMaxDisplayTime.x, _mMinMaxStringSizeForMaxDisplayTime.y, text.Length)));

        _mMessageObject.SetActive(false);

        _mMessageCoroutine = null;

        yield break;
    }

    private string GetMessageForEnum(MessageEnum messageToDisplay, out AudioClip audioClip)
    {
        audioClip = null;
        foreach (MessageDatas item in _mMessageDatas)
        {
            if (item.MessageName == messageToDisplay && IsListValid(item.Messages))
            {
                int random = Random.Range(0, item.Messages.Count);

                if(item.AudioClip.Count > random)
                {
                    audioClip = item.AudioClip[random];
                }

                return item.Messages[random];
            }
        }

        return string.Empty;
    }

    private string GetMessageForEnum(MessageEnum messageEnum, AvailableObject objectName, out AudioClip audioClip)
    {
        string stringToReturn = string.Empty;
        MessageDatas messages = new MessageDatas();
        audioClip = null;

        foreach (MessageDatas item in _mMessageDatas)
        {
            if (item.MessageName == messageEnum)
            {
                messages = item;
            }
        }

        if (IsListValid(messages.Messages))
        {
            int index = 0;
            switch (messageEnum)
            {
                case MessageEnum.BuyRandomPiece:
                    {
                        index = (int)objectName - (int)AvailableObject.Wine;
                    }
                    break;
                case MessageEnum.BuyArmorPiece:
                    {
                        index = (int)objectName;
                    }
                    break;
                case MessageEnum.BuyAnInstrument:
                    {
                        index = (int)objectName - (int)AvailableObject.Flute;
                    }
                    break;
            }

            stringToReturn = messages.Messages[index];
            if (messages.AudioClip.Count > index)
            {
                audioClip = messages.AudioClip[index];
            }

        }

        return stringToReturn;
    }

    public bool IsListValid(List<string> list)
    {
        return list != null && list.Count > 0;
    }

    public void SetDayText(int dayCount)
    {
        _mDayCount.text = (dayCount+1).ToString();
    }
}
