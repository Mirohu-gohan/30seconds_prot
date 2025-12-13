using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // 仮のHP
    public float currentHealth = 100f;

    void Start()
    {
        // スポーン時に必ずGameManagerに登録
        GameManager_M.Instance.RegisterPlayer(gameObject);
    }

    // 外部（SuddenDeathModeなど）からダメージを受け付ける
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die(true); // HPによる死亡
        }
    }

    // 場外に落下したときにGameManagerから呼ばれるメソッド
    public void OnFallOut()
    {
        if (GameManager_M.Instance.CurrentModeState == GameManager_M.Mode.Score)
        {
            // ScoreModeの場合、自滅/Knockout判定を行う

            // 誰かから攻撃を受けた直後ならKnockoutと判定
            // (LastAttackerの判定ロジックが別途必要)

            // 例: 攻撃者がいれば加算し、いなければ減算
            // if (ScoreMode.LastAttacker != null)
            // {
            //     ScoreMode.AddScoreForKnockout(ScoreMode.LastAttacker);
            // }
            // else
            // {
            ScoreMode.SubtractScoreForSelfDestruct(gameObject);
            // }
        }

        // 最終的にオブジェクトを破壊し、人数を減らす
        Die(false);
    }

    private void Die(bool forcedElimination)
    {
        if (GameManager_M.Instance != null)
        {
            GameManager_M.Instance.OnPlayerEliminated();
        }

        Destroy(gameObject);
    }
}