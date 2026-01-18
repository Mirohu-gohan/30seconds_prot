using UnityEngine;
using System.Collections.Generic;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager Instance { get; private set; }

    [Header("UI設定")]
    public Transform uiContainer;
    public GameObject statusUIPrefab;

    [Header("【各プレイヤー用】生存・死亡画像設定")]
    public Sprite[] aliveSprites;
    public Sprite[] deadSprites; 

    private List<PlayerStatusUI> spawnedUIs = new List<PlayerStatusUI>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void InitializePlayerUI(int playerCount)
    {
        foreach (var ui in spawnedUIs) if (ui != null) Destroy(ui.gameObject);
        spawnedUIs.Clear();

        if (statusUIPrefab == null || uiContainer == null) return;

        for (int i = 0; i < playerCount; i++)
        {
            GameObject uiObj = Instantiate(statusUIPrefab, uiContainer);
            PlayerStatusUI statusUI = uiObj.GetComponent<PlayerStatusUI>();

            if (statusUI != null)
            {
                Sprite myAlive = (i < aliveSprites.Length) ? aliveSprites[i] : null;
                Sprite myDead = (i < deadSprites.Length) ? deadSprites[i] : null;
                statusUI.SetupUI(0, myAlive, myDead);
                
                spawnedUIs.Add(statusUI);
            }
        }
    }

    public void SetPlayerDead(int playerIndex)
    {
        if (playerIndex >= 0 && playerIndex < spawnedUIs.Count)
        {
            spawnedUIs[playerIndex].SetEliminated(true);
        }
    }

    public void UpdatePlayerScore(int playerIndex, int score)
    {
        if (playerIndex >= 0 && playerIndex < spawnedUIs.Count)
        {
            spawnedUIs[playerIndex].UpdateStars(score);
        }
    }

    public void ResetAllUIState()
    {
        foreach (var ui in spawnedUIs)
        {
            ui.SetEliminated(false);
        }
    }
}