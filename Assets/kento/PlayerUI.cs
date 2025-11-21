using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Text playerID;

    public void SetCount(int count)
    {
        playerID.text += $"Player {count}\n";
    }
}
