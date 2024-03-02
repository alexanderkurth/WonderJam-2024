using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _gameOverExplanationText;
    [SerializeField] private string _gameOverOutofScreen;
    [SerializeField] private string _gameOverMoney;
    [SerializeField] private string _gameOverMadness;

    public void UpdateGameOverText(GameMode.GameOverCondition gameOverCondition)
    {
        switch (gameOverCondition)
        {
            case GameMode.GameOverCondition.OutOfScreen:
                _gameOverExplanationText.text = _gameOverOutofScreen;
                break;
            case GameMode.GameOverCondition.Madness:
                _gameOverExplanationText.text = _gameOverMadness;
                break;
            case GameMode.GameOverCondition.NotEnoughMoney:
                _gameOverExplanationText.text = _gameOverMoney;
                break;
        }
    }
}
