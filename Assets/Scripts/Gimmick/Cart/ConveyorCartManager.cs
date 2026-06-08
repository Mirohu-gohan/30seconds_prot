using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// ■ ConveyorCartManager
/// 
/// 複数のカート＋ルートのセットをリストで管理し、
/// ランダムで一つだけをアクティブにして指定秒数後に非アクティブに戻す。
/// caution（DecalProjector）は cart/spline より cautionPreDelay 秒早く表示される。
/// </summary>
public class ConveyorCartManager : MonoBehaviour
{
    [System.Serializable]
    public class CartEntry
    {
        public string label;
        public GameObject cart;
        public GameObject spline;
        public GameObject caution;
    }

    [Header("カート＋ルートのセット一覧")]
    public List<CartEntry> cartEntries = new List<CartEntry>();

    [Header("タイミング設定")]
    [Tooltip("cart/splineがアクティブになっている秒数")]
    public float activeDuration = 3.0f;

    [Tooltip("非アクティブ状態が続く秒数（次の抽選までのインターバル）")]
    public float intervalDuration = 2.0f;

    [Tooltip("ゲーム開始から最初の抽選までの待機秒数")]
    public float initialDelay = 1.0f;

    [Tooltip("cautionをcart/splineより何秒早く表示するか")]
    public float cautionPreDelay = 1.0f;

    private int _activeIndex = -1;

    // ─────────────────────────────────────────
    void Start()
    {
        DeactivateAll();
        StartCoroutine(CyclRoutine());
    }

    // ─────────────────────────────────────────
    IEnumerator CyclRoutine()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            // 次のインデックスを決定してcautionだけ先にON
            int next = GetNextIndex();
            _activeIndex = next;
            ActivateCautionOnly(_activeIndex);

            yield return new WaitForSeconds(cautionPreDelay);

            // cart + spline もON
            ActivateCartAndSpline(_activeIndex);

            yield return new WaitForSeconds(activeDuration);

            // 全部OFF
            DeactivateAll();

            yield return new WaitForSeconds(intervalDuration);
        }
    }

    // ─────────────────────────────────────────
    int GetNextIndex()
    {
        if (cartEntries.Count == 0) return -1;

        int next = _activeIndex;
        if (cartEntries.Count > 1)
        {
            while (next == _activeIndex)
                next = Random.Range(0, cartEntries.Count);
        }
        else
        {
            next = 0;
        }
        return next;
    }

    void ActivateCautionOnly(int index)
    {
        var entry = cartEntries[index];
        if (entry.caution != null)
        {
            entry.caution.SetActive(true);
            var decal = entry.caution.GetComponent<DecalProjector>();
            if (decal != null) decal.enabled = true;
        }
        Debug.Log($"[CartManager] caution表示: {entry.label} (index={index})");
    }

    void ActivateCartAndSpline(int index)
    {
        var entry = cartEntries[index];
        if (entry.cart != null) entry.cart.SetActive(true);
        if (entry.spline != null) entry.spline.SetActive(true);
        Debug.Log($"[CartManager] cart+spline表示: {entry.label} (index={index})");
    }

    void DeactivateAll()
    {
        for (int i = 0; i < cartEntries.Count; i++)
        {
            var entry = cartEntries[i];
            if (entry.cart != null) entry.cart.SetActive(false);
            if (entry.spline != null) entry.spline.SetActive(false);
            if (entry.caution != null)
            {
                entry.caution.SetActive(false);
                var decal = entry.caution.GetComponent<DecalProjector>();
                if (decal != null) decal.enabled = false;
            }
        }
        _activeIndex = -1;
    }

    // ─────────────────────────────────────────
    // 外部API
    public void ActivateEntry(int index)
    {
        DeactivateAll();
        _activeIndex = index;
        ActivateCautionOnly(index);
        ActivateCartAndSpline(index);
    }

    public void ForceNextCycle()
    {
        StopAllCoroutines();
        DeactivateAll();
        StartCoroutine(CyclRoutine());
    }
}