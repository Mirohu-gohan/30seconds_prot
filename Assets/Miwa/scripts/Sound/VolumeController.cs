using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{

    public Slider bgmSlider;
    public Slider seSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(SoundManager.Instance != null)
        {
            bgmSlider.value = SoundManager.Instance.bgmVolume;
            seSlider.value = SoundManager.Instance.seVolume;

            bgmSlider.onValueChanged.AddListener(val => SoundManager.Instance.SEtBGMVolume(val));
            seSlider.onValueChanged.AddListener(val => SoundManager.Instance.SetSEVolume(val));
        }
    }
}
