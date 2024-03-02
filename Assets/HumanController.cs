using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using UnityEditor;
using UnityEngine;

public class HumanController : MonoBehaviour
{
    public float MovementSpeed = 0.0f;
    public StarterAssetsInputs inputs;

    // Update is called once per frame
    void Update()
    {
         if(inputs != null)
         {
            Vector2 direction2D = inputs.move;

            //Vector3 directionRotation = Vector3.RotateTowards(transform.forward, new Vector3(direction.x, 0, direction.y).normalized, 60, 0);
            //float Angle 
            Vector3 direction = new Vector3(direction2D.x, direction2D.y, 0).normalized;
            
            if(direction != Vector3.zero)
            {
            Vector3 movement = direction * MovementSpeed * Time.deltaTime;

            float angle = Vector3.SignedAngle(transform.up, direction, transform.forward);

            transform.RotateAround(transform.position, Vector3.forward, angle * Time.deltaTime * 5.0f);
            transform.Translate(-transform.up * MovementSpeed * Time.deltaTime);
            }

         }
    }
}
