using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [Header("ポーズ画面のパネル")]
    public GameObject pausePanel;

    private bool isPaused = false;

    private Gamepad pad;

    void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
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
        isPaused = !isPaused;

        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
        }

        // 時間を止める
        Time.timeScale = isPaused ? 0f : 1f;

        AudioListener.pause = isPaused;
    }

    public void OnRestartButton()
    {
        // 時間と音を戻してからリスタート
        Time.timeScale = 1f;
        AudioListener.pause = false; 

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
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("TitleScene");
    }
}