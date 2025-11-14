using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoin : MonoBehaviour
{
    private SelectPlayer sp;

    public void OnPlayerJoined(PlayerInput input)
    {
        Debug.Log("プレイヤーが参加しました: " + input.devices[0].displayName);
    }


}
