using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public struct TeamCameras
{
    public int TeamID;
    public List<Camera> Cameras;
}

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private List<TeamCameras> mTeamCameras = new List<TeamCameras>();

    private const float LERP_TIME = 0.5f;
    
    private void Start()
    {
        SetHorseCameraForTeam((int)game.TeamID.Team1, false, true);
        SetHorseCameraForTeam((int)game.TeamID.Team2, false, true);
    }

    public void SetHorseCameraForTeam(int teamID, bool isEnabled, bool isInstant = false)
    {
        List<Camera> cameras = GetTeamCameras(teamID);
        cameras[0].enabled = true;
        cameras[1].enabled = !isEnabled;

        float timing = isInstant ? 0f : LERP_TIME;
        float x = 0.5f * teamID;
        Rect camera1Rect = isEnabled ? new Rect(x, 0, 0.5f, 1) : new Rect(x,0.5f,0.5f,0.5f);
        StartCoroutine(LerpCameraInScreen(cameras[0], camera1Rect, timing));
        StartCoroutine(LerpCameraInScreen(cameras[1], new Rect(x,0,0.5f,0.5f), timing));
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
        foreach (TeamCameras teamCameras in mTeamCameras)
        {
            if (teamID == teamCameras.TeamID)
            {
                return teamCameras.Cameras;
            }
        }

        return new List<Camera>();
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
}
