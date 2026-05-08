using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class StageSetting : MonoBehaviour
{
    public ModeSetting Ms;

    public Text stageName;
    public Text stageDescriptionText;
    public Image stagePreviewImage;
    public Stage[] Survive_stages;
    public Stage[] Score_stages;

    public int currentIndex = 0;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Inspector‚ÅSceneAsset‚ðƒZƒbƒg‚µ‚½‚Æ‚«Ž©“®‚ÅsceneName‚É“]ŽÊ
        foreach (var s in Survive_stages)
            if (s.sceneAsset != null)
                s.sceneName = s.sceneAsset.name;

        foreach (var s in Score_stages)
            if (s.sceneAsset != null)
                s.sceneName = s.sceneAsset.name;
    }
#endif

    private void Start()
    {
        currentIndex = 0;
        UpdatePreview();
    }

    public void NextStage()
    {
        if (Ms.isMode == 1)
        {
            currentIndex = (currentIndex + 1) % Survive_stages.Length;
            UpdatePreview();
        }
        else if (Ms.isMode == 2)
        {
            currentIndex = ( currentIndex + 1 ) % Score_stages.Length;
            UpdatePreview();
        }
            
    }

    public void PreviousStage()
    {
        if (Ms.isMode == 1)
        {
            currentIndex = (currentIndex + Survive_stages.Length - 1) % Survive_stages.Length;
            UpdatePreview();
        }
        else if(Ms.isMode == 2)
        {
            currentIndex = (currentIndex + Score_stages.Length - 1) % Score_stages.Length;
            UpdatePreview();
        }
    }

    public void UpdatePreview()
    {
        if (Ms.isMode == 1)
        {
            stageName.text              = Survive_stages[currentIndex].stageName;
            stagePreviewImage.sprite    = Survive_stages[currentIndex].previewSprite;
            stageDescriptionText.text   = Survive_stages[currentIndex].description;
        }
        else if(Ms.isMode == 2)
        {
            stageName.text              = Score_stages[currentIndex].stageName;
            stagePreviewImage.sprite    = Score_stages[currentIndex].previewSprite;
            stageDescriptionText.text   = Score_stages[currentIndex].description;
        }
    }
}
