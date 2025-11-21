using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // UI (Text) を使うために追加

public class GameManager : MonoBehaviour
{
    [Header("UI設定")]
    [Tooltip("残り時間を表示するText UI (TimeControllerから利用)")]
    public Text timerTextUI;

    [Header("境界設定")]
    [Tooltip("このY座標を下回るとプレイヤーを破壊します。")]
    [SerializeField]
    private float deathYCoordinate = -10.0f;

    [Header("参照")]
    // TimeControllerは直接参照する（手動でInspectorで設定推奨）
    public TImeController timeController;

    // 現在アクティブなプレイヤーオブジェクトを格納するリスト
    private List<GameObject> activePlayers = new List<GameObject>();
    private bool gameOver = false;
    private bool isGameStarted = false; // 参加フェーズがなくなり、すぐにゲームを開始するフラグ

    void Awake()
    {
        // TimeControllerはInspectorで設定するか、自動で検索
        if (timeController == null)
        {
            timeController = Object.FindFirstObjectByType<TImeController>();
        }
    }

    void Start()
    {
        // 参加受付がないため、ここでゲームをすぐに開始
        StartGameLogic();

        // TimeControllerの時間をスタート
        if (timeController != null)
        {
            timeController.StartGame();
        }
    }

    void StartGameLogic()
    {
        // シーンに存在するプレイヤーを登録
        RefreshPlayerList();

        if (activePlayers.Count == 0)
        {
            Debug.LogError("シーンに 'Player' タグのオブジェクトが見つかりません。ゲームを開始できません。");
            return;
        }

        isGameStarted = true;
        Debug.Log("ゲーム開始！監視対象プレイヤー数: " + activePlayers.Count);
    }

    void Update()
    {
        if (!isGameStarted || gameOver) return;

        // ゲームロジック
        CheckYBoundary();
        CheckWinCondition();
    }

    // Y座標の境界チェックと破壊
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

        // マルチプレイの制御がないため、ここでは何もする必要がありません。
        // CheckWinConditionがすぐに呼ばれます。
    }

    void CheckWinCondition()
    {
        // プレイヤーが一人以下になったらゲーム終了
        if (activePlayers.Count <= 1)
        {
            gameOver = true;
            string winnerName = (activePlayers.Count == 1) ? activePlayers[0].name : "None (全員敗退)";

            HandleWin(winnerName);
        }
    }

    void HandleWin(string winnerName)
    {
        // TimeControllerを通じてゲームをポーズし、UIに結果を表示
        if (timeController != null)
        {
            timeController.TimeUp(winnerName);
        }
        else
        {
            Time.timeScale = 0f;
        }
    }

    // シーンに存在するすべてのプレイヤー（'Player'タグ）をリストに格納
    void RefreshPlayerList()
    {
        activePlayers.Clear();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            activePlayers.Add(player);
        }
    }
}