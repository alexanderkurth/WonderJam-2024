using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEditor;
using UnityEngine;
using DG.Tweening;

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

    public int DashDistance = 1;

    private void Start()
    {
        _cameraRoot.transform.SetParent(null);
        _cameraRoot.name = "PlayerCamera";
        _cameraRoot.SetActive(true);
    }

    public InteractionState m_Sate = InteractionState.Invalid;

    public bool isDashing = false;
    public bool isPushed = false;


    // Update is called once per frame
    void Update()
    {
        if (inputs != null)
        {
            Vector2 direction2D = inputs.move;
            Vector3 direction = new Vector3(direction2D.x, direction2D.y, 0).normalized;

            if (direction != Vector3.zero)
            {
                // The game axis are X and Y
                // Make HumanController move depending of inputs
                transform.position += direction * MovementSpeed * Time.deltaTime;

                Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
            }
        }
    }

    public void Dash(int dashDistance)
    {
        if (isDashing == false)
        {
            isDashing = true;
            transform.DOMove(transform.position + transform.up * dashDistance, 0.2f)
                .OnComplete(() => isDashing = false);
        }
    }

    public void GetPushed(int pushDistance, Vector3 direction)
    {
        if (isPushed)
        {
            return;
        }
        isPushed = true;

        transform.DOMove(transform.position + direction * pushDistance, 1.0f)
        .SetEase(Ease.OutExpo)
        .OnComplete(() => isPushed = false);
    }

    public void OnInteraction()
    {
        Debug.Log("Player Interacted !");
    }

    public void OnAttack()
    {
        Debug.Log("Player Attacked !");
        Dash(DashDistance);
    }

    public void OnHorseRidingInteraction()
    {
        throw new System.NotImplementedException();
    }
}
