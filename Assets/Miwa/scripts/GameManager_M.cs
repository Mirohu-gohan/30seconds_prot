using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using static GameMode;
using System.Collections;

public class GameManager_M : MonoBehaviour
{
    public static GameManager_M Instance { get; private set; }

    // --- 静的変数 ---
    private static bool _isSuddenDeathNext = false;
    private static List<int> _qualifiedIndices = new List<int>(); 

    // ラウンド管理用
    public static int CurrentRound = 1;

    [Header("UI設定")]
    public Text timerTextUI;
    public GameObject resultCanvas;
    
    [Header("ラウンド表示用")]
    public Text roundTextUI;
    
    [Header("リザルト表示用")]
    public Text resultTextUI;

    [Header("ゲーム設定")]
    public float survivalTimeLimit = 20.0f;
    public float deathYCoordinate = -10.0f;
    [Header("サウンド設定")]
    public AudioClip mapBGM;

    public enum Mode { Survival, SuddenDeath, GameOver }
    public Mode CurrentModeState;

    private IGameMode _currentMode;
    private List<GameObject> activePlayers = new List<GameObject>();

    public float suddenDeathKnockbackMultiplier = 2.0f; 
    public float currentKnockbackMultiplier = 1.0f;    

    private GameObject join;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1.0f;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM(mapBGM);
        }
        
        if (resultCanvas != null) resultCanvas.SetActive(false);
        if (timerTextUI != null) timerTextUI.gameObject.SetActive(true);

        UpdateRoundDisplay();

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

        join = GameObject.Find("JoinedManager");
    }

    void Update()
    {
        if (_currentMode != null) _currentMode.OnUpdate();
        CheckPlayersFalling();
    }

    public void UpdateRoundDisplay()
    {
        if (roundTextUI != null)
        {
            if (CurrentModeState == Mode.SuddenDeath)
            {
                roundTextUI.text = "SUDDEN DEATH";
                roundTextUI.color = Color.black;
            }
            else
            {
                roundTextUI.text = "Round " + CurrentRound;
                // roundTextUI.color = Color.white;
            }
            roundTextUI.gameObject.SetActive(true);
        }
    }

    public void RegisterPlayer(GameObject p, int index)
    {
        if (CurrentModeState == Mode.SuddenDeath && !_qualifiedIndices.Contains(index))
        {
            Destroy(p);
            return;
        }
        if (!activePlayers.Contains(p)) activePlayers.Add(p);
    }

    public void OnPlayerEliminated(GameObject eliminatedPlayer)
    {
        if (activePlayers.Contains(eliminatedPlayer)) activePlayers.Remove(eliminatedPlayer);
        activePlayers.RemoveAll(p => p == null);

        if (activePlayers.Count <= 1)
        {
            NextRound();
        }
    }

    private void CheckPlayersFalling()
    {
        if (CurrentModeState == Mode.GameOver) return;

        for (int i = activePlayers.Count - 1; i >= 0; i--)
        {
            GameObject player = activePlayers[i];
            if (player == null) { activePlayers.RemoveAt(i); continue; }

            if (player.transform.position.y < deathYCoordinate)
            {
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayFallSound();
                }
                OnPlayerEliminated(player);
                Destroy(player);
            }
        }
    }

    public void TimeExpiredForSurvival()
    {
        NextRound(true);
    }

    public void NextRound(bool isTimeUp = false)
    {
        if (isTimeUp || GetActivePlayersCount() == 0)
        {
            List<int> survivors = new List<int>();
            foreach (var p in GetActivePlayers())
            {
                var health = p.GetComponent<PlayerHealth>();
                if (health != null) survivors.Add(health.playerIndex);
            }
            if (survivors.Count == 0) survivors = new List<int> { 0, 1, 2, 3 };
            
            TriggerSuddenDeath(survivors);
            return; 
        }

        if (CurrentRound < 3)
        {
            CurrentRound++;
            RestartGame();
        }
        else
        {
            string winner = GetWinnerName();
            CurrentRound = 1; 
            ChangeMode(new GameOverMode(winner));
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

　　// リザルトUIを表示するメソッドであります！
    public void ShowResultUI(string resultText)
    {
        if (resultCanvas != null)
        {
            // ラウンド表示を消す
            if (roundTextUI != null) roundTextUI.gameObject.SetActive(false);

            resultCanvas.SetActive(true);
            if (resultTextUI != null) resultTextUI.text = resultText;

            Button firstButton = resultCanvas.GetComponentInChildren<Button>();
            if (firstButton != null)
            {
                firstButton.Select();
                EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
            }
        }
    }

    public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    public void BackToJoinScene(string s)
    {
        Destroy(join);
        SceneManager.LoadScene(s);
    }

    // エラー防止のためにいったんnull実装
    public void HideUI(float delay) { }
    private IEnumerator HideUIRoutine(float delay) { yield break; }

    public void SetAllPlayersControl(bool enabled)
    {
        foreach (var player in GetActivePlayers())
        {
            if (player == null) continue;
            var input = player.GetComponent<UnityEngine.InputSystem.PlayerInput>();
            if (input != null) input.enabled = enabled;
        }
    }

    public List<GameObject> GetActivePlayers() { activePlayers.RemoveAll(p => p == null); return activePlayers; }
    
    public int GetActivePlayersCount()
    {
        int count = 0;
        foreach (var p in activePlayers) if (p != null) count++;
        return count;
    }

    public string GetWinnerName()
    {
        foreach (var p in activePlayers) if (p != null) return p.name;
        return "Unknown";
    }
}