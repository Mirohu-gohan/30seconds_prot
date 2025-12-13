using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScoreMode : IGameMode
{
    private float timeLimit = 300f;
    private int goalScore = 10;
    private float currentTime;
    private Text timerText;

    public static Dictionary<GameObject, int> PlayerScores { get; private set; } = new Dictionary<GameObject, int>();
    public static GameObject LastAttacker { get; private set; } // 落下させたプレイヤーの攻撃者

    public ScoreMode(Text uiText)
    {
        timerText = uiText;
    }

    public void OnEnter()
    {
        currentTime = timeLimit;
        PlayerScores.Clear();
        // ★ GameManagerのactivePlayersリストからスコアを初期化するのが理想 ★

        Time.timeScale = 1f;
        Debug.Log("Score Mode: 開始");
    }

    public void OnUpdate()
    {
        // 落下判定は有効
        GameManager_M.Instance.RunBoundaryCheck();

        // ... (時間管理とUI更新ロジックは省略) ...

        if (currentTime <= 0)
        {
            GameManager_M.Instance.ChangeMode(new GameOverMode());
            return;
        }

        // スコア目標チェック (省略 - 最高得点者判定ロジックが必要)
    }

    public void OnExit() { /* ... */ }

    // --- スコア処理メソッド ---

    // ★ 相手の攻撃による場外落下時に呼ばれる（攻撃者に+1点）
    public static void AddScoreForKnockout(GameObject attacker)
    {
        if (PlayerScores.ContainsKey(attacker))
        {
            PlayerScores[attacker] += 1;
            Debug.Log($"【Knockout】{attacker.name}: +1点. 現在スコア: {PlayerScores[attacker]}");
        }
    }

    // ★ 自滅・地形破壊による落下時に呼ばれる（自身に-1点）
    public static void SubtractScoreForSelfDestruct(GameObject self)
    {
        if (PlayerScores.ContainsKey(self))
        {
            PlayerScores[self] -= 1;
            Debug.Log($"【自滅】{self.name}: -1点. 現在スコア: {PlayerScores[self]}");
        }
    }
}