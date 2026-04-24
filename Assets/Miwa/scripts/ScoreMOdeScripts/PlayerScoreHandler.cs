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
        // 自分が他のプレイヤーにぶつかった時
        if (collision.gameObject.CompareTag("Player"))
        {
            // 相手のScoreHandlerを取得して、自分（this）のIDを「攻撃者」として登録する
            var targetHandler = collision.gameObject.GetComponent<PlayerScoreHandler>();
            if (targetHandler != null)
            {
                // _health.playerIndex は自分のID
                targetHandler.SetLastAttacker(_health.playerIndex);
                Debug.Log($"Player {targetHandler._health.playerIndex} に攻撃を記録！ 攻撃者: {_health.playerIndex}");
            }
        }
    }

    // 外部から攻撃者IDをセットする場合（弾丸などから呼ぶ用）
    public void SetLastAttacker(int index)
    {
        lastAttackerIndex = index;
    }

    public void HandleDeath()
    {
        int playerIndex = _health.playerIndex;
        // currentScores から今回のスコアを取得
        int currentScore = GameManager_M.currentScores[playerIndex];

        // 1. 【ボーナス加点】自分を落とした相手がいれば、その人に加点
        if (lastAttackerIndex != -1 && lastAttackerIndex != playerIndex)
        {
            Debug.Log($"Player {lastAttackerIndex} がボーナス獲得！");
            GameManager_M.Instance.AddScore(lastAttackerIndex, 3);
            lastAttackerIndex = -1; // IDリセット
        }

        // 2. 【アイテム放出判定】所持スコアが0ならここで終了
        if (currentScore <= 0) return;

        // 3. 【ペナルティ計算】ここで初めて宣言する（1回だけ！）
        int penalty = Mathf.Max(1, currentScore / 2);

        // 自分のスコアを減らす
        GameManager_M.Instance.AddScore(playerIndex, -penalty);

        // 4. 【アイテム生成】penalty の数だけループ
        for (int i = 0; i < penalty; i++)
        {
            if (_itemPrefab == null) break;

            GameObject itemObj = Instantiate(_itemPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
            ScoreItem itemScript = itemObj.GetComponent<ScoreItem>();

            if (itemScript != null)
            {
                Vector3 randomDir = new Vector3(
                    UnityEngine.Random.Range(-1.0f, 1.0f),
                    1.5f,
                    UnityEngine.Random.Range(-1.0f, 1.0f)
                ).normalized;

                itemScript.Launch(randomDir, 5f);
            }
        }

        _lastHitTag = "";
    }
}