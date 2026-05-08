using UnityEngine;
using UnityEngine.UI;
using static GameMode;

public class SurvivalMode : IGameMode
{
    private float currentTime;
    private Text timerText;
    public bool isTimerActive=false;
    private int _lastDisplayedTenths = int.MinValue;

    public SurvivalMode(Text uiText, float timeLimit)
    {
        timerText = uiText;
        currentTime = timeLimit;
    }

    public void OnEnter()
    {

    }

    public void OnUpdate()
    {
        if (!isTimerActive) return;

        // 1. 時間の計算（GameManagerのUpdateから呼ばれる）
        currentTime -= Time.deltaTime;

        // 2. 0.1秒単位の整数で比較し、変化した時だけ文字列生成して更新
        if (timerText != null)
        {
            int tenths = Mathf.RoundToInt(Mathf.Max(0f, currentTime) * 10f);
            if (tenths != _lastDisplayedTenths)
            {
                _lastDisplayedTenths = tenths;
                timerText.text = $"{tenths / 10}.{tenths % 10}";
            }
        }

        // 3. 時間切れ判定
        if (currentTime <= 0)
        {
            // GameManagerに「時間切れだよ」と伝える
            GameManager_M.Instance.TimeExpiredForSurvival();
        }
    }

    public void OnExit() { }
}