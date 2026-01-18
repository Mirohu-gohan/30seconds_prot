using UnityEngine;
using System.Reflection;
using static GameMode;

public class SuddenDeathMode : IGameMode
{
    public void OnEnter()
    {
        // 攻撃力を爆上げ（10倍）にする
        ApplyPowerUp(true);
        Debug.Log("<color=red>サドンデス開始：全プレイヤーの攻撃力を10倍に設定しました！</color>");

        //UIの表示
        if (GameManager_M.Instance != null && GameManager_M.Instance.suddenDeathUI != null)
        {
            GameManager_M.Instance.suddenDeathUI.SetActive(true);
        }
    }

    public void OnUpdate() { }

    public void OnExit()
    {
        // モード終了時に元に戻す（1/10倍にする）
        ApplyPowerUp(false);

        //UIの非表示
        if (GameManager_M.Instance != null && GameManager_M.Instance.suddenDeathUI != null)
        {
            GameManager_M.Instance.suddenDeathUI.SetActive(false);
        }
    }

    private void ApplyPowerUp(bool enable)
    {
        if (GameManager_M.Instance == null) return;

        foreach (var player in GameManager_M.Instance.GetActivePlayers())
        {
            if (player == null) continue;

            // 1. スクリプト「PlayerController1」を取得
            var controller = player.GetComponent<PlayerController1>();
            if (controller != null)
            {
                // 2. リフレクションを使って private な変数を無理やり特定する
                // BindingFlags.NonPublic を指定することで private 変数にアクセス可能
                BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;

                FieldInfo strongField = typeof(PlayerController1).GetField("StrongKnockbackForce", flags);
                FieldInfo weakField = typeof(PlayerController1).GetField("WeakKnockbackForce", flags);

                // 3. 値を書き換える（enableがtrueなら10倍、falseなら元の数値に戻す）
                float multiplier = enable ? 10.0f : 0.1f;

                if (strongField != null)
                {
                    float currentVal = (float)strongField.GetValue(controller);
                    strongField.SetValue(controller, currentVal * multiplier);
                }

                if (weakField != null)
                {
                    float currentVal = (float)weakField.GetValue(controller);
                    weakField.SetValue(controller, currentVal * multiplier);
                }
            }
        }
    }
}