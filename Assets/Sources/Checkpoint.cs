using System.Collections;
using System.Collections.Generic;
using game;
using Unity.VisualScripting;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool _IsValidatedByTeam1 = false;
    private bool _IsValidatedByTeam2 = false;

    private int _CheckpointIndex = 0;

    public List<MontureController> Montures;

    public float SquareValidatonDistance = 5.0f;

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

    void Update()
    {
        foreach(MontureController controller in Montures)
        {
            if(IsValidatedByTeam(controller.TeamID))
            {
                continue;
            }

            Vector3 vector = controller.transform.position - transform.position;
            if(vector.sqrMagnitude < SquareValidatonDistance)
            {
                CheckMonture(controller);
            }
        }

    }

    private void CheckMonture(MontureController monture)
    {
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

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, Mathf.Sqrt(SquareValidatonDistance));
    }
}
