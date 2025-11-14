using UnityEngine;
using System.Collections;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector3 areaSize = new Vector3(10f, 0f, 10f);
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private Vector2 scaleRange = new Vector2(0.5f, 2f);

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

            // ランダムスケール
            float randomScale = Random.Range(scaleRange.x, scaleRange.y);
            obj.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

            // SphereCollider のサイズもスケールに合わせる
            SphereCollider sphere = obj.GetComponent<SphereCollider>();
            if (sphere != null)
            {
                float originalRadius = 0.860137f; // prefab の半径が 0.5 の場合
                sphere.radius = originalRadius * randomScale;
            }

            yield return new WaitForSeconds(spawnInterval);
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, areaSize);
    }
}
