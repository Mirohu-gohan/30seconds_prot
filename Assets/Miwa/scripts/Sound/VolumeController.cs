using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider seSlider;

    public TMP_Text bgmText;
    public TMP_Text seText;

    void Start()
    {
        if (SoundManager.Instance != null)
        {
            // 最初に現在の音量をスライダーとテキストに反映
            bgmSlider.value = SoundManager.Instance.bgmVolume;
            seSlider.value = SoundManager.Instance.seVolume;

            UpdateBgmtext(bgmSlider.value);
            UpdateSetext(seSlider.value);

            // スライダーを動かした時の処理を登録
            bgmSlider.onValueChanged.AddListener(val => 
            {
                SoundManager.Instance.SEtBGMVolume(val); // 音量を変える
                UpdateBgmtext(val);                     // テキストを変える（←ここが抜けていました）
            });

            seSlider.onValueChanged.AddListener(val => 
            {
                SoundManager.Instance.SetSEVolume(val);  // 音量を変える
                UpdateSetext(val);                      // テキストを変える（←ここが抜けていました）
            });
        }
    }

    void UpdateBgmtext(float value)
    {
        if (bgmText != null)
        {
            bgmText.text = Mathf.RoundToInt(value * 100).ToString();
        }
    }

    void UpdateSetext(float value)
    {
        if (seText != null)
        {
            seText.text = Mathf.RoundToInt(value * 100).ToString();
        }
    }
}
