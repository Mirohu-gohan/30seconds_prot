using UnityEngine;

public class IGameMode : MonoBehaviour
{
    public interface Igamemode
    {
        //モード開始時の初期の設定
        void OnEnter();
        //モード実行中の毎フレーム処理
        void OnUpdate();
        //モード終了時のクリーンアップ
        void OnExit();

    }
}
