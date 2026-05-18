using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JoineManager : MonoBehaviour
{
    [SerializeField] private InputAction joinAction = default;  //参加するときの入力
    [SerializeField] private InputAction leaveAction = default;  //参加するときの入力
    [SerializeField] private InputAction startAction = default;

    [SerializeField] private int maxPlayers = 4;        //参加上限

    //----------
    [SerializeField] private Text device1text;         //1デバイス名Text
    [SerializeField] private Text device2text;         //2デバイス名Text
    [SerializeField] private Text device3text;         //3デバイス名Text
    [SerializeField] private Text device4text;         //4デバイス名Text

    private Dictionary<InputDevice, int> playerMap = new();
    private List<InputDevice> joinDevices = new List<InputDevice>();             //参加中のデバイス

    private void Awake()
    {
        //最大参加可能数で配列を初期化
        joinDevices = new List<InputDevice>(maxPlayers);
        // InputActionを有効化し、コールバックを設定
        joinAction.Enable();
        joinAction.performed += OnJoin;

        leaveAction.Enable();
        leaveAction.performed += OnLeave;

        startAction.Enable();
        startAction.performed += ctx => OnGameStarte(ctx);

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

        startAction.RemoveAllBindingOverrides();
        startAction.Disable();
    }


    //-----参加-----
    private void OnJoin(InputAction.CallbackContext context)
    {
        var device = context.control.device;

        if (playerMap.ContainsKey(device)) return;
        if (joinDevices.Count >= maxPlayers) return;

        int playerIndex = joinDevices.Count;

        joinDevices.Add(device);
        playerMap[device] = playerIndex;

        PlayerDataHolder.Instance.SetDevices(joinDevices);

        UpdateDeviceTexts();

        /* //現在の参加数がＭａｘならreturn
         if (joinDevices.Count >= maxPlayers) { return; }

         //押されたデバイスを取得
         var device = context.control.device;
         //重複参加防止
         if (joinDevices.Contains(device)) { return; }

         //参加中の数
         int i = joinDevices.Count;

         //リストにデバイスの追加
         joinDevices.Add(device);
         PlayerDataHolder.Instance.devices = new List<InputDevice>(joinDevices);
         //PlayerDataHolder.Instance.SetDevices(joinDevices.ToArray(), joinDevices.Count);

         //UIの更新
         UpdateDeviceTexts();*/
    }

    //-----退出-----
    void OnLeave(InputAction.CallbackContext context)
    {
        var device = context.control.device;

        if (!playerMap.ContainsKey(device)) return;

        int index = playerMap[device];

        joinDevices.Remove(device);
        playerMap.Remove(device);

        // ★インデックス再計算（ここが重要）
        RebuildMap();

        PlayerDataHolder.Instance.SetDevices(joinDevices);

        UpdateDeviceTexts();
        /*//入力したデバイスの取得
        var device = context.control.device;
        //Index取得
        int index = joinDevices.IndexOf(device);
        //参加していない場合はreturn
        if (context.control.device != device) return;
        if (index == -1) return;

        //デバイス,カーソルの削除
        *//* joinDevices.RemoveAt(index);
         Destroy(playerCursors[index]);
         //List,PlayerDataの更新
         playerCursors.RemoveAt(index);
         PlayerDataHolder.Instance.SetDevices(joinDevices.ToArray(), joinDevices.Count);*//*

        joinDevices.Clear();
        //UIの更新
        UpdateDeviceTexts();*/
    }
    private void RebuildMap()
    {
        playerMap.Clear();

        for (int i = 0; i < joinDevices.Count; i++)
        {
            playerMap[joinDevices[i]] = i;
        }
    }

    //-----UIの更新-----
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
            texts[i].text = $"{joinDevices[i].displayName}\n参加中";
        }
    }


    //StartButtonが押されたときのScene移行
    public void OnGameStarte(InputAction.CallbackContext context)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "Title" || currentSceneName == "Start")
        {
            startAction.Disable();
            joinAction.Disable();
            leaveAction.Disable();

            //PlayerDataHolder.Instance.SetDevices(joinDevices.ToArray(), joinDevices.Count);
            PlayerDataHolder.Instance.SetDevices(joinDevices);
            SceneManager.LoadScene("prot");
        }
    }
    private void Start()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Title" || currentSceneName == "JoinScene")
        {
            startAction.Enable();
            joinAction.Enable();
            leaveAction.Enable();
        }
    }
}
