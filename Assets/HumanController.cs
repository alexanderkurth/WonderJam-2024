using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEditor;
using UnityEngine;

public class HumanController : MonoBehaviour
{
    public enum InteractionState : Int32
    {
        Idle = 0,
        Grab = 1,
        Merge = 2,

        Count = 3,
        Invalid = 4
    }

    [SerializeField] private float radius = 10.0f;
    [SerializeField] private float radiusSaddle = 10.0f;
    public float MovementSpeed = 5.0f;
    public float RotationSpeed = 10.0f;
    public StarterAssetsInputs inputs;
    [SerializeField] private GameObject _cameraRoot;
    [SerializeField] private InteractionComponent2 _interactionComponent;

    public Action TryInteract;

    private void Start()
    {
        _cameraRoot.transform.SetParent(null);
        _cameraRoot.name = "PlayerCamera";
        _cameraRoot.SetActive(true);
    }

    public InteractionState m_Sate = InteractionState.Invalid;

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

    public void OnInteraction()
    {
        if(_interactionComponent.bestTarget == null)
        {
            return;
        }

        BaseIA ia = _interactionComponent.bestTarget.GetComponent<BaseIA>();

        MontureController mc = _interactionComponent.bestSaddle;
        float distanceSaddle = Vector3.Distance(mc.transform.position, transform.position);

        if (distanceSaddle <= radiusSaddle && m_Sate == InteractionState.Grab)
        {
            ia.OnMerge(mc);
            ia = null;
            m_Sate = InteractionState.Idle;
            return;
        }

        float distance = Vector3.Distance(ia.transform.position, transform.position);
        if (m_Sate != InteractionState.Grab && distance <= radius)
        {
            if (ia != null && !ia.IsGrab)
            {
                m_Sate = InteractionState.Grab;
                ia.OnGrab();
                ia.transform.parent = transform;
                ia.transform.localScale *= 0.75f;
                return;
            }
        }
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
