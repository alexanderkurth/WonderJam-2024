using System.Collections;
using System.Collections.Generic;
using game;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScreenUIController : MonoBehaviour
{
    public GameObject EndGameGO = null;

    public TextMeshProUGUI EndText = null;

    public void OnGameOver(TeamID id)
    {
        if(EndGameGO != null && EndText != null)
        {
            EndGameGO.SetActive(true);
            string name = GameManager.Instance.GetAnimalDatas().GetColorStringTeam(id);

            EndText.text = EndText.text.Replace("{}", name);
        }
    }
}
