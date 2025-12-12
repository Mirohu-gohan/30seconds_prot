using UnityEngine;

public class GameOverMode : IGameMode
{
    public void OnEnter()
    {
        // ★ゲーム全体の動きを停止させる
        Time.timeScale = 0f;

        Debug.Log("GAME OVER! すべての動きをポーズしました。");

        // リザルト画面のUIを表示する処理（例: FindObjectOfType<ResultUI>().ShowResult();）
    }

    public void OnUpdate()
    {
        // 時間が止まっているため、Time.deltaTimeに依存しない入力処理を行う
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("リトライ処理開始");
            // Time.timeScaleを1に戻す
            Time.timeScale = 1f;
            // シーンの再ロードや、初期モードへの切り替え処理を行う
            // UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            // または GameManager_M.Instance.ChangeMode(new TimeLimitMode(GameManager_M.Instance.timerTextUI, 60f));
        }
    }

    public void OnExit()
    {
        // ...
    }
}
