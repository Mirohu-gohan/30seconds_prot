using UnityEngine;
using UnityEngine.UI;
using static GameMode;

public class ScoreMode : IGameMode
{
    private float _timer;
    private bool _isActive;
    private Text _timerTextUI;
    private float _initialTime; // モード開始時の時間を保持

    public ScoreMode(Text timerTextUI, float timeLimit)
    {
        this._timerTextUI = timerTextUI;
        this._initialTime = timeLimit; // GameManagerから渡されたスコアモード専用時間を保持
    }

    public void OnEnter()
    {
        _isActive = true;
        _timer = _initialTime; // 開始時にタイマーをセット

        // ラウンド開始時に全員のスコアを0にする
        for (int i = 0; i < 4; i++)
        {
            if (i < GameManager_M.currentScores.Length)
                GameManager_M.currentScores[i] = 0;
        }
    }

    public void OnUpdate()
    {
        if (!_isActive) return;

        _timer -= Time.deltaTime;

        if (_timerTextUI != null)
        {
            _timerTextUI.text = Mathf.CeilToInt(_timer).ToString();
        }

        if (_timer <= 0)
        {
            _isActive = false;
            // 時間切れ：trueを渡してリザルト（勝敗判定）へ
            GameManager_M.Instance.NextRound(true);
        }
    }

    public void OnExit()
    {
        _isActive = false;
    }
}