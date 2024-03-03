using System.Collections;
using System.Collections.Generic;
using AK.Wwise;
using game;
using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public List<Checkpoint> Checkpoints;

    public ScreenUIController ScreenUIController = null;

    public float EndGameDuration = 5.0f;

    private float EndGameTimestamp = 0.0f;
    private bool IsGameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        int index = 0;
        foreach (Checkpoint cp in Checkpoints)
        {
            cp.SetIndex(index, this);
            index++;
        }
    }

    public void NotifyNewCheckpointValidatedByTeam(game.TeamID teamID, int cpIndex)
    {
        Debug.Log("NotifyNewCheckpointValidatedByTeam " + cpIndex);

        if(cpIndex == Checkpoints.Count - 1)
        {
            if (ScreenUIController != null)
            {
                Debug.Log("Won");
                ScreenUIController.OnGameOver(teamID);
                IsGameOver = true;
                EndGameTimestamp = Time.timeSinceLevelLoad;
            }
        }
    }

    public bool IsCheckpointValidated(int index, game.TeamID teamID)
    {
        if (index < Checkpoints.Count)
        {
            return Checkpoints[index].IsValidatedByTeam(teamID);
        }
        return false;
    }

    private void Update()
    {
        if(IsGameOver)
        {
            if(Time.timeSinceLevelLoad - EndGameTimestamp > EndGameDuration)
            {
                GameManager.Instance.ChangeState(GameManager.State.MainMenu);
            }
        }
    }
}
