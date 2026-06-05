using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEditor;

public class GamePadCursorController : MonoBehaviour
{
    [Header("カーソル設定")]
    [SerializeField] private RectTransform cursorRectTransform;
    [SerializeField] private Canvas canvas;
    [SerializeField] private float cursorSpeed = 100.0f;

    [Header("1P DeviceSettings")]
    [Tooltip("0 = 最初に接続されたゲームパッド(1P)")]
    [SerializeField] private int player1GamepadIndex = 0;

    private RectTransform canvasRect;
    private GameObject currentHovered;
    private Gamepad player1Gamepad;

    private enum InputMode { Gamepad, MouseKeyboard }
    private InputMode currentInputMode = InputMode.Gamepad;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasRect = canvas.GetComponent<RectTransform>();
        RefreshPlayer1Gamepad();

        //コントローラーの抜き差し対策
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        //接続or切断があれば1Pデバイスを更新
        if (device is Gamepad) RefreshPlayer1Gamepad();
    }

    void RefreshPlayer1Gamepad()
    {
        var gamepads = Gamepad.all;
        player1Gamepad = gamepads.Count > player1GamepadIndex ? gamepads[player1GamepadIndex] : null;
    }

    // Update is called once per frame
    void Update()
    {
        if (player1Gamepad == null) return;

        DetectInputMode();

        if (currentInputMode ==InputMode.MouseKeyboard)
        {
            if (currentHovered != null)
            {
                Debug.Log($"マウスモード移行時リセット: {currentHovered.name}"); // ← 追加
                PointerEventData pData = new PointerEventData(EventSystem.current);
                ExecuteEvents.ExecuteHierarchy(currentHovered, pData, ExecuteEvents.pointerExitHandler);
                currentHovered = null;
            }
        }

        MoveCursor();
        HandleHover();
        HandleClick();
    }

    void DetectInputMode()
    {
        //PCmodeに変更
        if (Mouse.current != null && (Mouse.current.delta.ReadValue().magnitude > 0f || Mouse.current.leftButton.wasPressedThisFrame))
        {
            Cursor.visible = true;
            currentInputMode = InputMode.MouseKeyboard;
            cursorRectTransform.gameObject.SetActive(false);
        }

        //Change to CSmode
        if (player1Gamepad != null && (player1Gamepad.leftStick.ReadValue().magnitude > 0.2f || player1Gamepad.buttonSouth.wasPressedThisFrame))
        {
            Cursor.visible = false;
            currentInputMode = InputMode.Gamepad;
            cursorRectTransform.gameObject.SetActive(true);
        }
    }

    void MoveCursor()
    {
        Vector2 input = player1Gamepad.leftStick.ReadValue();

        //DeadZone
        if (input.magnitude < 0.2f) return;

        Vector2 newPos = cursorRectTransform.anchoredPosition + input * cursorSpeed * Time.deltaTime;

        //Canvas内にクランプ
        Vector2 half = canvasRect.sizeDelta * 0.5f;
        newPos.x = Mathf.Clamp(newPos.x, -half.x, half.x);
        newPos.y = Mathf.Clamp(newPos.y, -half.y, half.y);

        cursorRectTransform.anchoredPosition = newPos;
    }

    void HandleHover()
    {
        PointerEventData pData = new PointerEventData(EventSystem.current)
        {
            position = GetCursorScreenPosition()
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pData, results);

        GameObject newHovered = results.Count > 0 ? results[0].gameObject : null;

        if (newHovered != currentHovered)
        {
            if (currentHovered != null)
                ExecuteEvents.ExecuteHierarchy(currentHovered, pData, ExecuteEvents.pointerExitHandler);

            if (newHovered != null) ExecuteEvents.ExecuteHierarchy(newHovered, pData, ExecuteEvents.pointerEnterHandler);

            currentHovered = newHovered;
        }
    }

    void HandleClick()
    {
        if (!player1Gamepad.buttonSouth.wasPressedThisFrame)
        {
            return;
        }
        else
        {
            Debug.Log("Abutton");
        }

        Debug.Log($"currentHovered: {currentHovered?.name ?? "null"}"); // ← 追加

        if (currentHovered == null) return;

        PointerEventData pData = new PointerEventData(EventSystem.current)
        {
            position = GetCursorScreenPosition()
        };

        ExecuteEvents.ExecuteHierarchy(currentHovered, pData, ExecuteEvents.pointerClickHandler);
    }

    Vector2 GetCursorScreenPosition()
    {
        Vector3[] corners = new Vector3[4];
        canvasRect.GetWorldCorners(corners);
        //corners[0] = 左下
        //corners[2] = 右上

        float tx = (cursorRectTransform.anchoredPosition.x + canvasRect.sizeDelta.x * 0.5f) / canvasRect.sizeDelta.x;

        float ty = (cursorRectTransform.anchoredPosition.y + canvasRect.sizeDelta.y * 0.5f) / canvasRect.sizeDelta.y;

        float sx = Mathf.Lerp(corners[0].x, corners[2].x, tx);
        float sy = Mathf.Lerp(corners[0].y, corners[2].y, ty);

        return RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, new Vector3(sx, sy, 0f));
    }
}
