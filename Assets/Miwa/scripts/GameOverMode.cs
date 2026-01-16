using UnityEngine;
using UnityEngine.UI;
using static GameMode;

public class GameOverMode : IGameMode
{
    private string _winnerName;

    // コンストラクタ（名前がなければ "DRAW" とみなす）
    public GameOverMode(string winnerName = "")
    {
        _winnerName = winnerName;
    }

    public void OnEnter()
    {
        // 時間を通常速度に戻す
        Time.timeScale = 1.0f;

        // 1. 勝者の名前を判定
        string winnerMessage = string.IsNullOrEmpty(_winnerName) ? "DRAW GAME" : _winnerName + " WIN!";

        // 2. GameManagerのリザルト表示機能を呼び出す
        if (GameManager_M.Instance != null)
        {
            // 既存のラウンド表示UIは隠す
            /*if (GameManager_M.Instance.roundTextUI != null)
                GameManager_M.Instance.roundTextUI.gameObject.SetActive(false);*/

            // リザルトUIを表示
            GameManager_M.Instance.ShowResultUI(winnerMessage);

            // 全プレイヤーの操作を無効化
            GameManager_M.Instance.SetAllPlayersControl(false);
        }
        // ★重要：スローを解除し、通常速度(1.0)にする
        /*Time.timeScale = 1.0f;

        if (GameManager_M.Instance != null && GameManager_M.Instance.roundTextUI != null)
        {
            // 表示するメッセージの作成
            string resultMessage = string.IsNullOrEmpty(_winnerName) ? "TIME UP / DRAW" : _winnerName + " WIN!";

            // UIをリザルト表示に書き換え
            GameManager_M.Instance.roundTextUI.text = "RESULT\n" + resultMessage;
            GameManager_M.Instance.roundTextUI.color = Color.white;
            GameManager_M.Instance.roundTextUI.gameObject.SetActive(true);
        }

        // プレイヤーを止める
        GameManager_M.Instance.SetAllPlayersControl(false);

        Debug.Log("Result: " + _winnerName);*/
    }

    public void OnUpdate() { }

    public void OnExit()
    {
        if (GameManager_M.Instance.roundTextUI != null)
            GameManager_M.Instance.roundTextUI.gameObject.SetActive(false);
    }
}