using System.Collections;
using System.Collections.Generic;
using game;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private PlayerCamera _playerCameraPrefab = null;
    private Dictionary<int, List<PlayerCamera>> mTeamCameras = new Dictionary<int, List<PlayerCamera>>();

    public void Initialize()
    {
        int playerPerTeam = GameManager.Instance.IsTwoPlayerMod ? 1 : 2;
        int playerCount = playerPerTeam * 2;
        for (int i = 0; i < playerCount * 2; i++)
        {
            PlayerCamera newCamera = Instantiate(_playerCameraPrefab);
            int teamId = Mathf.FloorToInt(i / playerPerTeam);
            int playerID = (i + 1) % playerPerTeam;
            newCamera.SetTeamID((TeamID)teamId, playerID);
            newCamera.SetUIActive(true);
            
            if (mTeamCameras.ContainsKey(teamId))
            {
                mTeamCameras[teamId].Add(newCamera);
            }
            else
            {
                List<PlayerCamera> linkedPlayer = new List<PlayerCamera>();
                linkedPlayer.Add(newCamera);
                mTeamCameras.Add(teamId, linkedPlayer);
            }
        }

        SetHorseCameraForTeam((int)game.TeamID.Team1, false, true);
        SetHorseCameraForTeam((int)game.TeamID.Team2, false, true);
    }

    public void SetHorseCameraForTeam(int teamID, bool isEnabled, bool isInstant = false)
    {
        List<PlayerCamera> cameras = GetTeamCameras(teamID);
        foreach (PlayerCamera camera in cameras)
        {
            camera.HandleCameraChange(isEnabled, isInstant);
        }
    }

    private List<PlayerCamera> GetTeamCameras(int teamID)
    {
        if (mTeamCameras.ContainsKey(teamID))
        {
            return mTeamCameras[teamID];
        }

        return new List<PlayerCamera>();
    }

    public void PairPlayerToTeam(int teamId, PlayerInput value)
    {
        HumanController humanController = value.GetComponent<HumanController>();
        List<PlayerCamera> playerCameras = GetTeamCameras(teamId);
        
        foreach (PlayerCamera playerCamera in playerCameras)
        {
            if (playerCamera.PlayerID == humanController.PlayerID)
            {
                playerCamera.Initialize(humanController);                
            }
        }
    }

    public void UnpairPlayerToTeam(int teamId, PlayerInput value)
    {
        HumanController humanController = value.GetComponent<HumanController>();
        List<PlayerCamera> playerCameras = GetTeamCameras(teamId);

        foreach (PlayerCamera playerCamera in playerCameras)
        {
            if (playerCamera.PlayerID == humanController.PlayerID)
            {
                playerCamera.Initialize(null);                
            }
        }
    }
}