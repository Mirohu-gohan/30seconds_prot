using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PanelData
{
    public string panelName;
    public GameObject panel;
    public Animator animator;

    [Header("このパネル用のアニメーターパラメータ名")]
    public string parameterName;

    // ★追加：インスペクターでチェックを入れられるようにする
    [Header("制限設定")]
    [Tooltip("チェックを入れると、このパネルが開いている間は他のパネルを開けなくなります")]
    public bool isModal;
}

public class OptionUIManager : MonoBehaviour
{
    [Header("複数UIパネルの設定")]
    [SerializeField] private List<PanelData> panels = new List<PanelData>();

    [Tooltip("閉じるアニメーションの秒数")]
    [SerializeField] private float closeAnimationDuration = 0.5f;

    private Coroutine currentCoroutine = null;

    private void Start()
    {
        foreach (var data in panels)
        {
            if (data.animator != null)
            {
                data.animator.keepAnimatorStateOnDisable = true;
            }
        }
    }

    public void ToggleOptionPanel(int panelIndex)
    {
        if (panelIndex < 0 || panelIndex >= panels.Count) return;
        if (currentCoroutine != null) return;

        // ★追加：ブロック機能（モーダルチェック）
        // 現在開いているパネルの中に「isModal = true」のパネルがあるか確認する
        for (int i = 0; i < panels.Count; i++)
        {
            // 「isModalがON」かつ「現在アクティブ(表示中)」かつ「押したボタンがそのパネル自身ではない」場合
            if (panels[i].isModal && panels[i].panel.activeSelf && i != panelIndex)
            {
                // 処理をここで終了し、ボタン入力を完全に無視する
                Debug.Log($"{panels[i].panelName} が開いているため、別のパネルは開けません！");
                return;
            }
        }

        // ブロックされていなければ、通常通りアニメーション処理を開始
        currentCoroutine = StartCoroutine(SwitchPanelSequence(panelIndex));
    }

    private IEnumerator SwitchPanelSequence(int targetIndex)
    {
        // --- ① 古いパネルを閉じる ---
        for (int i = 0; i < panels.Count; i++)
        {
            if (i != targetIndex && panels[i].panel.activeSelf)
            {
                if (panels[i].animator != null && !string.IsNullOrEmpty(panels[i].parameterName))
                {
                    panels[i].animator.SetBool(panels[i].parameterName, false);
                }

                yield return new WaitForSeconds(closeAnimationDuration);
                panels[i].panel.SetActive(false);
            }
        }

        // --- ② 新しいパネルを開く（または閉じる） ---
        PanelData targetData = panels[targetIndex];
        bool isCurrentlyActive = targetData.panel.activeSelf;

        if (!isCurrentlyActive)
        {
            targetData.panel.SetActive(true);
            if (targetData.animator != null && !string.IsNullOrEmpty(targetData.parameterName))
            {
                targetData.animator.SetBool(targetData.parameterName, true);
            }
        }
        else
        {
            if (targetData.animator != null && !string.IsNullOrEmpty(targetData.parameterName))
            {
                targetData.animator.SetBool(targetData.parameterName, false);
                yield return new WaitForSeconds(closeAnimationDuration);
            }
            targetData.panel.SetActive(false);
        }

        currentCoroutine = null;
    }
}