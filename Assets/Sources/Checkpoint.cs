using System.Collections;
using System.Collections.Generic;
using game;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool _IsValidatedByTeam1 = false;
    private bool _IsValidatedByTeam2 = false;

    private int _CheckpointIndex = 0;

    public bool IsValidatedByTeam(TeamID id)
    {
        switch (id)
        {
            case TeamID.Team1:
            {
                return _IsValidatedByTeam1;
            }
            case TeamID.Team2:
            {
                return _IsValidatedByTeam2;
            }
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter " + other.gameObject);
        MontureController monture = other.gameObject.GetComponent<MontureController>();
        if(monture != null)
        {
            if(_CheckpointIndex == 0) //First checkpoint
            {
                SetValidated(monture.TeamID);
            }
            else if(GameManager.Instance.IsCheckpointValidated(_CheckpointIndex - 1, monture.TeamID)) //replace by teamID
            {
                SetValidated(monture.TeamID);
            }
             else
            {
                return;
            }

            GameManager.Instance.NotifyNewCheckpointValidatedByTeam(monture.TeamID, _CheckpointIndex);
        }

        //TestCode because we don't have the monture :'(
        // HumanController controller = other.gameObject.GetComponentInParent<HumanController>();
        // if(controller != null)
        // {
        //     if(_CheckpointIndex == 0) //First checkpoint
        //     {
        //         SetValidated(TeamID.Team1);
        //     }
        //     else if(GameManager.Instance.IsCheckpointValidated(_CheckpointIndex - 1, TeamID.Team1)) //replace by teamID
        //     {
        //         SetValidated(TeamID.Team1);
        //     }
        //     else
        //     {
        //         return;
        //     }

        //     GameManager.Instance.NotifyNewCheckpointValidatedByTeam(TeamID.Team1, _CheckpointIndex);
        // }
    }

    private void SetValidated(TeamID teamID)
    {
        switch(teamID)
        {
            case TeamID.Team1:
            {
                _IsValidatedByTeam1 = true;
                break;
            }
            case TeamID.Team2:
            {
                _IsValidatedByTeam2 = true;
                break;
            }
        }
    }

    public void SetIndex(int index)
    {
        _CheckpointIndex = index;
    }
}
