using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager_M : MonoBehaviour
{
    public static GameManager_M Instance { get; private set; }

    [Header("UI設定")]
    public Text timerTextUI;
    [Header("境界設定")]
    [SerializeField] private float deathYCoordinate = -10.0f;

    private IGameMode _currentMode;
    public enum Mode { None, TimeLimit, Survival, Score, SuddenDeath, GameOver }
    public Mode CurrentModeState { get; private set; } = Mode.None;

    // 監視対象プレイヤーリスト
    private List<GameObject> activePlayers = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // シーンをまたぐ場合
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 初期プレイヤー数を取得し、初期モードを起動
        ChangeMode(new SurvivalMode(timerTextUI)); // 初期はサバイバルモードで開始
    }

    void Update()
    {
        _currentMode?.OnUpdate();
    }

    // --- モード管理（ステートパターン） ---
    public void ChangeMode(IGameMode newMode)
    {
        _currentMode?.OnExit();
        _currentMode = newMode;
        _currentMode.OnEnter();

        // Enumを更新
        if (newMode is SurvivalMode) CurrentModeState = Mode.Survival;
        else if (newMode is SuddenDeathMode) CurrentModeState = Mode.SuddenDeath;
        else if (newMode is ScoreMode) CurrentModeState = Mode.Score;
        else if (newMode is GameOverMode) CurrentModeState = Mode.GameOver;
        else if (newMode is TimeLimitMode) CurrentModeState = Mode.TimeLimit;

        Debug.Log($"モード変更: {CurrentModeState}");
    }

    // --- プレイヤー管理（コアロジック） ---
    public void RegisterPlayer(GameObject newPlayer)
    {
        if (newPlayer != null && !activePlayers.Contains(newPlayer))
        {
            activePlayers.Add(newPlayer);
        }
    }

    // プレイヤーが排除されたときにPlayerHealthから呼ばれる
    public void OnPlayerEliminated()
    {
        // activePlayersリストの整理は、RunBoundaryCheck()や外部破壊時に行う

        // 現在のモードに勝利判定を依頼
        CheckWinConditionForMode();
    }

    // 汎用的な勝利判定メソッド (主にSurvival/SuddenDeath用)
    public void CheckWinConditionForMode()
    {
        // リストをクリーンアップ
        activePlayers.RemoveAll(p => p == null);

        // Survival/SuddenDeathモードのみ、人数が1人以下でゲーム終了
        if (CurrentModeState == Mode.Survival || CurrentModeState == Mode.SuddenDeath)
        {
            if (activePlayers.Count <= 1)
            {
                ChangeMode(new GameOverMode());
            }
        }
        // ScoreModeはScoreMode自身で終了判定を行う
    }

    // Y境界チェックロジック (SurvivalMode, ScoreModeのOnUpdateから呼び出される)
    public void RunBoundaryCheck()
    {
        for (int i = activePlayers.Count - 1; i >= 0; i--)
        {
            GameObject player = activePlayers[i];

            if (player == null) // オブジェクトが外部で破壊された場合
            {
                activePlayers.RemoveAt(i);
                continue;
            }

            if (player.transform.position.y < deathYCoordinate)
            {
                // 落下したプレイヤーのPlayerHealthを通じて処理を続行
                player.GetComponent<PlayerHealth>()?.OnFallOut();
            }
        }
    }

    // --- サバイバルモード専用ロジック ---
    public void TimeExpiredForSurvival()
    {
        activePlayers.RemoveAll(p => p == null);
        if (activePlayers.Count >= 2)
        {
            ChangeMode(new SuddenDeathMode());
        }
        else
        {
            ChangeMode(new GameOverMode());
        }
    }
}