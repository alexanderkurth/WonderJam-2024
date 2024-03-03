using System;
using System.Collections;
using System.Collections.Generic;
using game;
using StarterAssets;
using UnityEditor;
using UnityEngine;

public class HumanController : MonoBehaviour
{
    public enum InteractionState : Int32
    {
        Idle = 0,
        Grab = 1,

        Count = 2,
        Invalid = 3
    }

    public float MovementSpeed = 5.0f;
    public float RotationSpeed = 10.0f;
    public StarterAssetsInputs inputs;
    [SerializeField] private GameObject _cameraRoot;
    private TeamID _teamID;
    public InteractionState m_State = InteractionState.Invalid;
    
    private void Start()
    {
        _cameraRoot.transform.SetParent(null);
        _cameraRoot.name = "PlayerCamera";
        _cameraRoot.SetActive(true);
    }

    public void Initialize(TeamID teamID)
    {
        _teamID = teamID;
    }

    // Update is called once per frame
    void Update()
    {
         if(inputs != null)
         {
            Vector2 direction2D = inputs.move;
            Vector3 direction = new Vector3(direction2D.x, direction2D.y, 0).normalized;

            if(direction != Vector3.zero)
            {
                // The game axis are X and Y
                // Make HumanController move depending of inputs
                transform.position += direction * MovementSpeed * Time.deltaTime;

                Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
            }
         }
    }

    public void OnInteraction()
    {
        Debug.Log("Player Interacted !");
    }

    public void OnAttack()
    {
        throw new System.NotImplementedException();
    }

    public void OnHorseRidingInteraction()
    {
        throw new System.NotImplementedException();
    }
}
