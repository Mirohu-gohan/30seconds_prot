using UnityEngine;

public class SuddenDeathMode : IGameMode
{
    public void OnEnter()
    {
        Time.timeScale = 1f;
        Debug.Log("Sudden Death Mode: 突入！攻撃力がMAXになります。");

        // ★ 攻撃力強化のロジックをここに実装 ★
        EnableOneHitKnockout();
    }

    public void OnUpdate()
    {
        // 落下は引き続きしない
        GameManager_M.Instance.CheckWinConditionForMode();
    }

    public void OnExit()
    {
        ResetOneHitKnockout(); // 攻撃力強化を解除
        Debug.Log("Sudden Death Mode: 終了");
    }

    private void EnableOneHitKnockout()
    {
        // 例: PlayerAttackコンポーネントのKnockbackForceを極端に高い値に設定
        // PlayerAttack[] attacks = FindObjectsOfType<PlayerAttack>();
        // foreach (PlayerAttack pa in attacks)
        // {
        //     pa.KnockbackForce = 5000f; 
        // }
    }

    private void ResetOneHitKnockout()
    {
        // ... 元のKnockbackForceに戻す処理 ...
    }
}