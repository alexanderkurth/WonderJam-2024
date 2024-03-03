using System.Collections;
using System.Collections.Generic;
using game;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    private const float LERP_TIME = 0.5f;
    
    [SerializeField] private Camera _camera;
    [SerializeField] private SimpleCameraFollow _simpleCameraFollow;

    public int PlayerID { get; private set; }
    private TeamID TeamID = TeamID.Invalid; 
    public HumanController Target { get; private set; }

    public GameObject ConnectControllerGO = null;

    public void SetTeamID(TeamID teamID, int playerID)
    {
        TeamID = teamID;
        PlayerID = playerID;
    }
    
    public void Initialize(HumanController owner)
    {
        if (owner != null)
        {
            _simpleCameraFollow.m_Target = owner.transform;
            owner.GetComponent<PlayerInput>().camera = _camera;
        }
        
        Target = owner;
        SetUIActive(Target == null);
    }

    public void SetUIActive(bool isActive)
    {
        _simpleCameraFollow.SetCameraActive(!isActive);

        if(ConnectControllerGO != null)
        {
            ConnectControllerGO.SetActive(isActive);
        }
    }

    public void HandleCameraChange(bool isEnabled, bool isInstant)
    {
        float timing = isInstant ? 0f : LERP_TIME;
        float x = 0.5f * (int)TeamID;
        float height = GameManager.Instance.IsTwoPlayerMod ? 1f : 0.5f;
        
        Rect cameraRect = default;
        if (PlayerID == 0)
        {
            _camera.enabled = true;
            _simpleCameraFollow.SetCameraActive(true);
            cameraRect = isEnabled ? new Rect(x, 0, 0.5f, 1) : new Rect(x,1-height,0.5f,height);
        }
        else
        {
            _camera.enabled = !isEnabled;
            _simpleCameraFollow.SetCameraActive(!isEnabled);
            cameraRect = new Rect(x, 0, 0.5f, height);
        }

        StartCoroutine(LerpCameraInScreen(cameraRect, timing));
    }
    
    public IEnumerator LerpCameraInScreen(Rect newRect, float timing)
    {
        float lerp = 0f;
        Rect initialRect = new Rect(_camera.rect);
        
        while (lerp <= 1f)
        {
            lerp += Time.deltaTime / LERP_TIME;
            Rect tempRect = _camera.rect;
            tempRect.x = Mathf.Lerp(initialRect.x, newRect.x, lerp);
            tempRect.y = Mathf.Lerp(initialRect.y, newRect.y, lerp);
            tempRect.width = Mathf.Lerp(initialRect.width, newRect.width, lerp);
            tempRect.height = Mathf.Lerp(initialRect.height, newRect.height, lerp);
            _camera.rect = tempRect;
            
            yield return null;
        }
    }
}
