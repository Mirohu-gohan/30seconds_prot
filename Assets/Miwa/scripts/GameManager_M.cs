using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using static GameMode; // コントローラー操作のUI選択に必要

public class GameManager_M : MonoBehaviour
{
    // シングルトンインスタンス
    public static GameManager_M Instance { get; private set; }

    [Header("UI設定")]
    public Text timerTextUI;      // 制限時間を表示するテキスト
    public GameObject resultCanvas; // リロード/戻るボタンが含まれるパネル

    [Header("境界設定")]
    [SerializeField] private float deathYCoordinate = -10.0f;

    // 内部管理用
    private IGameMode _currentMode;
    public enum Mode { Survival, SuddenDeath, GameOver }
    public Mode CurrentModeState { get; private set; }

    private List<GameObject> activePlayers = new List<GameObject>(); // 生存プレイヤーリスト

    void Awake()
    {
        // シングルトンの初期化
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 最初はサバイバルモード（20秒）から開始
        // ※この時点ではプレイヤーはまだスポーン中かもしれないので、リストは空でOK
        ChangeMode(new SurvivalMode(timerTextUI));
    }

    void Update()
    {
        // 現在のモードのロジック（タイマー減算など）を実行
        _currentMode?.OnUpdate();
    }

    // --- モード管理（ステートパターン） ---
    public void ChangeMode(IGameMode newMode)
    {
        _currentMode?.OnExit();
        _currentMode = newMode;
        _currentMode.OnEnter();

        // デバッグおよび判定用のEnum更新
        if (newMode is SurvivalMode) CurrentModeState = Mode.Survival;
        else if (newMode is SuddenDeathMode) CurrentModeState = Mode.SuddenDeath;
        else if (newMode is GameOverMode) CurrentModeState = Mode.GameOver;

        Debug.Log($"モード変更: {CurrentModeState}");
    }

    // --- プレイヤー管理 ---

    // 各プレイヤーがスポーン時に自分を登録する
    public void RegisterPlayer(GameObject newPlayer)
    {
        if (newPlayer != null && !activePlayers.Contains(newPlayer))
        {
            activePlayers.Add(newPlayer);
        }
    }

    // プレイヤーが死亡・脱落した時に呼ばれる
    public void OnPlayerEliminated()
    {
        // リストから消えたプレイヤー（Destroyされたもの）を掃除
        activePlayers.RemoveAll(p => p == null);

        // 1人以下になったらゲーム終了判定
        if (activePlayers.Count <= 1)
        {
            ChangeMode(new GameOver());
        }
    }

    // サバイバルモードでの時間切れ処理
    public void TimeExpiredForSurvival()
    {
        activePlayers.RemoveAll(p => p == null);

        if (activePlayers.Count >= 2)
        {
            // 2人以上残っていたらサドンデスへ
            ChangeMode(new SuddenDeathMode());
        }
        else
        {
            // 1人以下ならそのまま終了
            ChangeMode(new GameOver());
        }
    }

    // モードから生存プレイヤーを確認するための窓口
    public List<GameObject> GetActivePlayers()
    {
        activePlayers.RemoveAll(p => p == null);
        return activePlayers;
    }

    // --- UI/ボタン連携ロジック ---

    // GameOverModeから呼び出され、以前のUIボタンを表示する
    public void ShowResultUI()
    {
        if (resultCanvas != null)
        {
            resultCanvas.SetActive(true);

            // コントローラー操作対応：最初のボタンを自動で選択状態にする
            Button firstButton = resultCanvas.GetComponentInChildren<Button>();
            if (firstButton != null)
            {
                EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
            }
        }
    }

    // インスペクターのボタン（Reload）から呼び出す
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // インスペクターのボタン（Back To Join）から呼び出す
    public void BackToJoinScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("JoinScene"); // ここにJOINシーンの正確な名前を入れる
    }
}