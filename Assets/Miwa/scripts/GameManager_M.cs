using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using static GameMode;

public class GameManager_M : MonoBehaviour
{
    public static GameManager_M Instance { get; private set; }

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

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f;
        if (resultCanvas != null) resultCanvas.SetActive(false);
        ChangeMode(new SurvivalMode(timerTextUI, survivalTimeLimit));
    }

    void Update()
    {
        if (_currentMode != null) _currentMode.OnUpdate();

        //常にプレイヤーの全落下チェックを行う
        CheckPlayersFalling();
    }

    // 落下をチェック
    private void CheckPlayersFalling()
    {
        // プレイヤーを一人ずつ確認
        for (int i = activePlayers.Count - 1; i >= 0; i--)
        {
            GameObject player = activePlayers[i];
            if (player != null)
            {
                // 設定した高さより下に行ったら
                if (player.transform.position.y < deathYCoordinate)
                {
                    // PlayerHealthの死亡処理を呼ぶ
                    var health = player.GetComponent<PlayerHealth>();
                    if (health != null) health.OnFallOut();
                }
            }
        }
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

    public void RegisterPlayer(GameObject p) { if (!activePlayers.Contains(p)) activePlayers.Add(p); }

    public void OnPlayerEliminated()
    {
        activePlayers.RemoveAll(p => p == null);
        if (activePlayers.Count <= 1) ChangeMode(new GameOverMode());
    }

    public void TimeExpiredForSurvival()
    {
        activePlayers.RemoveAll(p => p == null);
        if (activePlayers.Count >= 2) ChangeMode(new SuddenDeathMode());
        else ChangeMode(new GameOverMode());
    }

    public List<GameObject> GetActivePlayers() { activePlayers.RemoveAll(p => p == null); return activePlayers; }

    public void ShowResultUI(string name)
    {
        if (resultCanvas != null) resultCanvas.SetActive(true);
        Button b = resultCanvas.GetComponentInChildren<Button>();
        if (b != null) EventSystem.current.SetSelectedGameObject(b.gameObject);
    }

    public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void BackToJoinScene(string s) => SceneManager.LoadScene(s);
}