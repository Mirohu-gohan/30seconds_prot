using UnityEngine;

/// <summary>
/// 隕石の着弾予定地点に警告サークルを表示するコンポーネント。
/// ObjectSpawner から AddComponent で動的にアタッチされる。
/// 設定不要 - 地面の検出・マテリアル生成をすべて自動で行う。
/// </summary>
public class MeteorLandingIndicator : MonoBehaviour
{
    private GameObject indicator;
    private SphereCollider sphereCollider;

    /// <summary>
    /// スポーン直後に ObjectSpawner から呼び出す。設定は不要。
    /// </summary>
    /// <param name="indicatorPrefab">カスタム円プレハブ（null で自動生成）</param>
    /// <param name="sizeMultiplier">サイズの倍率（1.0 = 隕石の直径に一致）</param>
    public void Initialize(GameObject indicatorPrefab, float sizeMultiplier)
    {
        sphereCollider = GetComponent<SphereCollider>();

        // "Stage" タグで地面を自動検索
        if (!TryFindGroundPosition(out Vector3 groundPos)) return;

        // インジケーター生成（プレハブがなければ自動生成）
        indicator = indicatorPrefab != null
            ? Instantiate(indicatorPrefab, groundPos, Quaternion.identity)
            : CreateDefaultIndicator(groundPos);

        // 隕石のワールド半径からサイズを計算してセット
        float worldRadius = sphereCollider != null
            ? sphereCollider.radius * transform.lossyScale.x
            : transform.lossyScale.x;
        float size = worldRadius * 2f * sizeMultiplier;

        Vector3 s = indicator.transform.localScale;
        indicator.transform.localScale = new Vector3(size, s.y, size);
        indicator.transform.position = groundPos + Vector3.up * 0.02f;
    }

    // "Stage" タグのコライダーを Raycast で自動検索
    private bool TryFindGroundPosition(out Vector3 groundPos)
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down, Mathf.Infinity);
        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Stage"))
            {
                groundPos = hit.point;
                return true;
            }
        }
        groundPos = Vector3.zero;
        return false;
    }

    // プレハブ未設定時に使う半透明オレンジの薄い円板を自動生成
    private GameObject CreateDefaultIndicator(Vector3 position)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        obj.transform.position = position;
        obj.transform.localScale = new Vector3(1f, 0.02f, 1f); // 極薄フラット

        // 物理判定は不要
        Destroy(obj.GetComponent<Collider>());

        // 半透明オレンジのマテリアルを自動生成
        Renderer r = obj.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = new Color(1f, 0.35f, 0f, 0.55f);
        r.material = mat;

        return obj;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Stage"))
            DestroyIndicator();
    }

    private void OnDestroy()
    {
        DestroyIndicator();
    }

    private void DestroyIndicator()
    {
        if (indicator != null)
        {
            Destroy(indicator);
            indicator = null;
        }
    }
}
