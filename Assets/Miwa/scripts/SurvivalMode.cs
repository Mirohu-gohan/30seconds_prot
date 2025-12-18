using UnityEngine;
using UnityEngine.UI;
using static GameMode;

public class SurvivalMode : IGameMode
{
    private float currentTime = 20f;
    private Text timerText;

    public SurvivalMode(Text uiText) => timerText = uiText;

    public void OnEnter()
    {
        Time.timeScale = 1f;
        Debug.Log("Survival Mode: 20秒ラウンド開始");
    }

    public void OnUpdate()
    {
        currentTime -= Time.deltaTime;

        if (timerText != null)
            timerText.text = Mathf.Max(0, currentTime).ToString("F2");

        if (currentTime <= 0)
            GameManager_M.Instance.TimeExpiredForSurvival();
    }

    public void OnExit() { }
}
