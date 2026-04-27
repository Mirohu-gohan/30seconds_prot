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
    public static Mode selectedGameMode = Mode.Survival;
    // プレイヤーごとにリスポーン中かどうかを管理する（4人分）
    private bool[] isRespawning = new bool[4];


    [Header("【デバッグ用】直接シーン再生時のモード指定")]
    public bool useDebugMode = true; // trueなら下のモードを強制適用
    public Mode debugGameMode = Mode.ScoreMode;

    [Header("タイマー設定")]
    public float scoreModeTimeLimit = 40f; // スコアモード用 (奪い合いなので少し長めなど)

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

    [Header("スコア")]
    public Transform[] SpawnPoint;
    public float Spawntime = 3.0f;
    public static int[] currentScores = new int[4];//ゲーム時の現在のスコア

    [Header("スコアばらまき設定")]
    public GameObject scoreItemPrefab; // 上で作ったPrefabをセット
    public int dropAmountPerDeath = 1;  // 死んだ時に何個出すか

    public enum Mode { Survival, SuddenDeath, ScoreMode, GameOver }
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

        // BGM再生
        if (SoundManager.Instance != null)
        {
            if (_isSuddenDeathNext)
                SoundManager.Instance.PlayBGM(SoundManager.Instance.suddenDeathBGM);
            else
                SoundManager.Instance.PlayBGM(SoundManager.Instance.normalBattleBGM);
        }

        // ★ ここが重要！ ★
        // タイトルで選んだ「staticな値」を、現在の動的な状態（CurrentModeState）に反映させる
        CurrentModeState = selectedGameMode;

        if (_isSuddenDeathNext)
        {
            ChangeMode(new SuddenDeathMode());
            _isSuddenDeathNext = false;
        }
        else
        {
            _qualifiedIndices.Clear();
            // CurrentModeState（反映済みの値）を見てモードを確定させる
            if (CurrentModeState == Mode.ScoreMode)
            {
                ChangeMode(new ScoreMode(timerTextUI, scoreModeTimeLimit));
            }
            else
            {
                ChangeMode(new SurvivalMode(timerTextUI, survivalTimeLimit));
            }
        }

        join = GameObject.Find("JoinedManager");
        StartCoroutine(InitializeUIWithDelay());
        SetupUIForMode();
    }


    //カウントダウンの演出
    private IEnumerator StartCountdown()
    {
        yield return null;

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

            var moveScrit = player.GetComponent<MoveController>();
            if (moveScrit != null) moveScrit.enabled = enabled;

            /*            var moveScrit = player.GetComponent<PlayerController1>();
                        if (moveScrit != null) moveScrit.enabled = enabled;

                        var moveBotScript = player.GetComponent<BotPlayerController1>();
                        if (moveBotScript != null) moveBotScript.enabled = enabled;*/

        }


    }


    //現在の参加人数を取得してUIを作るロジックです
    private IEnumerator InitializeUIWithDelay()
    {
        yield return null;

        if (PlayerUIManager.Instance != null)
        {
            bool isScoreMode = (CurrentModeState == Mode.ScoreMode);

            PlayerUIManager.Instance.InitializePlayerUI(playerWins.Length, CurrentModeState == Mode.ScoreMode);

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

            if (CurrentModeState == Mode.ScoreMode)
                PlayerUIManager.Instance.UpdatePlayerScore(i, currentScores[i]);
            else
                PlayerUIManager.Instance.UpdatePlayerStars(i, playerWins[i]);
        }
    }

    public void AddScore(int playerIndex, int amount)
    {
        if (playerIndex < 0 || playerIndex >= currentScores.Length) return;

        if (CurrentModeState == Mode.ScoreMode)
        {

            currentScores[playerIndex] = Mathf.Max(0, currentScores[playerIndex] + amount);

            if (PlayerUIManager.Instance != null)
            {
                PlayerUIManager.Instance.UpdatePlayerScore(playerIndex, currentScores[playerIndex]);
            }
        }
    }

    void Update()
    {
        if (!isGameStarted || CurrentModeState == Mode.GameOver) return;

        if (_currentMode != null) _currentMode.OnUpdate();
        CheckPlayersFalling();
    }

    //現在のラウンド数を表示
    public void UpdateRoundDisplay()
    {
        if (roundTextUI != null)
        {
            if (selectedGameMode == Mode.ScoreMode)
            {
                roundTextUI.gameObject.SetActive(false);
                return;
            }
            else
            {
                if (CurrentModeState == Mode.SuddenDeath)
                {
                    roundTextUI.gameObject.SetActive(false);
                }
                else
                {
                    roundTextUI.text = "Round " + CurrentRound;
                    // roundTextUI.color = Color.white; 
                }
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

        if (!activePlayers.Contains(p)) { activePlayers.Add(p); }

        if (CurrentModeState == Mode.SuddenDeath && _currentMode is SuddenDeathMode suddenMode)
        {
            suddenMode.PowerUpSinglePlayer(p);
        }
        if (PlayerUIManager.Instance != null)
        {
            PlayerUIManager.Instance.InitializePlayerUI(playerWins.Length, CurrentModeState == Mode.ScoreMode);
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
        if (CurrentModeState == Mode.GameOver) return;

        // 生存チェック（既存の処理）
        List<int> currentLiving = new List<int>();
        foreach (var p in activePlayers)
        {
            if (p != null)
            {
                var h = p.GetComponent<PlayerHealth>();
                if (h != null) currentLiving.Add(h.playerIndex);
            }
        }
        if (currentLiving.Count > 0) _lastActiveIndices = new List<int>(currentLiving);

        // 落下判定ループ
        for (int i = activePlayers.Count - 1; i >= 0; i--)
        {
            GameObject player = activePlayers[i];
            if (player == null) { activePlayers.RemoveAt(i); continue; }

            var health = player.GetComponent<PlayerHealth>();
            if (health == null) continue;
            int pIndex = health.playerIndex;

            // 【ここが重要！】すでに復活処理中なら無視する
            if (isRespawning[pIndex]) continue;

            // 落下判定
            if (player.transform.position.y < deathYCoordinate || player.transform.position.y > upperDeathYCoordinate)
            {
                // ★二重呼び出し防止：ここで「リスポーン中」にする
                isRespawning[pIndex] = true;

                // 1. スコア減算（PlayerScoreHandler側）
                var scoreHandler = player.GetComponent<PlayerScoreHandler>();
                if (scoreHandler != null)
                {
                    scoreHandler.HandleDeath();
                }

                // 2. 演出（音・UI）
                if (SoundManager.Instance != null)
                    SoundManager.Instance.PlaySE(SoundManager.Instance.groundBreakSE);

                if (PlayerUIManager.Instance != null)
                    PlayerUIManager.Instance.SetPlayerDead(pIndex);

                // 3. モード別の処理
                if (CurrentModeState == Mode.ScoreMode)
                {
                    StartCoroutine(RespawnPlayer(player, pIndex));
                }
                else
                {
                    OnPlayerEliminated(player);
                    Destroy(player);
                }
            }
        }
    }


    //スコアモードの時にリスポーンする処理
    private IEnumerator RespawnPlayer(GameObject player, int playerIndex)
    {
        // --- A. 死亡時のスコアから放出数を計算 (減点される前のスコアを元にする場合) ---
        // 現在のスコアの半分を放出アイテム数にする（例：10点持ってたら5個出す）
        int currentTotalScore = currentScores[playerIndex];
        int dropCount = 3;

        // 1. 非表示にする
        player.SetActive(false);

        // Rigidbodyを取得
        Rigidbody rb = player.GetComponent<Rigidbody>();

        yield return new WaitForSeconds(Spawntime);

        // 2. 位置を決定
        Vector3 spawnPosition = Vector3.zero;
        if (SpawnPoint != null && SpawnPoint.Length > 0)
        {
            int targetIndex = (playerIndex < SpawnPoint.Length) ? playerIndex : 0;
            spawnPosition = SpawnPoint[targetIndex].position;
        }

        // 3. 出現させる「前」に物理を強制停止
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 4. 座標を移動して再出現
        player.transform.position = spawnPosition;
        player.SetActive(true);

        // ★ 5. リスポーン地点でアイテムを放出！
        var scoreHandler = player.GetComponent<PlayerScoreHandler>();
        if (scoreHandler != null)
        {
            // PlayerScoreHandler側で作った関数を呼び出す
            scoreHandler.DropItemsAtRespawn(dropCount);
        }

        // 6. 各種状態のリセット
        var pController = player.GetComponent<PlayerController1>();
        if (pController != null) pController.ResetPlayerState();

        if (PlayerUIManager.Instance != null)
        {
            PlayerUIManager.Instance.ResetPlayerStatus(playerIndex);
        }

        // 7. 1フレーム待ってから物理演算を再開
        yield return new WaitForFixedUpdate();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        isRespawning[playerIndex] = false;
    }

    // プレイヤーの死亡時に呼ばれる想定
    public void DropScore(Vector3 deathPosition)
    {
        for (int i = 0; i < dropAmountPerDeath; i++)
        {
            if (scoreItemPrefab == null) break;

            GameObject item = Instantiate(scoreItemPrefab, deathPosition + Vector3.up, Quaternion.identity);
            ScoreItem script = item.GetComponent<ScoreItem>();

            if (script != null)
            {
                // ランダムな方向を計算
                Vector3 randomDir = new Vector3(
                    Random.Range(-1f, 1f),
                    1.5f, // 少し上に跳ねさせる
                    Random.Range(-1f, 1f)
                ).normalized;

                // Launchメソッドを呼ぶ（ScoreItem側にも後で追加します）
                script.Launch(randomDir, Random.Range(3f, 7f));
            }
        }
    }



    public void TimeExpiredForSurvival()
    {
        NextRound(true);
    }

    public void NextRound(bool isTimeUp = false)
    {
        // --- 1. スコアモードの場合 ---
        if (CurrentModeState == Mode.ScoreMode)
        {
            if (isTimeUp)
            {
                int maxScore = -1;
                int winnerIndex = -1;
                bool isDraw = false;

                for (int i = 0; i < currentScores.Length; i++)
                {
                    if (currentScores[i] > maxScore) { maxScore = currentScores[i]; winnerIndex = i; isDraw = false; }
                    else if (currentScores[i] == maxScore && maxScore != -1) { isDraw = true; }
                }

                if (!isDraw && winnerIndex != -1) playerWins[winnerIndex]++;
            }
            isGameStarted = false;
            StartCoroutine(WaitAndShowResult());
            return;
        }

        // --- 2. サバイバル・サドンデスモードの場合 ---
        else
        {
            // ★重要：リストを掃除してから人数を数える
            activePlayers.RemoveAll(p => p == null);
            int survivorCount = activePlayers.Count;

            // 【勝利判定】生き残りがちょうど1人なら、その人に星をあげる
            if (survivorCount == 1)
            {
                GameObject winner = activePlayers[0];
                var health = winner.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    playerWins[health.playerIndex]++; // ここで星が増える
                    Debug.Log($"Player {health.playerIndex + 1} に星を追加！ 現在の星: {playerWins[health.playerIndex]}");
                }
            }
            // 【サドンデス判定】タイムアップ、または全員同時に落ちた場合
            else if (isTimeUp || survivorCount == 0)
            {
                List<int> survivors = new List<int>();
                foreach (var p in activePlayers)
                {
                    if (p != null) survivors.Add(p.GetComponent<PlayerHealth>().playerIndex);
                }
                if (survivors.Count == 0) survivors = _lastActiveIndices;

                TriggerSuddenDeath(survivors);
                return;
            }
        }

        // --- 3. 共通：ゲーム全体の決着がついたかチェック ---
        CheckForGameWinner();
    }

    private void CheckForGameWinner()
    {
        bool someoneReachedThreeWins = false;
        for (int i = 0; i < playerWins.Length; i++)
        {
            if (playerWins[i] >= 3) { someoneReachedThreeWins = true; break; }
        }

        if (someoneReachedThreeWins)
        {
            StartCoroutine(WaitAndShowResult());
        }
        else
        {
            // まだ誰も3勝してなければ次のラウンドへリロード
            CurrentRound++;
            RestartGame();
        }
    }
    private IEnumerator WaitAndShowResult()
    {
        if (SoundManager.Instance != null)
        {
            // 1. 今のバトルBGMを止める
            SoundManager.Instance.StopBGM();

            // 2. リザルト用の音を鳴らす
            // resultBGMが短いジングルならPlaySE、長い曲ならPlayBGM
            SoundManager.Instance.PlayBGM(SoundManager.Instance.resultBGM);
        }

        if (PlayerUIManager.Instance != null)
        {
            for (int i = 0; i < playerWins.Length; i++)
            {
                PlayerUIManager.Instance.UpdatePlayerScore(i, playerWins[i]);
            }
        }
        yield return new WaitForSeconds(1.0f);

        string finalwinner = GetWinnerName();

        if (CurrentModeState == Mode.ScoreMode)
        {
            finalwinner = GetScoreWinnerName(); // スコアで判定
        }
        else
        {
            finalwinner = GetWinnerName(); // 3勝したかで判定
        }

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
        else if (newMode is ScoreMode) CurrentModeState = Mode.ScoreMode;
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


    // スコアモード用の勝者判定（ポイントが一番多い人）
    public string GetScoreWinnerName()
    {
        int maxScore = -1;
        List<string> winners = new List<string>();

        // 最大スコアを探す
        for (int i = 0; i < currentScores.Length; i++)
        {
            if (currentScores[i] > maxScore)
            {
                maxScore = currentScores[i];
            }
        }

        // 同点の場合も考えてリストアップ
        for (int i = 0; i < currentScores.Length; i++)
        {
            if (currentScores[i] == maxScore && maxScore != -1)
            {
                winners.Add("Player " + (i + 1));
            }
        }

        if (winners.Count == 0) return "No Winner";
        return string.Join(" & ", winners);
    }

    // モードによってUIを出し分ける
    private void SetupUIForMode()
    {
        UpdateRoundDisplay();

        bool isScore = (CurrentModeState == Mode.ScoreMode);

        if (PlayerUIManager.Instance != null)
        {
            for (int i = 0; i < playerWins.Length; i++)
            {
                // 名前を UpdatePlayerUI に合わせて呼び出す
                PlayerUIManager.Instance.UpdatePlayerUI(i, isScore);
            }
        }
    }
}