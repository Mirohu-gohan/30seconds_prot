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

    public void OnEnter() 
    {
        if (GameManager_M.Instance != null&&GameManager_M.Instance.roundTextUI !=null)
        {
            int round = GameManager_M.CurrentRound;

            GameManager_M.Instance.roundTextUI.text = "Round" + round;
            GameManager_M.Instance.roundTextUI.gameObject.SetActive(true);

            //GameManeger_Mに処理をお願い
            /*GameManager_M.Instance.HideUI(2.0f);*/
        }
    }

    public void OnUpdate()
    {
        // タイマー更新
        currentTime -= Time.deltaTime;
        if (timerText != null)
            timerText.text = Mathf.Max(0, currentTime).ToString("F1");
            timerText.gameObject.SetActive(true);

        // 1. 時間切れ判定
        if (currentTime <= 0)
        {
            // 引数なし = 引き分けリザルト
            //GameManager_M.Instance.ChangeMode(new GameOverMode(""));
            GameManager_M.Instance.NextRound(true);
        }

        // 2. 生き残り判定（1人になったら終了）
        if (GameManager_M.Instance.GetActivePlayersCount() <= 1)
        {
            //string winner = GameManager_M.Instance.GetWinnerName();
            //GameManager_M.Instance.ChangeMode(new GameOverMode(winner));
            GameManager_M.Instance.NextRound(false);
        }
        Debug.Log("タイマー更新中: " + currentTime); // これがコンソールに出るかチェック
        currentTime -= Time.deltaTime;
    }

    public void OnExit() { }
}