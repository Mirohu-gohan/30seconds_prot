using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Text playerID;

<<<<<<< HEAD
/*    public void SetCount(int count)
    {
        //playerID.text += $"Player {count}\n";
        if (playerID != null)
        {
            playerID.text = "Player " + playerID.ToString();
        }
    }*/
=======
    public void SetCount(int count)
    {
        playerID.text += $"Player {count}\n";
    }
>>>>>>> kento
}
