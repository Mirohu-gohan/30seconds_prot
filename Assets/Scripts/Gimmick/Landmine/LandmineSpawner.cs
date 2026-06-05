using UnityEngine;
using System.Collections;

public class LandmineSpawner : MonoBehaviour
{
    [Header("スポーン設定")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector3 areaSize = new Vector3(10f, 0f, 10f);
    [SerializeField] private float spawnInterval = 2f;

    [Header("デバッグ表示")]
    [SerializeField] private bool showDebugArea = true;
    [SerializeField] private float debugAreaDistance = 10f;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            Vector3 randomPos = GetRandomPosition();
            GameObject obj = Instantiate(prefab, randomPos, Quaternion.identity);

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                StartCoroutine(FreezeYAfterDelay(rb, 2f)); // 2f の部分が秒数
            }

            // SphereCollider のサイズもスケールに合わせる
            SphereCollider sphere = obj.GetComponent<SphereCollider>();
            if (sphere != null)
            {
                float originalRadius = 0.860137f; // prefab の半径
                sphere.radius = originalRadius;
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private IEnumerator FreezeYAfterDelay(Rigidbody rb, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezePosition;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    private Vector3 GetRandomPosition()
    {
        float x = Random.Range(-areaSize.x / 2, areaSize.x / 2);
        float y = Random.Range(-areaSize.y / 2, areaSize.y / 2);
        float z = Random.Range(-areaSize.z / 2, areaSize.z / 2);
        return transform.position + new Vector3(x, y, z);
    }

    private void OnDrawGizmosSelected()
    {
        // 生成エリア（黄色）
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, areaSize);
    }
}