using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerInputController : MonoBehaviour
{
    Vector2 inputVeer;

    [HideInInspector] public bool isStart;
    [HideInInspector] public bool isAttack1;
    [HideInInspector] public bool isAttack2;
    Animator animator;

    private PlayerStateManager stateManager;
    private MoveController move;
    private AtackController atack;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        stateManager = GetComponent<PlayerStateManager>();
        move = GetComponent<MoveController>();
        atack = GetComponent<AtackController>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputVeer = context.ReadValue<Vector2>();

        //ステート変更のための入力受け取り
        stateManager.UpdateMoveState(inputVeer);
        //移動処理のための入力受け取り
        move.SetMoveInput(inputVeer);
    }

    public void OnAtatck(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            atack.Shot(0);
        }
        if (context.canceled)
        {
            atack.Shot(1);
        }
    }

    private void Update()
    {
        float mag = inputVeer.magnitude;
        animator.SetFloat("Speed", mag);
        animator.SetBool("IsChage", isStart);
        animator.SetBool("IsAttack1", isAttack1);
        animator.SetBool("IsAttack2", isAttack2);
    }
}
