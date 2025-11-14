using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectPlayer : MonoBehaviour
{
    public static SelectPlayer Instance;

    public int playercount = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayerCount(int count)
    {
        SelectPlayer.Instance.playercount = count;
    }
}
