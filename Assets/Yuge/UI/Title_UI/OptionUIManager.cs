using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    [Header("コントローラー初期フォーカスボタン")]
    public GameObject firstSelectedButton;
}

public class OptionUIManager : MonoBehaviour
{
    [Header("ゲーム起動時に最初に選択させたいメインボタン")]
    [SerializeField] private GameObject mainFirstSelectedButton;

    [Header("複数UIパネルの設定")]
    [SerializeField] private List<PanelData> panels = new List<PanelData>();

    [Tooltip("閉じるアニメーションの秒数")]
    [SerializeField] private float closeAnimationDuration = 0.5f;

    [Header("ゲーム起動時に自動で開くパネルの番号（開かない場合は -1）")]
    [SerializeField] private int defaultOpenPanelIndex = -1;

    private Coroutine currentCoroutine = null;

    private void OnEnable()
    {
        Time.timeScale = 1f;

        foreach (var data in panels)
        {
            if (data.panel != null) data.panel.SetActive(false);
            if (data.animator != null)
            {
                data.animator.keepAnimatorStateOnDisable = false;
                if (!string.IsNullOrEmpty(data.parameterName))
                    data.animator.SetBool(data.parameterName, false);
                data.animator.Play("Idle", 0, 0f);
            }
        }

        if (mainFirstSelectedButton != null)
        {
            StartCoroutine(FocusMainButtonRoutine());
        }

        if (defaultOpenPanelIndex >= 0 && defaultOpenPanelIndex < panels.Count)
        {
            ToggleOptionPanel(defaultOpenPanelIndex);
        }
    }


    private IEnumerator FocusMainButtonRoutine()
    {
        // 起動直後のEventSystemのバタつきを避けるため、ほんの少し（0.05秒）だけ待つ
        yield return new WaitForSecondsRealtime(0.05f);

        if (EventSystem.current != null && mainFirstSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null); // 一旦クリア
            EventSystem.current.SetSelectedGameObject(mainFirstSelectedButton);
            Debug.Log($"<color=green>[UI Manager]</color> メインボタン '{mainFirstSelectedButton.name}' に初期フォーカスを完了しました！");
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

            if (targetData.firstSelectedButton != null && EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null); // 一旦クリア
                EventSystem.current.SetSelectedGameObject(targetData.firstSelectedButton);
            }
        }
        else
        {
            if (targetData.animator != null && !string.IsNullOrEmpty(targetData.parameterName))
            {
                targetData.animator.SetBool(targetData.parameterName, false);
                yield return new WaitForSecondsRealtime(closeAnimationDuration);
            }
            targetData.panel.SetActive(false);

            if (mainFirstSelectedButton != null && EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(mainFirstSelectedButton);
            }
        }

        currentCoroutine = null;
    }
}