using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("参加設定")]
    [Tooltip("プレイヤーの参加受付時間（秒）")]
    [SerializeField]
    private float participationTimeLimit = 10f;

    private float currentParticipationTime;
    private bool isJoiningPhase = true;

    [Header("UI設定")]
    public Text timerTextUI;

    [Header("境界設定")]
    [SerializeField]
    private float deathYCoordinate = -10.0f;

    [Header("参照")]
    public TImeController timeController;
    private PlayerInputManager inputManager;
    private List<GameObject> activePlayers = new List<GameObject>();
    private bool gameOver = false;

    void Awake()
    {
        inputManager = inputManager = Object.FindFirstObjectByType<PlayerInputManager>();
        if (inputManager == null) Debug.LogError("PlayerInputManager が見つかりません。");

        if (timeController == null) timeController = timeController = Object.FindFirstObjectByType<TImeController>();
    }

    void Start()
    {
        currentParticipationTime = participationTimeLimit;
        isJoiningPhase = true;

        // 参加受付を開始したい場合
        if (inputManager != null)
        {
            inputManager.EnableJoining();
        }

        // 参加受付を停止したい場合（EndJoiningPhase()など）
        if (inputManager != null)
        {
            inputManager.DisableJoining();
        }

        // TimeControllerがカウントダウンしないよう、開始を停止しておく
        if (timeController != null) timeController.isGameStarted = false;

        RefreshPlayerList();
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (!isJoiningPhase) return; // 参加フェーズ外なら無視

        if (playerInput.gameObject.CompareTag("Player"))
        {
            activePlayers.Add(playerInput.gameObject);
        }

        // 最大人数に達したら、時間切れ前でも参加を締め切る
        if (activePlayers.Count >= inputManager.maxPlayerCount)
        {
            EndJoiningPhase();
        }
    }

    void Update()
    {
        if (isJoiningPhase)
        {
            HandleJoiningTimer();
            return; // 参加フェーズ中はゲームロジックをスキップ
        }

        if (gameOver) return;

        // ゲームロジック
        CheckYBoundary();
        CheckWinCondition();
    }

    void HandleJoiningTimer()
    {
        if (currentParticipationTime > 0)
        {
            currentParticipationTime -= Time.deltaTime;

            if (timerTextUI != null)
            {
                timerTextUI.text = "参加受付中: " + Mathf.CeilToInt(currentParticipationTime).ToString() + "秒";
            }
        }

        if (currentParticipationTime <= 0)
        {
            EndJoiningPhase();
        }
    }

    void EndJoiningPhase()
    {
        if (!isJoiningPhase) return;

        isJoiningPhase = false;

        // 参加受付を完全に終了（途中参加禁止）
        if (inputManager != null) inputManager.DisableJoining();

        if (timerTextUI != null) timerTextUI.text = "ゲーム開始！";

        if (activePlayers.Count == 0)
        {
            Debug.LogWarning("プレイヤーが一人も参加しませんでした。");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Time.timeScale = 1f;
            return;
        }

        // TimeControllerにゲーム開始の合図を送り、制限時間をスタートさせる
        if (timeController != null)
        {
            timeController.StartGame();
        }
        Debug.Log("参加受付終了。プレイヤー数: " + activePlayers.Count);
    }

    // Y座標の境界チェックと破壊 (GameManager内で実行)
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

        // 念のため、破壊後も参加禁止を維持
        if (inputManager != null) inputManager.DisableJoining();
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
        if (inputManager != null) inputManager.DisableJoining();

        if (timeController != null)
        {
            // 勝利者名を与えてTimeUpを強制実行
            timeController.TimeUp(winnerName);
        }
        else
        {
            Time.timeScale = 0f;
        }
    }

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