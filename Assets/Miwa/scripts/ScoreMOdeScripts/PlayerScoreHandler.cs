using System;
using UnityEngine;

public class PlayerScoreHandler : MonoBehaviour
{
    private PlayerHealth _health;
    [SerializeField] private GameObject _itemPrefab; 
    private string _lastHitTag;

    private int lastAttackerIndex = -1;

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

    void SetLastAttacker(int index)
    {
        lastAttackerIndex = index;
    }


    public void HandleDeath()
    {
        // 1. 現在のスコアを取得 (GameManager_Mの変数から直接取るのが確実)
        int playerIndex = _health.playerIndex;
        int currentScore = GameManager_M.playerWins[playerIndex];

        if (currentScore <= 0) return;

        // 2. ペナルティ計算（今のロジックを維持）
        int penalty = Mathf.Max(1, currentScore / 2);

        // 3. スコアを減らす (AddScoreにマイナス値を渡す)
        GameManager_M.Instance.AddScore(playerIndex, -penalty);

        // 4. ペナルティの数だけアイテムを生成
        int itemCount = Mathf.Max(3, penalty);

        if (lastAttackerIndex != -1)
        {
            GameManager_M.Instance.AddScore(lastAttackerIndex, 3);
            lastAttackerIndex = -1;
        }

        for (int i = 0; i < penalty; i++)
        {
            if (_itemPrefab == null) break;

            // リスポーン地点（現在の位置）の少し上からバラまく
            GameObject item = Instantiate(_itemPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb)
            {
                Vector3 force = new Vector3(UnityEngine.Random.Range(-1f, 1f), 1.5f, UnityEngine.Random.Range(-1f, 1f)).normalized * 5f;
                rb.AddForce(force, ForceMode.Impulse);
            }
        }
    }


}