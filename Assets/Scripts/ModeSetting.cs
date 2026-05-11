using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModeSetting : MonoBehaviour
{
    public enum GameMode { Survival, Score}
    public GameMode currentMode = 0;

    public TMP_Text modeName;
    public TMP_Text modeDescription;

    public StageSetting Ss;

    public void Awake()
    {
        UpdateModeText();
    }

    public void NextMode()
    {
        currentMode = (GameMode)(((int)currentMode + 1) % 2);
        UpdateModeText();
    }

    public void PreviousMode()
    {
        currentMode = (GameMode)(((int)currentMode + 1) % 2);
        UpdateModeText();
    }

    public void UpdateModeText()
    {
        switch(currentMode)
        {
            case GameMode.Survival: 
                modeName.text = "対戦";
                modeDescription.text = "対戦モードの説明";
                
                break;

            case GameMode.Score:    
                modeName.text = "スコア";
                modeDescription.text = "スコアモードの説明";

                break;
        }

        if (Ss != null)
        {
            int newLength = (currentMode == GameMode.Survival) ? Ss.Survive_stages.Length : Ss.Score_stages.Length;
            Ss.currentIndex = Mathf.Min(Ss.currentIndex, newLength - 1);
            Ss.UpdatePreview();
        }
    }
}
