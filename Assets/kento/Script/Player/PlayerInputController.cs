using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private PlayerStateManager stateManager;
    private MoveController move;
    private AtackController atack;

    private void Awake()
    {
        stateManager = GetComponent<PlayerStateManager>();
        move = GetComponent<MoveController>();
        atack = GetComponent<AtackController>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 inputVeer = context.ReadValue<Vector2>();

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
}
