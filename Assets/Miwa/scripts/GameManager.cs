using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI設定")]
    public Text timerTextUI;

    [Header("境界設定")]
    [SerializeField]
    private float deathYCoordinate = -10.0f;

    [Header("参照")]
    public TImeController timeController;

    private List<GameObject> activePlayers = new List<GameObject>(); // 監視リスト
    private bool gameOver = false;
    private bool isGameStarted = false;

    void Awake()
    {
        if (timeController == null)
        {
            timeController = Object.FindFirstObjectByType<TImeController>();
        }
    }

    void Start()
    {
        StartGameLogic();
    }

    void StartGameLogic()
    {
        // プレイヤーはスポーン時に自動でリストに登録されるため、ここではリストチェックやスキャンは行わない。

        isGameStarted = true;

        // TimeControllerの時間をスタート
        if (timeController != null)
        {
            timeController.StartGame();
        }
        Debug.Log("ゲーム開始！プレイヤーの参加を待っています。");
    }

    // ★★★ 追加: プレイヤーがスポーン時に自分を登録するためのメソッド ★★★
    /// <summary>
    /// 新しくスポーンされたプレイヤーを監視リストに追加する
    /// </summary>
    public void RegisterPlayer(GameObject newPlayer)
    {
        if (newPlayer != null && !activePlayers.Contains(newPlayer))
        {
            activePlayers.Add(newPlayer);
            Debug.Log("プレイヤー登録完了。現在監視プレイヤー数: " + activePlayers.Count);
        }
    }

    // Update()、CheckYBoundary()、HandleWin() は基本的に変更なし。
    void Update()
    {
        if (!isGameStarted || gameOver) return;

        CheckYBoundary();
        CheckWinCondition();
    }

    void CheckYBoundary()
    {
        for (int i = activePlayers.Count - 1; i >= 0; i--)
        {
            GameObject player = activePlayers[i];

            if (player == null)
            {
                activePlayers.RemoveAt(i);
                continue;
            }

            if (player.transform.position.y < deathYCoordinate)
            {
                DestroyPlayer(player, i);
            }
        }
    }

    void DestroyPlayer(GameObject playerToDestroy, int indexInList)
    {
        Debug.Log(playerToDestroy.name + " がY座標境界を下回り、破壊されました。");
        Destroy(playerToDestroy);
        activePlayers.RemoveAt(indexInList);
    }

    void CheckWinCondition()
    {
        // プレイヤーが一人以下になったらゲーム終了
        if (activePlayers.Count <= 1 && activePlayers.Count > 0) // 1人残って勝利
        {
            gameOver = true;
            string winnerName = activePlayers[0].name;
            HandleWin(winnerName);
        }
        else if (activePlayers.Count == 0 && isGameStarted) // 全員敗退
        {
            gameOver = true;
            HandleWin("None (全員敗退)");
        }
    }

    void HandleWin(string winnerName)
    {
        if (timeController != null)
        {
            timeController.TimeUp(winnerName);
        }
        else
        {
            Time.timeScale = 0f;
        }
    }

    // RefreshPlayerList() は完全に削除（動的スポーンのため不要）
    // void RefreshPlayerList() { ... }
}