using UnityEngine;
using UnityEngine.UI;
using static GameMode;

public class SurvivalMode : IGameMode
{
    private float currentTime;
    private Text timerText;

    public SurvivalMode(Text uiText, float timeLimit)
    {
        timerText = uiText;
        currentTime = timeLimit;
    }

    public void OnEnter() { }

    public void OnUpdate()
    {
        currentTime -= Time.deltaTime;
        if (timerText != null)
            timerText.text = Mathf.Max(0, currentTime).ToString("F1");

        if (currentTime <= 0)
            GameManager_M.Instance.TimeExpiredForSurvival();
    }

    public void OnExit() { }
}