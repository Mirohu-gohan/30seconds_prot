using System;
using System.Collections;
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
        int currentScore = GameManager_M.currentScores[playerIndex];

        //加点（落とした相手へのプレゼント）
        if (lastAttackerIndex != -1 && lastAttackerIndex != playerIndex)
        {
            GameManager_M.Instance.AddScore(lastAttackerIndex, 3);
            lastAttackerIndex = -1;
        }

        if (currentScore <= 0) return;

        //スコアを減らす（これは「落ちた瞬間」に即座に実行）
        int penalty = Mathf.Max(1, currentScore / 2);
        GameManager_M.Instance.AddScore(playerIndex, -penalty);


        _lastHitTag = "";
    }

    public void DropItemsAtRespawn(int currentPenalty)
    {
        // リスポーン地点にポイントをばらまく
        StartCoroutine(DropItemsWithDelay(currentPenalty));
    }

    // アイテムを時間差で出すためのコルーチン
    private IEnumerator DropItemsWithDelay(int penalty)
    {
        // スポーンしてすぐ出す
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < penalty; i++)
        {
            if (_itemPrefab == null) break;

            // キャラクターの頭より上
            Vector3 spawnPos = transform.position + Vector3.up * 1.5f;

            GameObject itemObj = Instantiate(_itemPrefab, spawnPos, Quaternion.identity);
            ScoreItem itemScript = itemObj.GetComponent<ScoreItem>();

            if (itemScript != null)
            {
                Vector3 randomDir = new Vector3(
                    UnityEngine.Random.Range(-1f, 1f),
                    1.5f,
                    UnityEngine.Random.Range(-1f, 1f)
                ).normalized;
                itemScript.Launch(randomDir, 5f);
            }
        }
    }
}