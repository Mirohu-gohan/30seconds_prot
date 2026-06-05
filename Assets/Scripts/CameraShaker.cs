using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance { get; private set; }

    [Header("カメラの振動パラメータ")]
    [SerializeField] private float defaultDuration = 0.3f;
    [SerializeField] private float defaultMagnitude = 0.2f;

    void Awake()
    {
        Instance = this;
    }

    public void Shake()
    {
        StartCoroutine(ShakeRoutine(defaultDuration, defaultMagnitude));
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        Vector3 oriPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = 
                new Vector3(oriPos.x + x, 
                            oriPos.y + y,
                            oriPos.z + x
                            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = oriPos;
    }
}
