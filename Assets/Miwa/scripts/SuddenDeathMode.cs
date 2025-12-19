using UnityEngine;
using static GameMode;

public class SuddenDeathMode : IGameMode
{
    public void OnEnter()
    {
        // モードに入ったら倍率をサドンデス用に変更
        if (GameManager_M.Instance != null)
        {
            GameManager_M.Instance.currentKnockbackMultiplier = GameManager_M.Instance.suddenDeathKnockbackMultiplier;
        }
        Debug.Log("サドンデス：ふっとばし力アップ！");
    }
    public void OnUpdate() { }
    public void OnExit() { ApplyPowerUp(false); }

    private void ApplyPowerUp(bool enable)
    {
        foreach (var player in GameManager_M.Instance.GetActivePlayers())
        {
            if (player == null) continue;
            var meter = player.GetComponent<PlayerController>();
            if (meter != null)
            {
                // ここはPowerMeter内の実際の変数名に合わせてください
                meter.curentknockbackForce = enable ? 5000f : 500f;
            }
        }
    }
}