using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using static GameMode;


public class GameManager_M : MonoBehaviour
{
    public static GameManager_M Instance { get; private set; }

    // --- 静的変数（リロード後も保持される） ---
    private static bool _isSuddenDeathNext = false;
    private static List<int> _qualifiedIndices = new List<int>(); // 参加資格のあるインデックス

    [Header("UI設定")]
    public Text timerTextUI;
    public GameObject resultCanvas;

    [Header("ゲーム設定")]
    public float survivalTimeLimit = 20.0f;
    public float deathYCoordinate = -10.0f;

    public enum Mode { Survival, SuddenDeath, GameOver }
    public Mode CurrentModeState;

    private IGameMode _currentMode;
    private List<GameObject> activePlayers = new List<GameObject>();

    public float suddenDeathKnockbackMultiplier = 2.0f; // サドンデス時の強化倍率
    public float currentKnockbackMultiplier = 1.0f;    // 現在の倍率（通常は1.0）

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f;
        if (resultCanvas != null) resultCanvas.SetActive(false);

        // サドンデス開始の判定
        if (_isSuddenDeathNext)
        {
            ChangeMode(new SuddenDeathMode());
            _isSuddenDeathNext = false;
        }
        else
        {
            _qualifiedIndices.Clear();
            ChangeMode(new SurvivalMode(timerTextUI, survivalTimeLimit));
        }
    }

    void Update()
    {
        if (_currentMode != null) _currentMode.OnUpdate();
        CheckPlayersFalling();
    }

    public void RegisterPlayer(GameObject p, int index)
    {
        // サドンデスモード中、参加リストに自分のインデックスがなければ即座に削除
        if (CurrentModeState == Mode.SuddenDeath && !_qualifiedIndices.Contains(index))
        {
            Debug.Log($"プレイヤー {index} は参加資格がないため削除します");
            Destroy(p);
            return;
        }

        if (!activePlayers.Contains(p)) activePlayers.Add(p);
    }

    public void OnPlayerEliminated(GameObject eliminatedPlayer)
    {
        // リストから即座に削除（Destroy完了を待たない）
        if (activePlayers.Contains(eliminatedPlayer))
        {
            activePlayers.Remove(eliminatedPlayer);
        }

        // 念のため null になった要素も掃除
        activePlayers.RemoveAll(p => p == null);

        Debug.Log("残り人数: " + activePlayers.Count);

        // 生き残りが1人になったらゲームオーバー
        if (activePlayers.Count == 1)
        {
            string winnerName = activePlayers[0].name;
            // GameOverMode に勝者の名前を渡して切り替え
            ChangeMode(new GameOver(winnerName));
        }
        // 同時落下で 0 人になった場合（サドンデス中など）
        else if (activePlayers.Count == 0)
        {
            ChangeMode(new GameOver("DRAW"));
        }
    }

    // 2. 落下チェック部分も修正
    private void CheckPlayersFalling()
    {
        if (CurrentModeState == Mode.GameOver) return;

        for (int i = activePlayers.Count - 1; i >= 0; i--)
        {
            GameObject player = activePlayers[i];
            if (player != null && player.transform.position.y < deathYCoordinate)
            {
                // 脱落を通知してから破壊する
                OnPlayerEliminated(player);
                Destroy(player);
            }
        }
    }

    public void TimeExpiredForSurvival()
    {
        activePlayers.RemoveAll(p => p == null);
        if (activePlayers.Count >= 2)
        {
            // 生き残っている人たちのインデックスを保存
            List<int> survivors = new List<int>();
            foreach (var p in activePlayers)
            {
                var health = p.GetComponent<PlayerHealth>();
                if (health != null) survivors.Add(health.playerIndex);
            }
            TriggerSuddenDeath(survivors);
        }
        else
        {
            ChangeMode(new GameOverMode());
        }
    }

    private void TriggerSuddenDeath(List<int> qualifiers)
    {
        _isSuddenDeathNext = true;
        _qualifiedIndices = new List<int>(qualifiers);
        RestartGame();
    }

    public void ChangeMode(IGameMode newMode)
    {
        if (_currentMode != null) _currentMode.OnExit();
        _currentMode = newMode;
        if (_currentMode != null) _currentMode.OnEnter();

        if (newMode is SurvivalMode) CurrentModeState = Mode.Survival;
        else if (newMode is SuddenDeathMode) CurrentModeState = Mode.SuddenDeath;
        else if (newMode is GameOverMode) CurrentModeState = Mode.GameOver;
    }

    public List<GameObject> GetActivePlayers() { activePlayers.RemoveAll(p => p == null); return activePlayers; }

    public void ShowResultUI(string name)
    {
        if (resultCanvas != null)
        {
            resultCanvas.SetActive(true);

            // 最初のボタン（リトライボタンなど）を強制的に選択状態にする
            Button b = resultCanvas.GetComponentInChildren<Button>();
            if (b != null)
            {
                b.Select(); // これでキーボードやコントローラーで押せるようになる
                EventSystem.current.SetSelectedGameObject(b.gameObject);
            }
        }
    }

    public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void BackToJoinScene(string s) => SceneManager.LoadScene(s);
}