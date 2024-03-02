using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEditor;
using UnityEngine;

public class HumanController : MonoBehaviour
{
    public float MovementSpeed = 5.0f;
    public float RotationSpeed = 10.0f;
    public StarterAssetsInputs inputs;
    [SerializeField] private GameObject _cameraRoot;
    
    private void Start()
    {
        _cameraRoot.transform.SetParent(null);
        _cameraRoot.name = "PlayerCamera";
        _cameraRoot.SetActive(true);
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
