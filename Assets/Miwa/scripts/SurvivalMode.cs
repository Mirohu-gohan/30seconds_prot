using UnityEngine;
using UnityEngine.UI;

public class SurvivalMode : IGameMode
{
    // TimeControllerの代わりに、モード内で時間を管理
    private float timeLimit;
    private float currentTime;
    private Text timerText;

    public SurvivalMode(Text uiText, float limit)
    {
        timerText = uiText;
        timeLimit = limit;
    }

    public void OnEnter()
    {
        currentTime = timeLimit;
        // ... UIやアセットのロード ...
        Time.timeScale = 1f;
    }

    public void OnUpdate()
    {
        // Y境界チェックをGameManagerに要求
        GameManager_M.Instance.RunBoundaryCheck();

        // 時間の管理
        currentTime -= Time.deltaTime;
        // UI更新ロジック (TimeSpan使用) ...

        if (currentTime <= 0)
        {
            // 時間切れ -> GameManagerにサドンデス判定を依頼
            GameManager_M.Instance.TimeExpiredForSurvival();
        }
    }

    // プレイヤーが減った時にGameManagerから呼ばれる判定メソッド
    public void CheckWinCondition(int currentCount)
    {
        // 1人残って勝利
        if (currentCount == 1)
        {
            GameManager_M.Instance.ChangeMode(new GameOverMode());
        }
        // 全員敗退
        else if (currentCount == 0)
        {
            GameManager_M.Instance.ChangeMode(new GameOverMode());
        }
    }

    public void OnExit()
    {
        // ...
    }
}
