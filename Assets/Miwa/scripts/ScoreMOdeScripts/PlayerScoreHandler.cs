using UnityEngine;

public class PlayerScoreHandler : MonoBehaviour
{
    private PlayerHealth _health;
    [SerializeField] private GameObject _itemPrefab; 
    private string _lastHitTag;

    private void Awake()
    {
        _health = GetComponent<PlayerHealth>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 最後に当たった相手を記録（今のロジックを維持）
        if (collision.gameObject.CompareTag("Gimmick") || collision.gameObject.CompareTag("Player"))
        {
            _lastHitTag = collision.gameObject.tag;
        }
    }

    public void HandleDeath()
    {
        // 1. 現在のスコアを取得 (GameManager_Mの変数から直接取るのが確実)
        int playerIndex = _health.playerIndex;
        int currentScore = GameManager_M.playerWins[playerIndex];

        // 2. ペナルティ計算（今のロジックを維持）
        int penalty = currentScore / 2;
        if (_lastHitTag == "Gimmick") penalty += 5; // 100で割るなら、500じゃなく5個分とかに調整

        // 3. スコアを減らす (AddScoreにマイナス値を渡す)
        GameManager_M.Instance.AddScore(playerIndex, -penalty);

        // 4. ペナルティの数だけアイテムを生成
        // (100で割るとペナルティが大きくないと出ないので、最低個数を保証すると賑やかになります)
        int itemCount = Mathf.Max(3, penalty);

        for (int i = 0; i < itemCount; i++)
        {
            GameObject item = Instantiate(_itemPrefab, transform.position + Vector3.up, Quaternion.identity);
            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb)
            {
                // 勢いよく飛ばす
                Vector3 force = new Vector3(Random.Range(-1f, 1f), 1.5f, Random.Range(-1f, 1f)) * 5f;
                rb.AddForce(force, ForceMode.Impulse);
            }
        }
        _lastHitTag = ""; // リセット
    }
}