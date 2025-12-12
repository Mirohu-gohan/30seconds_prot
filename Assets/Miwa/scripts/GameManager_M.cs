using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// IGameModeインターフェースと各モードクラスが事前に定義されている必要があります！

public class GameManager_M : MonoBehaviour
{
    // ★ ステートパターンに必要な変数を追加
    private IGameMode _currentMode;
    public enum Mode { None, TimeLimit, Survival, Score, SuddenDeath, GameOver }
    public Mode CurrentModeState { get; private set; } = Mode.None;

    // ★ Singletonとして機能させる
    public static GameManager_M Instance { get; private set; }

    [Header("UI設定")]
    // UIの参照はGameManagerに持たせ、各モードクラスに渡す形にする
    public Text timerTextUI;

    [Header("境界設定")]
    // 境界チェックは生存系モードのUpdateで呼び出す
    [SerializeField] private float deathYCoordinate = -10.0f;

    // ★ TImeControllerへの直接参照は削除する（TimeControllerの役割はモードクラスが引き継ぐため）

    private List<GameObject> activePlayers = new List<GameObject>();
    // private bool gameOver = false; // 終了判定はCurrentModeStateで管理する
    private bool isGameStarted = false;

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
        StartGameLogic();
    }

    void StartGameLogic()
    {
        isGameStarted = true;
        // ★初期モードを切り替え（例: サバイバルモードから開始）
        ChangeMode(new SurvivalMode(timerTextUI, 180f)); // 3分サバイバルで開始
        Debug.Log("ゲーム開始！");
    }

    void Update()
    {
        // ★現在のモードのUpdateを呼び出す
        _currentMode?.OnUpdate();
    }

    // --- モード管理（ステートパターン） ---
    public void ChangeMode(IGameMode newMode)
    {
        _currentMode?.OnExit();
        _currentMode = newMode;
        _currentMode.OnEnter();

        // Enumの更新ロジックはGameManagerに残す
        if (newMode is SurvivalMode) CurrentModeState = Mode.Survival;
        else if (newMode is SuddenDeathMode) CurrentModeState = Mode.SuddenDeath;
        else if (newMode is GameOverMode) CurrentModeState = Mode.GameOver;
        // ... 他のモードも追加 ...

        Debug.Log($"モード変更: {CurrentModeState}");
    }

    // --- プレイヤー管理（コアロジック） ---
    public void RegisterPlayer(GameObject newPlayer)
    {
        if (newPlayer != null && !activePlayers.Contains(newPlayer))
        {
            activePlayers.Add(newPlayer);
            Debug.Log("プレイヤー登録完了。現在監視プレイヤー数: " + activePlayers.Count);
        }
    }

    // Y境界チェックはモードによって必要/不要が変わるため、外部から呼び出す形に修正
    public void CheckAndDestroyBoundary(GameObject player, int indexInList)
    {
        if (player.transform.position.y < deathYCoordinate)
        {
            DestroyPlayer(player, indexInList);
        }
    }

    private void DestroyPlayer(GameObject playerToDestroy, int indexInList)
    {
        Debug.Log(playerToDestroy.name + " がY座標境界を下回り、破壊されました。");
        Destroy(playerToDestroy);
        // リストからの削除は、PlayerEliminated()を呼び出す前に別メソッドとして処理するのが安全
        activePlayers.RemoveAt(indexInList);

        // ★プレイヤーが減ったことを現在のモードに通知
        OnPlayerEliminated();
    }

    // プレイヤー排除時の通知メソッド (CheckBoundary/PlayerHealthから呼ばれる)
    public void OnPlayerEliminated()
    {
        // プレイヤーが減ったことを現在のモードに通知し、勝利判定を任せる
        if (_currentMode is SurvivalMode survivalMode)
        {
            survivalMode.CheckWinCondition(activePlayers.Count);
        }
        // ... ScoreModeやTimeLimitModeの場合は処理しないか、別の判定を行う ...
    }

    // Y境界チェックロジックをGameManagerに残し、モード側で呼び出す形にする
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

            CheckAndDestroyBoundary(player, i);
        }
    }

    // ★★★ TimeExpiredForSurvival() はそのままGameManagerに残し、
    // SurvivalModeから呼び出させることで、サドンデス判定を一元化する。 ★★★
    public void TimeExpiredForSurvival()
    {
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