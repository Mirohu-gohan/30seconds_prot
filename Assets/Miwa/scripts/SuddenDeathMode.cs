using UnityEngine;
using static GameMode;

public class SuddenDeathMode : IGameMode // ©•K‚¸u: IGameModev‚ğ‚Â‚¯‚é
{
    public void OnEnter() { ApplyPowerUp(true); }
    public void OnUpdate() { }
    public void OnExit() { ApplyPowerUp(false); }

    private void ApplyPowerUp(bool enable)
    {
        foreach (var player in GameManager_M.Instance.GetActivePlayers())
        {
            if (player == null) continue;
            var meter = player.GetComponent<PowerMeter>();
            if (meter != null)
            {
                // ‚±‚±‚ÍPowerMeter“à‚ÌÀÛ‚Ì•Ï”–¼‚É‡‚í‚¹‚Ä‚­‚¾‚³‚¢
                //meter.knockbackForce = enable ? 5000f : 500f;
            }
        }
    }
}