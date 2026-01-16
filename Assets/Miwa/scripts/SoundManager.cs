using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("オーディオソース（スピーカー）")]
    public AudioSource bgmSource; // BGM用
    public AudioSource seSource;  // SE用

    [Header("登録したいSEリスト")]
    public AudioClip meteorSE;    // 隕石の音
    public AudioClip playerFallSE; // 落下音
    //もしほかにも新規で登録したい場合はここに追記するであります！

    void Awake()
    {
        // シングルトン化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーン移動しても壊さないであります！
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    // --- BGMを鳴らす機能 ---
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;

        // すでに同じ曲が流れていたら何もしないであります！
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.Play();
    }

    // --- SEを鳴らす基本機能 ---
    public void PlaySE(AudioClip clip)
    {
        if (clip != null)
        {
            seSource.PlayOneShot(clip);
        }
    }

    // --- 専用関数 ---
    
    //（今後、音量やピッチを変えたくなったらここだけいじればいいよー）
    // 隕石の音
    public void PlayMeteorSound()
    {
        if (meteorSE != null)
        {
            // 例としては少し音を大きくしたいなら第2引数に数値を入れるであります！
            seSource.PlayOneShot(meteorSE, 1.2f); 
        }
    }

    // 落下の音
    public void PlayFallSound()
    {
        if (playerFallSE != null)
        {
            seSource.PlayOneShot(playerFallSE);
        }
    }
}