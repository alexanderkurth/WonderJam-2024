using System.Collections;
using System.Collections.Generic;
using game;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : Singleton<CameraManager>
{
    private Dictionary<int, List<Camera>> mTeamCameras = new Dictionary<int, List<Camera>>();

    private const float LERP_TIME = 0.5f;
    
    public void Initialize()
    {
        SetHorseCameraForTeam((int)game.TeamID.Team1, false, true);
        SetHorseCameraForTeam((int)game.TeamID.Team2, false, true);
    }

    public void SetHorseCameraForTeam(int teamID, bool isEnabled, bool isInstant = false)
    {
        List<Camera> cameras = GetTeamCameras(teamID);
        bool isFirst = true;
        float height = GameManager.Instance.IsTwoPlayerMod ? 1f : 0.5f;
        foreach (Camera camera in cameras)
        {
            float timing = isInstant ? 0f : LERP_TIME;
            float x = 0.5f * teamID;
            Rect cameraRect = default;
            if (isFirst)
            {
                isFirst = false;
                camera.enabled = true;
                cameraRect = isEnabled ? new Rect(x, 0, 0.5f, 1) : new Rect(x,1-height,0.5f,height);
            }
            else
            {
                camera.enabled = !isEnabled;
                cameraRect = new Rect(x, 0, 0.5f, height);
            }

            StartCoroutine(LerpCameraInScreen(camera, cameraRect, timing));
        }
    }

    private IEnumerator LerpCameraInScreen(Camera camera, Rect newRect, float timing)
    {
        float lerp = 0f;
        Rect initialRect = new Rect(camera.rect);
        
        while (lerp <= 1f)
        {
            lerp += Time.deltaTime / LERP_TIME;
            Rect tempRect = camera.rect;
            tempRect.x = Mathf.Lerp(initialRect.x, newRect.x, lerp);
            tempRect.y = Mathf.Lerp(initialRect.y, newRect.y, lerp);
            tempRect.width = Mathf.Lerp(initialRect.width, newRect.width, lerp);
            tempRect.height = Mathf.Lerp(initialRect.height, newRect.height, lerp);
            camera.rect = tempRect;
            
            yield return null;
        }
    }

    private List<Camera> GetTeamCameras(int teamID)
    {
        if (mTeamCameras.ContainsKey(teamID))
        {
            return mTeamCameras[teamID];
        }

        return new List<Camera>();
    }

    public void PairPlayerToTeam(int teamId, PlayerInput value)
    {
        if (mTeamCameras.ContainsKey(teamId))
        {
            mTeamCameras[teamId].Add(value.camera);
        }
        else
        {
            List<Camera> linkedPlayer = new List<Camera>();
            linkedPlayer.Add(value.camera);
            mTeamCameras.Add(teamId, linkedPlayer);
        }
    }
    
    #if UNITY_EDITOR
    [ContextMenu("ToggleCameraStateTeam1")]
    private void ToggleCameraStateTeam1()
    {
        SetHorseCameraForTeam(0, GetTeamCameras(0)[1].enabled);
    }
    
    [ContextMenu("ToggleCameraStateTeam2")]
    private void ToggleCameraStateTeam2()
    {
        SetHorseCameraForTeam(1, GetTeamCameras(1)[1].enabled);
    }
    #endif
    public void UnpairPlayerToTeam(int teamId, PlayerInput value)
    {
        if (mTeamCameras.ContainsKey(teamId))
        {
            mTeamCameras[teamId].Remove(value.camera);
        }
    }
}
