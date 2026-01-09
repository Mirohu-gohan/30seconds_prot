using UnityEngine;

public class AddHitEffect : MonoBehaviour
{
    PlayerController PLcon;

    [Header("エフェクト設定")]
    [SerializeField] private GameObject particleObject; // Scene上のParticle
    private ParticleSystem particle;

    void Start()
    {
        PLcon = GetComponent<PlayerController>();

        if (PLcon == null)
            Debug.LogError("PlayerControllerが見つかりません");

        if (particleObject == null)
            Debug.LogError("particleObject が Inspector に設定されていません");

        particle = particleObject.GetComponent<ParticleSystem>();

        particleObject.SetActive(false); // 最初は非表示
    }

    void Update()
    {
        if (PLcon == null || particleObject == null) return;

        if (PLcon.isPrese)
        {
            if (!particleObject.activeSelf)
            {
                particleObject.SetActive(true);
                particle.Play(); // 再生
            }
        }
        else
        {
            if (particleObject.activeSelf)
            {
                particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                particleObject.SetActive(false);
            }
        }
    }
}
