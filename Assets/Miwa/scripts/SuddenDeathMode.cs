using UnityEngine;
using System.Reflection;
using static GameMode;

public class SuddenDeathMode : IGameMode
{
    public void OnEnter()
    {
        // 攻撃力を爆上げ（10倍）にする
        ApplyPowerUp(true);

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

    public void PowerUpSinglePlayer(GameObject player)
    {
        var controller = player.GetComponent<PlayerController1>();
        if (controller != null)
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;
            FieldInfo strongField = typeof(PlayerController1).GetField("StrongKnockbackForce", flags);
            FieldInfo weakField = typeof(PlayerController1).GetField("WeakKnockbackForce", flags);

            if (strongField != null)
            {
                float val = (float)strongField.GetValue(controller);
                strongField.SetValue(controller, val * 10.0f);
            }
            if (weakField != null)
            {
                float val = (float)weakField.GetValue(controller);
                weakField.SetValue(controller, val * 10.0f);
            }
        }
    }
}