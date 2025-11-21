using UnityEngine;
using UnityEngine.SceneManagement;


public class SeceneLoader : MonoBehaviour
{
    public string sceneload;
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneload);
    }
}
