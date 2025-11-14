using UnityEngine;
using UnityEngine.SceneManagement; // ゲームリセットが必要な場合に使用

public class YBoundaryDestroyer : MonoBehaviour
{
    // プレイヤーがこれより下に行った場合に破壊するY座標
    [Tooltip("このY座標を下回るとプレイヤーを破壊します。")]
    [SerializeField]
    private float deathYCoordinate = -10.0f;

    // ゲーム開始時のデバッグチェック
    void Start()
    {
        float initialY = transform.position.y;
    }

    void Update()
    {
        // プレイヤーの現在のY座標を取得
        float currentY = transform.position.y;

        // 設定されたY座標を下回ったかチェックする
        // 問題解決のため、ここでログを出し、破壊の直前を確認します。
        if (currentY < deathYCoordinate)
        {
            Debug.LogWarning(gameObject.name + " が破壊境界(" + deathYCoordinate + ")を下回りました。現在Y: " + currentY);
            DestroyPlayer();
        }
    }

    void DestroyPlayer()
    {
        // プレイヤーのGameObjectをシーンから削除
        Destroy(gameObject);

        // ★備考: 破壊後、シーンをリセットしたい場合は以下のコメントを解除してください。
        // Time.timeScale = 1f;
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
