using UnityEngine;
using UnityEngine.SceneManagement;

public class Loop : MonoBehaviour
{
    [SerializeField] private float loopInterval = 3f; // ループ間隔（秒）

    [SerializeField] private bool enableLoop = false;

    void Start()
    {
#if UNITY_EDITOR
        if (enableLoop)
            InvokeRepeating(nameof(LoopScene), loopInterval, loopInterval);
#endif
    }

    public void LoopScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
