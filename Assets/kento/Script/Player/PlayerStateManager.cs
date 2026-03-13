using UnityEngine;
using UnityEngine.InputSystem;

public enum MoveState
{
    Idle,Walk
}
public enum ActionState
{
    None,Charge, Attack
}
public enum AttackPower
{
    None,Weak,Strong
}
public enum State
{
    None,Knockback
}
//ステート管理クラス
public class PlayerStateManager : MonoBehaviour
{
    //ステートのメソッド,代入はこのクラスのみ参照は別クラスでも可
    public MoveState MoveState {get; private set;} = MoveState.Idle;
    public ActionState ActionState {get; private set;} = ActionState.None;
    public AttackPower AttackPower { get; private set; } = AttackPower.None;
    public State State {get; private set;} = State.None;

    public void UpdateMoveState(Vector2 inputVere)
    {
        //入力がされたらステートを変更(? = true,: = false) 
        MoveState = inputVere.sqrMagnitude > 0.01 ? MoveState.Walk : MoveState.Idle;
    }
    public void SetActionState(ActionState state)
    {
        //ステートの変更
        ActionState = state;
    }
    public void SetAttackPower(AttackPower state)
    {
        AttackPower = state;
    }
    public void SetState(State state)
    {
        State = state;
    }

    /* private void Update()
     {
         Debug.Log($"MoveState: {MoveState}, ActionState: {ActionState}");
     }*/
}
