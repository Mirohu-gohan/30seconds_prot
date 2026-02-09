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

/*    //カーソル
    [SerializeField] private Canvas uiCanv;
    [SerializeField] private GameObject cursors;
    private InputDevice mindevice;
    private GameObject cursorInstance;*/

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

       /* if (mindevice != null) return;
        mindevice = context.control.device;
        int i = joinDevices.Count;
        cursorInstance = Instantiate(cursors, uiCanv.transform, false);
        cursorInstance.GetComponent<CursorController>().controlDevice = mindevice;*/

        joinDevices.Add(device);
        UpdateDeviceTexts();
    }

    void OnLeave(InputAction.CallbackContext context)
    {
        var device = context.control.device;

        if (context.control.device != device) return;

        /*Destroy(cursorInstance);
        cursorInstance = null;
        mindevice = null;*/

        joinDevices.Clear();
        //joinDevices.Remove(device);
        UpdateDeviceTexts();
       
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
            //texts[i].text = $"Player {i + 1}: {joinDevices[i].displayName}\n参加中";
            texts[i].text = $"{joinDevices[i].displayName}\n参加中";
        }
    }


    //StartButtonが押されたときのScene移行
    public void OnGameStarte()
    {
        PlayerDataHolder.Instance.SetDevices(joinDevices.ToArray(), joinDevices.Count);
        SceneManager.LoadScene("prot");
    }

}
