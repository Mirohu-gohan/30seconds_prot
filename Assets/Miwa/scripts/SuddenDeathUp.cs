using UnityEngine;
using System.Reflection;

public class SuddenDeathPowerUp : MonoBehaviour
{
    void Start()
    {
        //サドンデスモードかどうかチェック
        if (GameManager_M.Instance != null &&
            GameManager_M.Instance.CurrentModeState == GameManager_M.Mode.SuddenDeath)
        {
            ApplyHack();
        }
    }

    private void ApplyHack()
    {
        //自分についている「PlayerController1」を探す
        var controller = GetComponent<PlayerController1>();
        if (controller == null) return;

        //リフレクションで強引に数値を書き換える
        // (相手のコードが private [SerializeField] でも無視して上書きします)
        BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;

        FieldInfo strongField = typeof(PlayerController1).GetField("StrongKnockbackForce", flags);
        FieldInfo weakField = typeof(PlayerController1).GetField("WeakKnockbackForce", flags);

        // ★ここで倍率を設定（10倍！）
        float multiplier = 10.0f;
        

        if (strongField != null)
        {
            float val = (float)strongField.GetValue(controller);
            strongField.SetValue(controller, val * multiplier);
        }
        if (weakField != null)
        {
            float val = (float)weakField.GetValue(controller);
            weakField.SetValue(controller, val * multiplier);
        }

        Debug.Log($"<color=red>【サドンデス発動】{gameObject.name} の威力を10倍にハックしました！</color>");
    }
}
