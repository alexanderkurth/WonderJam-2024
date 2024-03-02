using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using UnityEditor;
using UnityEngine;

public class HumanController : MonoBehaviour
{
    public float MovementSpeed = 5.0f;
    public float RotationSpeed = 10.0f;
    public StarterAssetsInputs inputs;

    // Update is called once per frame
    void Update()
    {
         if(inputs != null)
         {
            Vector2 direction2D = inputs.move;
            Vector3 direction = new Vector3(direction2D.x, direction2D.y, 0).normalized;

            if(direction != Vector3.zero)
            {
                float angle = Vector3.SignedAngle(transform.up, direction, transform.forward);

                transform.RotateAround(transform.position, Vector3.forward, angle * Time.deltaTime *RotationSpeed);
                transform.Translate(-transform.up * MovementSpeed * Time.deltaTime);
            }

         }
    }
}
