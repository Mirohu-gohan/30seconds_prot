using System.Collections;
using UnityEngine;

public class OptionUIManager : MonoBehaviour
{
    [Header("UI設定")]
    [Tooltip("表示・非表示を切り替えるOptionのパネル")]
    [SerializeField] private GameObject optionPanel;

    [Tooltip("パネルについているAnimator")]
    [SerializeField] private Animator panelAnimator;

    [Tooltip("閉じるアニメーションの秒数")]
    [SerializeField] private float closeAnimationDuration = 0.5f;

    // Animatorのパラメータ名（文字列のタイポを防ぐためハッシュ化）
    private readonly int IsOpenHash = Animator.StringToHash("IsOpen");

    /// <summary>
    /// Optionボタンが押された時に呼ばれるメソッド
    /// </summary>

    private void Start()
    {
        if (panelAnimator != null)
        {
            // オブジェクトが非アクティブになっても、Animatorの状態（現在地）を記憶する
            panelAnimator.keepAnimatorStateOnDisable = true;
        }
    }

    public void ToggleOptionPanel()
    {
        // 現在パネルがアクティブかどうかを取得
        bool isCurrentlyActive = optionPanel.activeSelf;

        if (!isCurrentlyActive)
        {
            // --- 開く時の処理 ---
            optionPanel.SetActive(true); // まずアクティブにする
            if (panelAnimator != null)
            {
                panelAnimator.SetBool(IsOpenHash, true); // 開くアニメーションを再生
            }
        }
        else
        {
            // --- 閉じる時の処理 ---
            if (panelAnimator != null)
            {
                panelAnimator.SetBool(IsOpenHash, false); // 閉じるアニメーションを再生
                StartCoroutine(DeactivateAfterAnimation()); // アニメーション終了を待つ
            }
            else
            {
                // アニメーターが設定されていない場合は即座に閉じる
                optionPanel.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 閉じるアニメーションの再生が終わってから非アクティブにするコルーチン
    /// </summary>
    private IEnumerator DeactivateAfterAnimation()
    {
        // アニメーションが完了するまで待機
        yield return new WaitForSeconds(closeAnimationDuration);

        // 待機後に非アクティブにする
        optionPanel.SetActive(false);
    }
}