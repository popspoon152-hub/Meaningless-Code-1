using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerDirection : MonoBehaviour
{
    [Header("��ǰ����״̬")]
    public Vector2 directionState;   // ��ǰ��������
    public bool hasInput;            // �Ƿ����ڰ������
    public Vector2 facingDirection;  // ��ǰ���򣨹������ű�ʹ�ã�

    private SpriteRenderer sr;
    private PlayerInputControls PlayerInputControls;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        PlayerInputControls = new PlayerInputControls();
        PlayerInputControls.Player.Move.performed += OnMove;
        PlayerInputControls.Player.Move.canceled += OnMoveCanceled;
    }

    private void OnEnable()
    {
        PlayerInputControls.Enable();
    }

    private void OnDisable()
    {
        PlayerInputControls.Disable();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        directionState = ctx.ReadValue<Vector2>();
        hasInput = directionState != Vector2.zero;

        //���µ�ǰ�������һ����Ч����
        if (hasInput)
        {
            facingDirection = directionState.normalized;
            //if (facingDirection.x > 0)
            //    sr.flipX = false;
            //else if (facingDirection.x < 0)
            //    sr.flipX = true;
        }
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        hasInput = false;
        directionState = Vector2.zero;
        // ������ facingDirection��������һ�γ���
    }
}
