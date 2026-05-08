using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // 二重クリック（連打）によるエラーを防止するためのフラグ
    private bool isLoading = false;

    // シーン名で移動
    public void LoadByString(string sceneName)
    {
        // もでにロード中なら、二回目以降の命令は無視する
        if (isLoading) return;
        isLoading = true;
        SceneManager.LoadScene(sceneName);
    }

    // インデックス番号で移動
    public void LoadByIndex(int sceneIndex)
    {
        if (isLoading) return;
        isLoading = true;

        SceneManager.LoadScene(sceneIndex);
    }

    // 現在のシーンをリロード
    public void ReloadScene()
    {
        if (isLoading) return;
        isLoading = true;

        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        // Unityエディタ実行中の場合は、再生モードを停止する
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // ビルドされた実際のゲームでは、アプリを完全に終了する
            Application.Quit();
#endif
    }
}
