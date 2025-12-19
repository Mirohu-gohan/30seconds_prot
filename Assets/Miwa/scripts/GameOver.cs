using static GameMode;
using UnityEngine;

public class GameOver : IGameMode
{
    private string winner;

    // コンストラクタで名前を受け取る
    public GameOver(string winnerName = "Finish")
    {
        winner = winnerName;
    }

    public void OnEnter()
    {
        Time.timeScale = 0f; // 時間停止
        // 勝者の名前をUIに表示
        GameManager_M.Instance.ShowResultUI(winner);
    }

    public void OnUpdate() { }
    public void OnExit() { Time.timeScale = 1f; }
}