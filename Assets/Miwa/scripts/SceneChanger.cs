using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // シーン名で移動（ご提示のコードと同じ）
    public void LoadByString(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Debug.Log("ボタンが押されました！ターゲットシーン: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    // インデックス番号で移動（ステージ1, 2...と続く場合に便利）
    public void LoadByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // 現在のシーンをリロード（ゲームオーバー時などに）
    public void ReloadScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }
}
