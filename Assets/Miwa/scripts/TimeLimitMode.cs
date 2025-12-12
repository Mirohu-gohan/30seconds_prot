using System;
using UnityEngine;
using UnityEngine.UI;

public class TimeLimitMode : IGameMode
{
    private float timeLimit; // 制限時間
    private float currentTime;
    private Text timerText;

    // コンストラクタでUI参照と制限時間を受け取る
    public TimeLimitMode(Text uiText, float limit)
    {
        timerText = uiText;
        timeLimit = limit;
    }

    public void OnEnter()
    {
        currentTime = timeLimit;
        Time.timeScale = 1f;
        Debug.Log($"TimeLimit Mode: {timeLimit}秒制限で開始");
        // TimeLimit専用アセットのロード（あれば）
    }

    public void OnUpdate()
    {
        // Y境界チェック（TimeLimitModeでもプレイヤーが落ちたらカウント対象外とする場合）
        GameManager_M.Instance.RunBoundaryCheck();

        // 時間の管理
        currentTime -= Time.deltaTime;

        // UI更新
        var span = new TimeSpan(0, 0, (int)Mathf.Max(0, currentTime));
        timerText.text = span.ToString(@"mm\:ss");

        if (currentTime <= 0)
        {
            // 時間切れ -> ゲーム終了
            Debug.Log("TimeLimit Mode: 時間切れでゲーム終了");
            GameManager_M.Instance.ChangeMode(new GameOverMode());
        }
    }

    public void OnExit()
    {
        // ... クリーンアップ処理 ...
    }
}
