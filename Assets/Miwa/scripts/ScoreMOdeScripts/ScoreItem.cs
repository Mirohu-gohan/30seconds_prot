using UnityEngine;

public class ScoreItem : MonoBehaviour
{
    private bool isCollected = false;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 direction, float force)
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(direction * force, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player"))
        {
            isCollected = true;

            // プレイヤーのIDを取得
            int id = -1;
            var health = other.GetComponent<PlayerHealth>();
            if (health != null) id = health.playerIndex;

            if (id != -1)
            {
                // スコア加算
                GameManager_M.Instance.AddScore(id, 1);

                // 演出（あれば）
                // SoundManager.Instance.PlaySE(...);

                Destroy(gameObject);
            }
        }
    }
}