using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameOverMode : IGameMode
{
    public void OnEnter()
    {
        // ★ 1. ゲーム全体の動きを完全に停止させる
        Time.timeScale = 0f;

        Debug.Log("GAME OVER MODE: 突入。ゲームポーズ状態です。");

        // ★ 2. 勝者の判定と結果の表示（スコアモードの場合）
        string winnerName = DetermineWinner();

        // UI表示用の外部処理を呼び出す（例: リザルトパネルの有効化）
        // FindObjectOfType<ResultUI>().ShowResult(winnerName); 
    }

    public void OnUpdate()
    {
        // 時間が止まっているため、Time.deltaTimeに依存しない入力処理（キーボード/コントローラー入力）を行う

        // 【リトライ処理】スペースキーが押されたら
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("リトライ入力検知。ゲームをリセットします。");

            // Time.timeScaleを元に戻す
            Time.timeScale = 1f;

            // シーンを再ロードするか、初期モードに切り替える
            // 簡潔な方法：現在のシーンを再ロード
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            // より洗練された方法：GameManagerをリセットし、初期モードへ移行
            // GameManager_M.Instance.ChangeMode(new TimeLimitMode(GameManager_M.Instance.timerTextUI, 60f));
        }

        // 【メニューへ戻る処理】
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //     // Time.timeScale = 1f; 
        //     // SceneManager.LoadScene("MainMenuScene"); 
        // }
    }

    public void OnExit()
    {
        // リザルトUIを非表示にする
        // FindObjectOfType<ResultUI>().HideResult();
    }

    // 勝者を決定するロジック
    private string DetermineWinner()
    {
        // スコアモードの場合：最高得点者を決定
        if (GameManager_M.Instance.CurrentModeState == GameManager_M.Mode.Score)
        {
            if (ScoreMode.PlayerScores.Count == 0) return "全員敗退";

            // スコアが最も高いプレイヤーを探す
            var winnerEntry = ScoreMode.PlayerScores.OrderByDescending(kv => kv.Value).FirstOrDefault();

            if (winnerEntry.Key != null)
            {
                return winnerEntry.Key.name;
            }
        }

        // サバイバルモードの場合：最後に残ったプレイヤーを決定
        // (GameManager_MのactivePlayersリストから最後に残った1名を取得)
        if (GameManager_M.Instance.CurrentModeState == GameManager_M.Mode.Survival ||
            GameManager_M.Instance.CurrentModeState == GameManager_M.Mode.SuddenDeath)
        {
            var activePlayers = FindObjectsOfType<PlayerHealth>().Select(ph => ph.gameObject).ToList();
            if (activePlayers.Count == 1)
            {
                return activePlayers[0].name;
            }
        }

        return "勝者なし (引き分け)";
    }
}