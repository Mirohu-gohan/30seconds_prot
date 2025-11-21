using UnityEngine;

/// <summary>
/// スポーンされた直後にGameManagerに自分自身を登録するためのスクリプト
/// </summary>
public class PlayerRegistrar : MonoBehaviour
{
    void Start()
    {
        // シーン内のGameManagerを見つける
        GameManager gm = Object.FindFirstObjectByType<GameManager>();

        if (gm != null)
        {
            // GameManagerの監視リストに自分自身を登録
            gm.RegisterPlayer(this.gameObject);
        }
        else
        {
            Debug.LogError("GameManagerが見つかりません。プレイヤーの監視リスト登録が失敗しました。");
        }
    }
}
