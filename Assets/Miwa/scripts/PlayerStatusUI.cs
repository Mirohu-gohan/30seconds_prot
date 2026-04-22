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

    [Header("スコア表示用")]
    public Text scoreText;


    
    public void SetupUI(int initialValue, Sprite alive, Sprite dead,bool isScoreMode)
    {
        myAliveSprite = alive;
        myDeadSprite = dead;

        if (backgroundPanel != null && myAliveSprite != null)
        {
            backgroundPanel.sprite = myAliveSprite;
            backgroundPanel.color = Color.white; 
        }
        if (stars != null)
        {
            foreach(var star in stars)
            {
                if(star != null) star.gameObject.SetActive(!isScoreMode);
            }
        }
        if(scoreText  != null)
        {
            scoreText.gameObject.SetActive(isScoreMode);
        }
        if(isScoreMode)
        {
            UpdateScore(initialValue);
        }
        else
        {
            UpdateStars(initialValue);
        }
    }

    public void UpdateStars(int score)
    {
        if (stars != null) return;

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].color = (i < score) ? starOnColor : starOffColor;
        }
    }

    public void UpdateScore(int score)
    {
        if(scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public void SetEliminated(bool isDead)
    {
        backgroundPanel.sprite = isDead ? myDeadSprite : myAliveSprite;
    }
}