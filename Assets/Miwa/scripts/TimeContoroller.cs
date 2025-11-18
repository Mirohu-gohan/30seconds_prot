using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TImeController : MonoBehaviour
{
    public Text Timetext;

    [SerializeField]
    private float time = 60.0f; // 時間制限

    public bool isTimeUp = false;
    public bool isGameStarted = false; // ★追加: ゲーム開始フラグ

    public GameObject timeUpPanel;


    void Start()
    {
        Time.timeScale = 1f;
        isTimeUp = false;
        isGameStarted = false; // 初期状態では停止
        if (timeUpPanel != null)
        {
            timeUpPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (isTimeUp || !isGameStarted) // ★修正: 開始していない場合は処理を抜ける
        {
            return;
        }

        if (time > 0)
        {
            time -= Time.deltaTime;
            Timetext.text = time.ToString("F1");
        }
        else
        {
            time = 0;
            TimeUp(); // 時間切れによる終了
        }
    }

    // GameManagerから呼ばれ、ゲームをスタートさせる
    public void StartGame()
    {
        isGameStarted = true;
    }

    // GameManagerや時間切れでゲームを終了させる
    public void TimeUp(string winnerName = null)
    {
        if (isTimeUp) return; // 多重実行防止

        isTimeUp = true;

        if (!string.IsNullOrEmpty(winnerName) && winnerName != "None (全員敗退)")
        {
            Timetext.text = winnerName + " の勝利！";
        }
        else if (time <= 0)
        {
            Timetext.text = "崩壊！！！";
        }
        else
        {
            Timetext.text = "ゲーム終了！";
        }

        // ゲームポーズ
        Time.timeScale = 0f;

        if (timeUpPanel != null)
        {
            timeUpPanel.SetActive(true);
        }
    }

    // リセットボタンに割り当てる
    public void ResetGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}