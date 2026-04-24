using UnityEngine;
using UnityEngine.UI;

public class ModeSetting : MonoBehaviour
{
    public enum GameMode { Survival, Score}
    public GameMode currentMode;
    public Text modeName;
    public Text modeDescription;

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

    void UpdateModeText()
    {
        switch(currentMode)
        {
            case GameMode.Survival: 
                modeName.text = "対戦モード";
                modeDescription.text = "対戦モードの説明";
                break;

            case GameMode.Score:    
                modeName.text = "スコアモード";
                modeDescription.text = "スコアモードの説明";
                break;
        }
    }
}
