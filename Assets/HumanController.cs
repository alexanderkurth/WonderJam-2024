using game;
using StarterAssets;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using TMPro;

public class HumanController : MonoBehaviour
{
    public float radius = 10.0f;
    public float radiusSaddle = 10.0f;
    [SerializeField] private GameObject _anchor;
    [SerializeField] private InteractionComponent2 _interactionComponent;
    [SerializeField] private PlayerInput _playerInput;
    public TextMeshProUGUI _text;
    public TextMeshProUGUI _saddleText;

    private int _playerID = 0;
    public int PlayerID { get { return _playerID; } }
    public float MovementSpeed = 5.0f;
    public float RotationSpeed = 10.0f;
    public StarterAssetsInputs inputs;
    public int DashDistance = 1;
    private TeamID _teamID;
    public TeamID TeamID { get { return _teamID; } }
    private MontureController _currentMount = null;
    public MontureController CurrentMount
    {
        get { return _currentMount; }
    }

    [SerializeField] private GameObject _visualParent;
    public GameObject EcuyerGameObject = null;
    public GameObject ChevalierGameObject = null;

    public List<SpriteRenderer> EcuyerSpritesToModify;
    public List<SpriteRenderer> ChevalierSpritesToModify;

    public bool isDashing = false;
    public bool isPushed = false;

    [Header("Sounds")]
    [SerializeField]
    private AK.Wwise.Event SlapSoundEvent = null;
    [SerializeField]
    private AK.Wwise.Event DashSoundEvent = null;

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

        if (EcuyerGameObject != null && ChevalierGameObject != null)
        {
            List<SpriteRenderer> sprites = EcuyerSpritesToModify;

            if (playerID != 0)
            {
                EcuyerGameObject.active = false;
                ChevalierGameObject.active = true;

                sprites = ChevalierSpritesToModify;
            }

            Color color = GameManager.Instance.GetAnimalDatas().GetColorForTeam(teamID);

            foreach (SpriteRenderer spriteRenderer in sprites)
            {
                spriteRenderer.color = color;
            }
        }
    }

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

        /*BaseIA ia = _interactionComponent.bestTarget;
        MontureController mc = _interactionComponent.bestSaddle;
        if (ia == null || _text == null || mc == null)
        {
            return;
        }

        float distanceSaddle = Vector3.Distance(mc.GetSaddlePosition(), transform.position);
        float distance = Vector3.Distance(ia.transform.position, transform.position);

        bool isDistanceValid = distance < radius;
        if (isDistanceValid)
        {
            Vector3 pos = ia.transform.position;// _playerInput.camera.WorldToScreenPoint(ia.transform.position);

            _text.gameObject.transform.position = pos;
        }
        bool canInterac = isDistanceValid;
        _text.gameObject.SetActive(canInterac);

        bool isDistanceSaddlevalid = distanceSaddle < radiusSaddle;
        if (isDistanceSaddlevalid)
        {
            Vector3 pos = _playerInput.camera.WorldToScreenPoint(mc.transform.position);
            _saddleText.gameObject.transform.position = pos;
            if (mc.IsReadyToMount())
            {
                _saddleText.text = "MOUNT";
            }
        }
        bool condition = _currentMount == null && mc.IsReadyToMount() || ia.IsGrab && isDistanceSaddlevalid;
        _saddleText.gameObject.SetActive(condition);
        mc.SetOutlineVisibility(condition);*/
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
            DashSoundEvent.Post(gameObject);
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

        SlapSoundEvent.Post(gameObject);

        BaseIA ia = _interactionComponent.bestTarget.GetComponent<BaseIA>();
        if (ia != null && ia.IsGrab)
        {
            DropItem(ia);
        }

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
        float distanceSaddle = Vector3.Distance(mc.GetSaddlePosition(), transform.position);

        if (distanceSaddle <= radiusSaddle)
        {
            if (ia.IsGrab)
            {
                ia.transform.parent = null;
                InteractionManager2.Instance.m_Animals.Remove(ia);
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
                DropItem(ia);
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
                    ia.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
                    return;
                }
            }
        }
    }

    private void DropItem(BaseIA ia)
    {
        ia.transform.parent = null;
        ia.transform.localScale = Vector3.one;
        ia.SetGrab(false);
        ia = null;
    }

    private void ToggleMount(MontureController mc)
    {
        if (mc.TeamID == _teamID)
        {
            if (_currentMount == null)
            {
                _currentMount = mc;
                transform.parent = mc.GetSaddleTransform();
                transform.localPosition = Vector3.zero;
            }
            else
            {
                _currentMount = null;
                transform.parent = null;
                Vector3 tempPos = mc.GetSaddlePosition();
                tempPos.x += _playerID == 1 ? -2 : 2;
                transform.position = tempPos;
            }

            _visualParent.SetActive(_currentMount == null);
        }
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
