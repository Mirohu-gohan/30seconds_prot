using UnityEngine;
using System.Collections;


public class KOEffect : MonoBehaviour
{
    [Header("エフェクト")]
    public GameObject koEffect;

    [Header("エフェクト調整")]
    public float efsize = 75f;
    public float efOffset = 10f; //内側にずらす距離

    [Header("HitStop")]
    public float freezeDuration = 0.08f;

    private Rigidbody rb;
    private bool isKO = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckBlastZone();
    }

    public void CheckBlastZone()
    {
        if (isKO) return;

        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);

        Debug.Log($"screenPos: {screenPos}");


        bool outOfScreen =
                screenPos.x < -0.1f || screenPos.x > 1.1f ||
                screenPos.y < -0.1f || screenPos.y > 1.1f;

        if (outOfScreen)
        {
            StartCoroutine(KOSequence(screenPos));
        }
    }

    IEnumerator KOSequence(Vector3 Pos)
    {
        isKO = true;

        Vector3 flyDirection = rb.linearVelocity.normalized;

        //HitStop
        Vector3 savedVelocity = rb.linearVelocity;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
        yield return new WaitForSeconds(freezeDuration);

        //restart
        rb.isKinematic = false;
        rb.linearVelocity = savedVelocity;

        //3.PlayeEffect
        Vector3 screenEdgePos = GetScreenEdgePosition(flyDirection);
        SpawnKOEffect(flyDirection, Pos, screenEdgePos);

        //4.one sec
        yield return new WaitForSeconds(1.5f);
    }

    // 画面端の座標を取得
    Vector3 GetScreenEdgePosition(Vector3 flyDirection)
    {
        // キャラが画面外に出る直前のViewport座標をワールド座標に変換
        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);

        // 画面端にクランプ
        screenPos.x = Mathf.Clamp(screenPos.x, 0.05f, 0.95f);
        screenPos.y = Mathf.Clamp(screenPos.y, 0.05f, 0.95f);

        return Camera.main.ViewportToWorldPoint(screenPos);
    }

    public void SpawnKOEffect(Vector3 flyDirection,Vector3 Pos, Vector3 spwanPos)
    {
        if (koEffect != null)
        {
            //プレイヤーの位置から吹っ飛び方向と逆に少しずらす
            Vector3 spawnPos = transform.position - flyDirection * efOffset;

            GameObject fx = Instantiate(koEffect, spawnPos, Quaternion.identity);
            fx.transform.localScale = new Vector3(efsize, efsize, efsize);

            float angle = Mathf.Atan2(flyDirection.z, flyDirection.x) * Mathf.Rad2Deg;

            if (Pos.x < -0.1f)
            {
                fx.transform.rotation = Quaternion.Euler(4f, 0f, angle + 180f);
            }
            else if(Pos.x > 1.1f)
            {
                fx.transform.rotation = Quaternion.Euler(4f, 0f, angle);
            }

            if (Pos.y  > 1.1f)
            {
                fx.transform.rotation = Quaternion.Euler(4f, 0f, 90f);
            }
            else if(Pos.y < -0.1f)
            {
                fx.transform.rotation = Quaternion.Euler(4f, 0f, 270f);
            }

            Debug.Log($"flyDirection: {flyDirection}");
            Debug.Log($"spawnPos: {spawnPos}");
            Debug.Log($"キャラの現在位置: {transform.position}");

            Destroy(fx, 3f);
        }
    }
}
