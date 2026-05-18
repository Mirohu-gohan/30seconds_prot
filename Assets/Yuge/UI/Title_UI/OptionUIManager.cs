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
}

public class OptionUIManager : MonoBehaviour
{
    [Header("複数UIパネルの設定")]
    [SerializeField] private List<PanelData> panels = new List<PanelData>();

    [Tooltip("閉じるアニメーションの秒数")]
    [SerializeField] private float closeAnimationDuration = 0.5f;

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

        // --- 追加：無理やりフォーカスを当てる ---
        StartCoroutine(ForceFocusByNameRoutine());
    }


    private IEnumerator ForceFocusByNameRoutine()
    {
        // 1. シーン遷移とパネル初期化が完全に終わるまで少し長めに待つ
        yield return new WaitForSecondsRealtime(0.2f);

        if (EventSystem.current != null)
        {
            // ★ヒエラルキーで実際に動いているボタンの名前に書き換えてください
            // "Option_BT" なのか "StartButton" なのか、今一度確認！
            string buttonName = "StartButton";
            GameObject target = GameObject.Find(buttonName);

            if (target != null)
            {
                // 2. ボタンを見つけたら、そのボタンが「押せる状態」か強制的にチェック
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(target);
                Debug.Log($"<color=cyan>[UI Manager]</color> {buttonName} にフォーカスを強制セットしました！");
            }
            else
            {
                // ここでログが出たら、名前が間違っているか、オブジェクトが非表示です
                Debug.LogWarning($"<color=red>[UI Manager]</color> '{buttonName}' が見つかりませんでした！名前を確認してください。");
            }
        }
        else
        {
            Debug.LogError("[UI Manager] EventSystemが見つかりません！");
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
                yield return new WaitForSecondsRealtime(closeAnimationDuration);
            }
            targetData.panel.SetActive(false);
        }

        currentCoroutine = null;
    }
}