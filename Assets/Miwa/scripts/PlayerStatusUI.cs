using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    [Header("背景のImageコンポーネント")]
    public Image backgroundPanel; 

    private Sprite myAliveSprite;
    private Sprite myDeadSprite;

    [Header("星の画像たち")]
    public Image[] stars; 
    [Header("星の色設定")]
    public Color starOnColor = Color.yellow; 
    public Color starOffColor = Color.gray;  
    
    public void SetupUI(int currentScore, Sprite alive, Sprite dead)
    {
        myAliveSprite = alive;
        myDeadSprite = dead;

        if (backgroundPanel != null && myAliveSprite != null)
        {
            backgroundPanel.sprite = myAliveSprite;
            backgroundPanel.color = Color.white; 
        }
        
        UpdateStars(currentScore);
    }

    public void UpdateStars(int score)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].color = (i < score) ? starOnColor : starOffColor;
        }
    }

    public void SetEliminated(bool isDead)
    {
        if (backgroundPanel != null)
        {
            Sprite targetSprite = isDead ? myDeadSprite : myAliveSprite;
            if (targetSprite != null)
            {
                backgroundPanel.sprite = targetSprite;
            }
        }
    }
}