using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class Command : MonoBehaviour
{
    public GameObject Noukin;
    public GameObject message;
    private int cmdSeq = 0;

    // Input Systemのキー定義
    private readonly Key[] CommandKeys = new Key[]
    {
        Key.UpArrow,
        Key.UpArrow,
        Key.DownArrow,
        Key.DownArrow,
        Key.LeftArrow,
        Key.RightArrow,
        Key.LeftArrow,
        Key.RightArrow,
        Key.B,
        Key.A
    };

    // UIの入力を優先するかどうか
    [SerializeField] private bool ignoreWhenUIFocused = false;

    void Start()
    {
        Noukin.SetActive(false);
        message.SetActive(false);
    }

    void Update()
    {
        // UIにフォーカスがある場合、オプションで無視できる
        if (ignoreWhenUIFocused && EventSystem.current != null &&
            EventSystem.current.currentSelectedGameObject != null)
        {
            return;
        }

        // キーボード全体をチェック
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            // 何かキーが押されたかチェック
            if (keyboard.anyKey.wasPressedThisFrame)
            {
                // コマンドキーのみをチェック
                foreach (Key key in CommandKeys)
                {
                    if (keyboard[key].wasPressedThisFrame)
                    {
                        CheckCommand(key);
                        break;
                    }
                }

                // その他のキーが押されたらリセット
                bool isCommandKey = false;
                foreach (Key key in CommandKeys)
                {
                    if (keyboard[key].wasPressedThisFrame)
                    {
                        isCommandKey = true;
                        break;
                    }
                }

                if (!isCommandKey)
                {
                    cmdSeq = 0;
                }
            }
        }
    }

    private void CheckCommand(Key key)
    {
        if (key == CommandKeys[cmdSeq])
        {
            cmdSeq++;

            if (cmdSeq >= CommandKeys.Length)
            {
                ActivateHiddenContent();
                cmdSeq = 0;
            }
        }
        else
        {
            cmdSeq = 0;
        }
    }

    private void ActivateHiddenContent()
    {
        Noukin.SetActive(true);
    }

    // 外部からリセットできるメソッド
    public void ResetCommand()
    {
        cmdSeq = 0;
    }
}