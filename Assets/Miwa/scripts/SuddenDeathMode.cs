using UnityEngine;
using static GameMode;

public class SuddenDeathMode : IGameMode
{
    public void OnEnter()
    {
        

        //UI�̕\��
        if (GameManager_M.Instance != null && GameManager_M.Instance.suddenDeathUI != null)
        {
            GameManager_M.Instance.suddenDeathUI.SetActive(true);
        }
    }

    public void OnUpdate() { }

    public void OnExit()
    {
       

        //UI�̔�\��
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
            controller.ApplyKnockbackMultiplier(10.0f);
        }
    }
}