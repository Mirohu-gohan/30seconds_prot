using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerInputController : MonoBehaviour
{
    Vector2 inputVer;

    public Vector2 InputVeer => inputVer; // 追加


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
        inputVer = context.ReadValue<Vector2>();

        //ステート変更のための入力受け取り
        stateManager.UpdateMoveState(inputVer);
        //移動処理のための入力受け取り
        move.SetMoveInput(inputVer);
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
