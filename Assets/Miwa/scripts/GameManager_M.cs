using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using static GameMode;
using System.Collections;
using Unity.VisualScripting;

public class GameManager_M : MonoBehaviour
{
    public static GameManager_M Instance { get; private set; }

    // --- 静的変数 ---
    private static bool _isSuddenDeathNext = false;
    private static List<int> _qualifiedIndices = new List<int>(); 
    public static int CurrentRound = 1;
    public static int[] playerWins = new int[4];

    [Header("UI設定")]
    public Text timerTextUI;
    public GameObject resultCanvas;
    public Text CountdownUI;
    
    [Header("ラウンド表示用")]
    public Text roundTextUI;

    [Header("リザルト表示用")]
    public Text resultTextUI;
    public Text winnerNameTextUI;

    [Header("サドンデス")]
    public GameObject suddenDeathUI;

    [Header("ゲーム設定")]
    public float survivalTimeLimit = 20.0f;
    public float deathYCoordinate = -10.0f;
    public float upperDeathYCoordinate = 20.0f;

    [Header("リザルト演出用")]
    public GameObject ResultCanvas;      // リザルト用のCanvas（またはPanel）
    public RectTransform resultRibbon;    // スパン！！と動かすボタングループ
    public GameObject resultBlurVolume;  // リザルト時に自動でONにするGlobal Volume

    public enum Mode { Survival, SuddenDeath, GameOver }
    public Mode CurrentModeState;

    private IGameMode _currentMode;
    private List<GameObject> activePlayers = new List<GameObject>();

    public float suddenDeathKnockbackMultiplier = 2.0f; 
    public float currentKnockbackMultiplier = 1.0f;

    private List<int> _lastActiveIndices = new List<int>();//同時にデスした時のため直前に生きていたPlayerを格納

    private GameObject join;

    private bool isGameStarted = false;//ゲーム開始フラグ
    public bool IsGameStartedProperty => isGameStarted;

    void Awake()
    {
        if (CurrentRound == 1)
        {
            for (int i = 0; i < playerWins.Length; i++)
            {
                playerWins[i] = 0;
            }
        }
        AudioListener.pause = false;
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        StartCoroutine(StartCountdown());//ゲーム開始時にカウントダウンを入れてディレイ
    }

    void Start()
    {
        Time.timeScale = 1.0f;
        
        if (resultCanvas != null) resultCanvas.SetActive(false);
        if (timerTextUI != null) timerTextUI.gameObject.SetActive(true);

        UpdateRoundDisplay();

　　　　//ここにBGM再生の処理を追加したよ～ん
        if (SoundManager.Instance != null)
        {
            if (_isSuddenDeathNext)
            {
                // サドンデス用BGM
                SoundManager.Instance.PlayBGM(SoundManager.Instance.suddenDeathBGM);
            }
            else
            {
                // 通常バトル用BGM
                SoundManager.Instance.PlayBGM(SoundManager.Instance.normalBattleBGM);
            }
        }

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
        StartCoroutine(InitializeUIWithDelay());
    }


    private IEnumerator StartCountdown()
    {
        isGameStarted = false;
        SetAllPlayersControl(false);
        if (_currentMode is SurvivalMode survival) survival.isTimerActive = false;

        int count = 3;
        while (count > 0)
        {
            if (CountdownUI != null)
            {
                CountdownUI.text = count.ToString();
                CountdownUI.color = (count <= 1) ? Color.red : Color.white;

                // 演出：残像エフェクト
                StartCoroutine(GhostTrailEffect(CountdownUI));

                yield return new WaitForSeconds(1.0f);
                count--;
            }
        }

        if (CountdownUI != null)
        {
            CountdownUI.text = "Fight!!";
            CountdownUI.color = Color.yellow;
            StartCoroutine(GhostTrailEffect(CountdownUI));

            SoundManager.Instance.PlaySE(SoundManager.Instance.gameStartGongSE);
            isGameStarted = true;
            SetAllPlayersControl(true);
            if (_currentMode is SurvivalMode survivalstart) survivalstart.isTimerActive = true;

            yield return new WaitForSeconds(1.0f);
            CountdownUI.text = "";
        }
    }

    // 演出用サブコルーチン
    private IEnumerator GhostTrailEffect(Text uiText)
    {
        // 残像用のクローンを生成
        Text ghost = Instantiate(uiText, uiText.transform.parent);
        ghost.transform.localPosition = uiText.transform.localPosition;

        float duration = 0.6f;
        float elapsed = 0f;
        Vector3 startScale = Vector3.one;
        Vector3 endScale = new Vector3(3.0f, 3.0f, 1f); // 大きく広がる

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 残像を拡大させながら透明にする
            ghost.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            ghost.color = new Color(uiText.color.r, uiText.color.g, uiText.color.b, 1f - t);

            yield return null;
        }
        Destroy(ghost.gameObject);
    }
    //ゲームスタート時の処理の停止
    public void SetAllPlayersControl(bool enabled)
    {
        foreach (var player in GetActivePlayers())
        {
            if (player == null) continue;
            var input = player.GetComponent<UnityEngine.InputSystem.PlayerInput>();
            if (input != null) input.enabled = enabled;

            var rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                if (!enabled)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = true;
                }
                else
                {
                    rb.isKinematic = false;
                }
            }

            var moveScrit = player.GetComponent<PlayerController1>();
            if (moveScrit != null) moveScrit.enabled = enabled;

            var moveBotScript = player.GetComponent<BotPlayerController>();
            if (moveBotScript != null) moveBotScript.enabled = enabled;

        }


    }


    //現在の参加人数を取得してUIを作るロジックです
    private IEnumerator InitializeUIWithDelay()
    {
        yield return null; 
        
        if (PlayerUIManager.Instance != null)
        {
            PlayerUIManager.Instance.InitializePlayerUI(playerWins.Length);

            if (CurrentModeState == Mode.SuddenDeath)
            {
                for (int i = 0; i < playerWins.Length; i++)
                {
                    if (!_qualifiedIndices.Contains(i))
                    {
                        PlayerUIManager.Instance.SetPlayerDead(i);
                    }
                }
            }
        }
        for (int i = 0; i < playerWins.Length; i++)
        {
            PlayerUIManager.Instance.UpdatePlayerScore(i, playerWins[i]);
        }
    }

    void Update()
    {
        if (!isGameStarted || CurrentModeState == Mode.GameOver) return;

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
                roundTextUI.color = Color.red;
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

        if(!activePlayers.Contains(p)){ activePlayers.Add(p);}

        if (CurrentModeState == Mode.SuddenDeath && _currentMode is SuddenDeathMode suddenMode)
        {
            suddenMode.PowerUpSinglePlayer(p);
        }
        if (PlayerUIManager.Instance != null)
        {
            PlayerUIManager.Instance.InitializePlayerUI(playerWins.Length);
        }
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

    //落下、吹っ飛び時にDeathする処理
    private void CheckPlayersFalling()
    {
        //同時に落ちた場合の処理
        if (CurrentModeState == Mode.GameOver) return;

        List<int> currentLiving = new List<int>();
        foreach (var p in activePlayers)
        {
            if (p != null)
            {
                var h = p.GetComponent<PlayerHealth>();
                if (h != null) currentLiving.Add(h.playerIndex);
            }
        }

        if(currentLiving.Count > 0)
        {
            _lastActiveIndices =new List<int>(currentLiving);
        }

        for (int i = activePlayers.Count - 1; i >= 0; i--)
        {
            GameObject player = activePlayers[i];
            if (player == null) { activePlayers.RemoveAt(i); continue; }

            if (player.transform.position.y < deathYCoordinate|| player.transform.position.y>upperDeathYCoordinate)
            {
                if (SoundManager.Instance != null)
                {
                     SoundManager.Instance.PlaySE(SoundManager.Instance.groundBreakSE); 
                }

                var health = player.GetComponent<PlayerHealth>(); 
                if (health != null && PlayerUIManager.Instance != null)
                {
                    PlayerUIManager.Instance.SetPlayerDead(health.playerIndex);
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
        // 1. 1人だけ生き残っている場合、そのプレイヤーに勝ち星を付与
        if (GetActivePlayersCount() == 1)
        {
            var winner = activePlayers[0];
            var health = winner.GetComponent<PlayerHealth>();
            if (health != null)
            {
                playerWins[health.playerIndex]++;
                Debug.Log($"Player {health.playerIndex + 1} が勝利！ 現在の勝ち星: {playerWins[health.playerIndex]}");
            }
        }

        // 2. タイムアップまたは全員死亡の場合、サドンデスへ
        if (isTimeUp || GetActivePlayersCount() == 0)
        {
            List<int> survivors = new List<int>();
            foreach (var p in GetActivePlayers())
            {
                var health = p.GetComponent<PlayerHealth>();
                if (health != null) survivors.Add(health.playerIndex);
            }
            if (survivors.Count == 0)
            {
                survivors = (_lastActiveIndices.Count > 0) ? _lastActiveIndices : new List<int> { 0, 1, 2, 3 };
            }
            TriggerSuddenDeath(survivors);
            return;
        }

        // 誰かが3勝したかどうかをチェック
        bool someoneReachedThreeWins = false;
        for (int i = 0; i < playerWins.Length; i++)
        {
            if (playerWins[i] >= 3) // 3勝に到達したか
            {
                someoneReachedThreeWins = true;
                break;
            }
        }

        //  判定結果による分岐
        if (someoneReachedThreeWins)
        {
            // 誰かが3勝したらリザルト画面へ
            StartCoroutine(WaitAndShowResult());
        }
        else
        {
            // まだ3勝した人がいなければ、次のラウンドへ（リロード）
            CurrentRound++; // 表示上のラウンド数をカウントアップ
            RestartGame();
        }
    }

    private IEnumerator WaitAndShowResult()
    {
        if (PlayerUIManager.Instance != null)
        {
            for (int i = 0; i < playerWins.Length; i++)
            {
                PlayerUIManager.Instance.UpdatePlayerScore(i, playerWins[i]);
            }
        }
        yield return new WaitForSeconds(1.0f);

        string finalwinner = GetWinnerName();
        CurrentRound = 1;
        ChangeMode(new GameOverMode(finalwinner));
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

    public void ShowResultUI(string resultText)
    {
        
        if (resultCanvas != null)
        {
            resultCanvas.SetActive(true);
        }

       
        var pm = Object.FindFirstObjectByType<PauseManager>();
        if (pm != null && pm.pausePanel != null)
        {
            pm.pausePanel.SetActive(false);
        }

        
        if (resultBlurVolume != null)
        {
            resultBlurVolume.SetActive(true);
        }

        if (winnerNameTextUI != null)
        {
            winnerNameTextUI.text = resultText;
            winnerNameTextUI.gameObject.SetActive(true);
        }

       
        if (resultTextUI != null)
        {
            resultTextUI.text = "Result";
            resultTextUI.gameObject.SetActive(true);
        }

       
        if (resultRibbon != null)
        {
            resultRibbon.gameObject.SetActive(true);
            StartCoroutine(AnimateButtonsSwipe());
        }

       
        Button firstButton = resultCanvas.GetComponentInChildren<Button>();
        if (firstButton != null)
        {
            firstButton.Select();
        }
    }

    private IEnumerator AnimateButtonsSwipe()
    {
        if (resultRibbon == null) yield break;

        
        Vector2 endPos = Vector2.zero;
       
        Vector2 startPos = new Vector2(2000, 0);

      
        resultRibbon.anchoredPosition = startPos;

        float duration = 0.3f;
        float elapsed = 0f;

        
        Time.timeScale = 0f;

        while (elapsed < duration)
        {
            
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;

           
            t = 1f - Mathf.Pow(1f - t, 5f);

            resultRibbon.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

      
        resultRibbon.anchoredPosition = endPos;
    }

    public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    public void BackToJoinScene(string s)
    {
        CurrentRound = 1;         
        _isSuddenDeathNext = false;  
        _qualifiedIndices.Clear();

        for (int i = 0; i < playerWins.Length; i++)  // 全プレイヤーの勝利数を0にする
        {
            playerWins[i] = 0;
        }

        Destroy(join);
        SceneManager.LoadScene(s);
        AudioListener.pause = true;
    }

    public void HideUI(float delay) { }
    private IEnumerator HideUIRoutine(float delay) { yield break; }

    public List<GameObject> GetActivePlayers() { activePlayers.RemoveAll(p => p == null); return activePlayers; }
    
    public int GetActivePlayersCount()
    {
        int count = 0;
        foreach (var p in activePlayers) if (p != null) count++;
        return count;
    }

    //勝利者の名前を表示する
    public string GetWinnerName()
    {
        List<string> winners = new List<string>();

        // 単純に3勝しているプレイヤーをリストアップ
        for (int i = 0; i < playerWins.Length; i++)
        {
            if (playerWins[i] >= 3)
            {
                winners.Add("Player " + (i + 1));
            }
        }

        // もしバグ等で3勝がいない場合は最大スコアの人を出す
        if (winners.Count == 0)
        {
            int maxWins = 0;
            for (int i = 0; i < playerWins.Length; i++)
                if (playerWins[i] > maxWins) maxWins = playerWins[i];

            for (int i = 0; i < playerWins.Length; i++)
                if (playerWins[i] == maxWins) winners.Add("Player " + (i + 1));
        }

        return string.Join(" & ", winners);
    }
}