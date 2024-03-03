using System;
using game;
using StarterAssets;
using UnityEngine;
using DG.Tweening;

public class HumanController : MonoBehaviour
{
    [SerializeField] private float radius = 10.0f;
    [SerializeField] private float radiusSaddle = 10.0f;
    [SerializeField] private GameObject _anchor;
    [SerializeField] private GameObject _cameraRoot;
    [SerializeField] private InteractionComponent2 _interactionComponent;

    public float MovementSpeed = 5.0f;
    public float RotationSpeed = 10.0f;
    public StarterAssetsInputs inputs;
    public int DashDistance = 1;
    private TeamID _teamID;

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

    public void OnBillyInteraction()
    {
        Debug.Log("OnInteraction()");

        if (_interactionComponent.bestTarget == null)
        {
            return;
        }

        BaseIA ia = _interactionComponent.bestTarget.GetComponent<BaseIA>();

        MontureController mc = _interactionComponent.bestSaddle;
        float distanceSaddle = Vector3.Distance(mc.transform.position, transform.position);

        if (distanceSaddle <= radiusSaddle)
        {
            ia.OnMerge(mc);
            ia = null;
            return;
        }

        if (ia != null)
        {
            if (ia.IsGrab)
            {
                ia.transform.localScale = Vector3.one;
                ia.transform.parent = null;
                ia.SetGrab(false);
                ia = null;
                return;
            }
            else
            {
                float distance = Vector3.Distance(ia.transform.position, transform.position);
                if (distance <= radius && _anchor.transform.childCount == 0)
                {
                    ia.OnGrab();
                    ia.transform.parent = _anchor.transform;
                    ia.transform.localPosition = Vector3.zero;
                    ia.transform.localScale = _anchor.transform.localScale;
                    return;
                }
            }
        }
    }
    public void OnAttack()
    {
        Dash(DashDistance);
    }

    public void OnHorseRidingInteraction()
    {
        throw new System.NotImplementedException();
    }
}
