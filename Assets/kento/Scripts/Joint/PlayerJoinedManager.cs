using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;

//using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerJoinedManager : MonoBehaviour
{
    [SerializeField] private InputAction joinAction = default;  //参加するときの入力
    [SerializeField] private InputAction leaveAction = default;  //参加するときの入力

    [SerializeField] private int maxPlayers = 4;　　　　　　　　//参加上限
    //----------
    [SerializeField] private Text device1text;                  //1デバイス名Text
    [SerializeField] private Text device2text;　　　　　　　　　//2デバイス名Text
    [SerializeField] private Text device3text;　　　　　　　　　//3デバイス名Text
    [SerializeField] private Text device4text;　　　　　　　　　//4デバイス名Text



    private List<InputDevice> joinDevices = new List<InputDevice>();             //参加中のデバイス

    [SerializeField] private RectTransform root;
    [SerializeField] private VirtualMouseInput[] cursorPrefabs;
    private readonly List<PlayerCursor> cursors = new();

   

    private void Awake()
    {
        //最大参加可能数で配列を初期化
        joinDevices = new List<InputDevice>(maxPlayers);
        // InputActionを有効化し、コールバックを設定
        joinAction.Enable();
        joinAction.performed += OnJoin;

        leaveAction.Enable();
        leaveAction.performed += OnLeave;

        //-----Text非表示-----
        device1text.enabled = false;
        device2text.enabled = false;
        device3text.enabled = false;
        device4text.enabled = false;
    }


    private void OnDestroy()
    {
        joinAction.performed -= OnJoin;
        joinAction.Disable();

        leaveAction.performed -= OnLeave;
        leaveAction.Disable();
    }

    private void OnJoin(InputAction.CallbackContext context)
    {
        //現在の参加数がＭａｘならreturn
        if (joinDevices.Count >= maxPlayers) { return; }

        //押されたデバイスを取得
        var device = context.control.device;
        if (joinDevices.Contains(device)) { return; }
        
     /*   // カーソルの生成
        int i = cursors.Count;
        var cursor = Instantiate(cursorPrefabs[i], root);
        cursor.name = $"Cursor#{i + 1}";

        var playerInput = cursor.GetComponent<PlayerInput>();
        playerInput.neverAutoSwitchControlSchemes = true;
        playerInput.SwitchCurrentControlScheme(device);

        // Player2以降のカーソルは入力を無効化する
        if (i >= 1)
        {
            cursor.enabled = false;
            playerInput.enabled = false;
        }
        // カーソルを管理リストに追加
        cursors.Add(new PlayerCursor
        {
            device = device,
            curdor = cursor
        });*/
        joinDevices.Add(device);
        UpdateDeviceTexts();
    }

    void OnLeave(InputAction.CallbackContext context)
    {
        var device = context.control.device;
        var player = cursors.Find(p => p.device == device);
        if (player == null) return;

        cursors.Remove(player);
        joinDevices.Remove(device);
        Destroy(player.curdor.gameObject);

        UpdateDeviceTexts();
        /* // カーソルを管理リストから削除
         var playerIndex = joinDevices.Count;
         // 生成されたカーソル取得
         var cursor = cursors.Find(c => c != null && c.name == $"Cursor#{playerIndex}");
         if (cursor == null) return;
         // カーソルの削除
         cursors.Remove(cursor);
         Destroy(cursor.gameObject);

         var device = context.control.device;
         if (joinDevices.Remove(device))
         {
             UpdateDeviceTexts();
         }*/
    }

    void UpdateDeviceTexts()
    {
        Text[] texts = { device1text, device2text, device3text, device4text };

        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].enabled = false;
            texts[i].text = "";
        }

        for (int i = 0; i < joinDevices.Count; i++)
        {
            texts[i].enabled = true;
            texts[i].text = $"Player {i + 1}: {joinDevices[i].displayName}";
        }
    }

    void CreateCursor(InputDevice device)
    {
       
    }


    //StartButtonが押されたときのScene移行
    public void OnGameStarte()
    {
        PlayerDataHolder.Instance.SetDevices(joinDevices.ToArray(), joinDevices.Count);
        SceneManager.LoadScene("prot");
    }

}
