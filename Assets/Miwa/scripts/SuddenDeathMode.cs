using UnityEngine;
using static GameMode;

public class SuddenDeathMode : IGameMode // ←ここが重要
{
    public void OnEnter()
    {
        Time.timeScale = 1f;
        ApplyPowerUp(true);
    }

    public void OnUpdate() { }

    public void OnExit() => ApplyPowerUp(false);

    private void ApplyPowerUp(bool enable)
    {
        foreach (var player in GameManager_M.Instance.GetActivePlayers())
        {
            if (player == null) continue;

            // PowerMeterを取得
            var meter = player.GetComponent<PowerMeter>();
            if (meter != null)
            {
                // エラー回避：もしPowerMeterに変数がない場合は
                // 下の行がエラーになるので、その場合はPowerMeter側を修正してください
                meter.knockbackForce = enable ? 5000f : 500f;
            }
        }
    }
}