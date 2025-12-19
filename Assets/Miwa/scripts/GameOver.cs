using UnityEngine;
using UnityEngine.SceneManagement;
using static GameMode;

public class GameOver : IGameMode // ©‚±‚±‚ªd—v
{
    public void OnEnter()
    {
        Time.timeScale = 0f;
        GameManager_M.Instance.ShowResultUI("None");
    }

    public void OnUpdate() { }

    public void OnExit() => Time.timeScale = 1f;
}