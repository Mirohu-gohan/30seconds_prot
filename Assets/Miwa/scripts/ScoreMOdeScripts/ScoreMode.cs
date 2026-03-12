using UnityEngine;
using UnityEngine.UI;
using static GameMode;

public class ScoreMode : IGameMode
{
    private float _timer;
    private bool _isActive;
    private Text _timerTextUI;

    // コンストラクタ（GameManagerから UI と 制限時間 を受け取る）
    public ScoreMode(Text timerTextUI, float timeLimit)
    {
        this._timerTextUI = timerTextUI;
        this._timer = timeLimit;
    }

    public void OnEnter()
    {
        _isActive = true;

        // ラウンド開始時に全員のスコアを0にする
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetAllScores();
        }
    }

    public void OnUpdate()
    {
        if (!_isActive) return;

        _timer -= Time.deltaTime;

        // タイマーUIの更新（切り上げて表示）
        if (_timerTextUI != null)
        {
            _timerTextUI.text = Mathf.CeilToInt(_timer).ToString();
        }

        // 時間切れになったら
        if (_timer <= 0)
        {
            _timer = 0;
            _isActive = false;
            if (_timerTextUI != null) _timerTextUI.text = "0";

            GameManager_M.Instance.NextRound(true);
        }
    }

    public void OnExit()
    {
        _isActive = false;
    }
}