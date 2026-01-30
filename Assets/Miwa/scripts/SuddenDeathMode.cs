using UnityEngine;
using System.Reflection;
using static GameMode;

public class SuddenDeathMode : IGameMode
{
    public void OnEnter()
    {
        

        //UI‚Ì•\Ž¦
        if (GameManager_M.Instance != null && GameManager_M.Instance.suddenDeathUI != null)
        {
            GameManager_M.Instance.suddenDeathUI.SetActive(true);
        }
    }

    public void OnUpdate() { }

    public void OnExit()
    {
       

        //UI‚Ì”ñ•\Ž¦
        if (GameManager_M.Instance != null && GameManager_M.Instance.suddenDeathUI != null)
        {
            GameManager_M.Instance.suddenDeathUI.SetActive(false);
        }
    }

   

    public void PowerUpSinglePlayer(GameObject player)
    {
        var controller = player.GetComponent<PlayerController1>();
        if (controller != null)
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;
            FieldInfo strongField = typeof(PlayerController1).GetField("StrongKnockbackForce", flags);
            FieldInfo weakField = typeof(PlayerController1).GetField("WeakKnockbackForce", flags);

            if (strongField != null)
            {
                float val = (float)strongField.GetValue(controller);
                strongField.SetValue(controller, val * 10.0f);
            }
            if (weakField != null)
            {
                float val = (float)weakField.GetValue(controller);
                weakField.SetValue(controller, val * 10.0f);
            }
        }
    }
}