using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // サバイバルボタンに登録

    private const float DelayTime = 0.5f;
    private float canTransitionTime = 0f;

    void Start()
    {
        canTransitionTime = Time.time + DelayTime;
    }

    public void OnClickSurvivalMode()
    {
        GameManager_M.selectedGameMode = GameManager_M.Mode.Survival;
        Debug.Log("サバイバルモードを選択しました");
    }

    // スコアモードボタンに登録
    public void OnClickScoreMode()
    {
        GameManager_M.selectedGameMode = GameManager_M.Mode.ScoreMode;
        Debug.Log("スコアモードを選択しました");
    }

    // ゲーム開始ボタンに登録（引数にシーン名を入れる）
    public void LoadBattleScene(string sceneName)
    {
        if (Time.time < canTransitionTime) return;

        SceneManager.LoadScene(sceneName);
    }
}
