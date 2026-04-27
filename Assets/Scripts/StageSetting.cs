using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class StageSetting : MonoBehaviour
{
    public Text stageName;
    public Text stageDescriptionText;
    public Image stagePreviewImage;
    public Stage[] stages;

    private int currentIndex = 0;

    private void Start()
    {
        currentIndex = 0;
        UpdatePreview();
    }

    public void NextStage()
    {
        currentIndex = (currentIndex + 1) % stages.Length;
        UpdatePreview();
    }

    public void PreviousStage()
    {
        currentIndex = (currentIndex + stages.Length - 1) % stages.Length;
        UpdatePreview();
    }

    void UpdatePreview()
    {
        stageName.text = stages[currentIndex].stageName;
        stagePreviewImage.sprite = stages[currentIndex].previewSprite;
        stageDescriptionText.text = stages[currentIndex].description;
    }
}
