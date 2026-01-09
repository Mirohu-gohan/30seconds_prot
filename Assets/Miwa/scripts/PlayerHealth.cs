using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    public int playerIndex; 

    void Start()
    {
        // PlayerInputコンポーネントからインデックスを取得（もしあれば）
        var input = GetComponent<PlayerInput>();
        if (input != null) playerIndex = input.playerIndex;

        // インデックスを添えてGameManagerに登録
        if (GameManager_M.Instance != null)
        {
            GameManager_M.Instance.RegisterPlayer(gameObject, playerIndex);
        }
    }
    public void OnFallOut()
    {
        // 自分自身を引数に入れて呼ぶ
        if (GameManager_M.Instance != null)
        {
            GameManager_M.Instance.OnPlayerEliminated(gameObject);
        }
        Destroy(gameObject);
    }
}