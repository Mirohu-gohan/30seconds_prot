using UnityEngine;
using UnityEngine.UI;
using static GameMode;

public class SurvivalMode : IGameMode
{
    private float currentTime;
    private Text timerText;
    public bool isTimerActive=false;
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

        // 2. 秒数を画面に出す（表示文字列が変わった時だけ更新）
        if (timerText != null)
        {
            string newText = Mathf.Max(0, currentTime).ToString("F1");
            if (timerText.text != newText)
                timerText.text = newText;
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