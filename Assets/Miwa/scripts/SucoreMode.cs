using System;
using UnityEngine;
using UnityEngine.UI;

public class ScoreMode : IGameMode
{
    // スコア目標
    private int goalScore = 1000;
    // 時間制限
    private float timeLimit = 300f; // 5分制限

    private float currentTime;
    private Text timerText;

    // スコアは静的変数で管理し、外部から加算できるようにする
    public static int CurrentScore { get; private set; }

    public ScoreMode(Text uiText)
    {
        timerText = uiText;
    }

    public void OnEnter()
    {
        currentTime = timeLimit;
        CurrentScore = 0;
        Time.timeScale = 1f;
        Debug.Log($"Score Mode: 目標 {goalScore}点、制限時間 {timeLimit}秒で開始");
        // ScoreMode専用UI（スコアボード）の有効化
    }

    public void OnUpdate()
    {
        GameManager_M.Instance.RunBoundaryCheck();

        // 1. 時間制限チェック
        currentTime -= Time.deltaTime;
        var span = new TimeSpan(0, 0, (int)Mathf.Max(0, currentTime));
        timerText.text = span.ToString(@"mm\:ss");

        if (currentTime <= 0)
        {
            Debug.Log("Score Mode: 時間切れでゲーム終了");
            GameManager_M.Instance.ChangeMode(new GameOverMode());
            return;
        }

        // 2. スコア目標チェック
        if (CurrentScore >= goalScore)
        {
            Debug.Log("Score Mode: スコア目標達成でゲーム終了");
            GameManager_M.Instance.ChangeMode(new GameOverMode());
        }
    }

    public void OnExit()
    {
        // ...
    }

    // 外部からスコアを加算するためのメソッド（例: 敵を倒した時などに呼び出す）
    public static void AddScore(int amount)
    {
        CurrentScore += amount;
        Debug.Log($"Score Updated: {CurrentScore}");
    }
}