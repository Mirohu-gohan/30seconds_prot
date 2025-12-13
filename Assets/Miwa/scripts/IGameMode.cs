using UnityEngine;

public class IGameMode : MonoBehaviour
{
    public interface IGamemode
    {
        void OnEnter();  // モード開始時の初期設定
        void OnUpdate(); // モード実行中の毎フレーム処理
        void OnExit();   // モード終了時のクリーンアップ
    }
}
