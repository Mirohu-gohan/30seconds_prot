using UnityEngine;
using System;
using UnityEngine.UI;

public class SurvivalMode : IGameMode
{
    private float roundTimeLimit = 20f;
    private float currentTime;
    private Text timerText;

    public SurvivalMode(Text uiText)
    {
        timerText = uiText;
    }

    public void OnEnter()
    {
        currentTime = roundTimeLimit;
        Time.timeScale = 1f;
        Debug.Log("Survival Mode: 開始 (ラウンド制、落下無効)");
        // 落下による排除は行わないため、PlayerHealthにその旨を伝達する設定があればここでON
    }

    public void OnUpdate()
    {
        // 落下による排除はしないため、RunBoundaryCheck()は呼び出さない

        // 人数チェックは常に行う (CheckWinConditionForMode()はOnPlayerEliminatedから呼ばれる)

        currentTime -= Time.deltaTime;

        // UI更新ロジック (省略) ...

        if (currentTime <= 0)
        {
            // 時間切れ -> GameManagerにサドンデス判定を依頼
            GameManager_M.Instance.TimeExpiredForSurvival();
        }
    }

    public void OnExit() { /* ... */ }
}
