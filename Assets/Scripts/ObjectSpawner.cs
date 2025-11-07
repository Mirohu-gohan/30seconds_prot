using UnityEngine;
using System.Collections;

public class RandomSpawnerLoop : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector3 areaSize = new Vector3(10f, 0f, 10f);
    [SerializeField] private float spawnInterval = 2f; // 何秒ごとに生成

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            Vector3 randomPos = GetRandomPosition();
            Instantiate(prefab, randomPos, Quaternion.identity);
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
