using UnityEngine;

public class MainManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    void Start()
    {
        int num = SelectPlayer.Instance.playercount;
        Debug.Log("Player Count: " + num);

        SpawnPlayers(num);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnPlayers(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Instantiate(playerPrefab, new Vector3(i * 2, 0, 0), Quaternion.identity);
        }
    }
}
