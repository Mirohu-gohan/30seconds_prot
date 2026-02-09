using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [Header("ポーズ画面のパネル")]
    public GameObject pausePanel;

    // ぼかし用のGlobal Volume
    [Header("ぼかし演出用のVolume")]
    public GameObject blurVolume;

    private bool isPaused = false;

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        // ★開始時はぼかしをOFFにしておく
        if (blurVolume != null) blurVolume.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }

        if (Gamepad.current != null && Gamepad.current.selectButton.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (GameManager_M.Instance != null && GameManager_M.Instance.CurrentModeState == GameManager_M.Mode.GameOver)
        {
            return;
        }

        isPaused = !isPaused;

        if (pausePanel != null) pausePanel.SetActive(isPaused);
        if (blurVolume != null) blurVolume.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
        AudioListener.pause = isPaused;
    }


    public void OnRestartButton()
    {
        ResetPauseState();
        if (GameManager_M.Instance != null)
        {
            GameManager_M.Instance.RestartGame();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void OnTitleButton()
    {
        ResetPauseState();
        SceneManager.LoadScene("TitleScene");
    }

   //シーン遷移前にぼかしや時間を確実にリセットする用
    private void ResetPauseState()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        if (blurVolume != null) blurVolume.SetActive(false);
    }
}