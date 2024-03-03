using game;
using StarterAssets;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class HumanController : MonoBehaviour
{
    [SerializeField] private float radius = 10.0f;
    [SerializeField] private float radiusSaddle = 10.0f;
    [SerializeField] private GameObject _anchor;
    [SerializeField] private GameObject _cameraRoot;
    [SerializeField] private InteractionComponent2 _interactionComponent;

    private int _playerID = 0;
    public float MovementSpeed = 5.0f;
    public float RotationSpeed = 10.0f;
    public StarterAssetsInputs inputs;
    public int DashDistance = 1;
    private TeamID _teamID;
    private MontureController _currentMount = null;

    [SerializeField] private GameObject _visualParent;
    public GameObject EcuyerGameObject = null;
    public GameObject ChevalierGameObject = null;

    public List<SpriteRenderer> EcuyerSpritesToModify;
    public List<SpriteRenderer> ChevalierSpritesToModify;

    public Color Team1Color = Color.red;
    public Color Team2Color = Color.blue;
    
    private void Start()
    {
        _cameraRoot.transform.SetParent(null);
        _cameraRoot.name = "PlayerCamera";
        _cameraRoot.SetActive(true);
    }

    #if UNITY_EDITOR
    [ContextMenu("TestINIT")]
    void TestInit()
    {
        Initialize(TeamID.Team2, 1);
    }
    #endif

    public void Initialize(TeamID teamID, int playerID)
    {
        _teamID = teamID;
        _playerID = playerID;

        

        if(EcuyerGameObject != null && ChevalierGameObject != null)
        {
            List<SpriteRenderer> sprites = EcuyerSpritesToModify;

            if(playerID != 0)
            {
                EcuyerGameObject.active = false;
                ChevalierGameObject.active = true;

                sprites = ChevalierSpritesToModify;
            }

            Color color = (teamID == TeamID.Team1) ? Team1Color : Team2Color;

            foreach(SpriteRenderer spriteRenderer in sprites)
            {
                spriteRenderer.color = color;
            }
        }
    }

    public bool isDashing = false;
    public bool isPushed = false;


    // Update is called once per frame
    void Update()
    {
        if (_currentMount != null)
        {
            return;
        }
        
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
        if (_currentMount != null)
        {
            return;
        }
        
        if (isDashing == false)
        {
            isDashing = true;
            transform.DOMove(transform.position + transform.up * dashDistance, 0.2f)
                .OnComplete(() => isDashing = false);
        }
    }

    public void GetPushed(int pushDistance, Vector3 direction)
    {
        if (_currentMount != null)
        {
            return;
        }
        
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
        if (_interactionComponent.bestTarget == null)
        {
            return;
        }

        BaseIA ia = _interactionComponent.bestTarget.GetComponent<BaseIA>();

        MontureController mc = _interactionComponent.bestSaddle;
        float distanceSaddle = Vector3.Distance(mc.transform.position, transform.position);

        if (distanceSaddle <= radiusSaddle)
        {
            if(ia.IsGrab)
            {
                ia.transform.parent = null;
                ia.OnMerge(mc);
                ia = null;
            }
            else if (mc.IsReadyToMount())
            {
                ToggleMount(mc);
            }

            return;
        }

        if (_currentMount != null)
        {
            return;
        }

        if (ia != null)
        {
            if (ia.IsGrab)
            {
                ia.transform.parent = null;
                ia.transform.localScale = Vector3.one;
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
                    ia.transform.localRotation = Quaternion.Euler(new Vector3(0f,0f,-90f));
                    return;
                }
            }
        }
    }

    private void ToggleMount(MontureController mc)
    {
        if (_currentMount == null)
        {
            _currentMount = mc;
            transform.parent = _currentMount.transform;
            transform.localPosition = Vector3.zero;            
        }
        else
        {
            _currentMount = null;
            transform.parent = null;
            Vector3 tempPos = mc.transform.position;
            tempPos.x += _playerID == 1 ? -2 : 2;
            transform.position = tempPos;
        }
        
        _visualParent.SetActive(_currentMount == null);
    }

    public void OnAttack()
    {
        if (_currentMount != null)
        {
            return;
        }
        
        Dash(DashDistance);
    }

    public void TriggerHorseRidingInteraction(InputValue value, bool is4PlayerBehavior = true)
    {
        if (_currentMount != null)
        {
            bool isFirstPlayer = _playerID == 1;
            _currentMount.TriggerLegMovement(isFirstPlayer, value.isPressed, is4PlayerBehavior);
        }
    }
}
