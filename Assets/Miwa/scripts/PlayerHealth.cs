using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("ステータス")]
    public float currentHealth = 100f;

    void Start()
    {
        if (GameManager_M.Instance != null)
        {
            GameManager_M.Instance.RegisterPlayer(gameObject);
        }
    }

    // 外部（サドンデスでのダメージなど）からHPを減らす時に呼ぶ
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 落下した時にGameManagerのRunBoundaryCheckから呼ばれる
    public void OnFallOut()
    {
        // 落下 = 即脱落として処理します
        Die();
    }

    // 死亡・脱落処理の共通化
    private void Die()
    {
        if (GameManager_M.Instance != null)
        {
            // GameManagerに「一人が脱落した」ことを通知して勝敗判定を行わせる
            GameManager_M.Instance.OnPlayerEliminated();
        }

        // 自身を消滅させる
        Destroy(gameObject);
    }
}