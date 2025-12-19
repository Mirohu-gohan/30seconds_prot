using UnityEngine;
using static GameMode;

public class GameOverMode : IGameMode // ©•K‚¸u: IGameModev‚ğ‚Â‚¯‚é
{
    public void OnEnter()
    {
        Time.timeScale = 0f;
        GameManager_M.Instance.ShowResultUI("None");
    }
    public void OnUpdate() { }
    public void OnExit() { Time.timeScale = 1f; }
}
