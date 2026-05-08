using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
    [SerializeField] public float lifetime = 5f; // 何秒後に消えるか

    private float timer = 0f; // 経過時間をカウント

    void Update()
    {
        // 前のフレームからの時間を加算
        timer += Time.deltaTime;

        // 一定時間を超えたら削除
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
