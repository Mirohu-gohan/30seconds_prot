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

    private int maxPlayers = 4;        //参加上限

    //----------
    [SerializeField] private Text device1text;         //1デバイス名Text
    [SerializeField] private Text device2text;         //2デバイス名Text
    [SerializeField] private Text device3text;         //3デバイス名Text
    [SerializeField] private Text device4text;         //4デバイス名Text

    private Dictionary<int, int> playerMap = new();
    private List<InputDevice> joinDevices = new List<InputDevice>();             //参加中のデバイス

    private void Awake()
    {
        //最大参加可能数で配列を初期化
        joinDevices = new List<InputDevice>(maxPlayers);
        playerMap = new Dictionary<int, int>(maxPlayers);
        // InputActionを有効化し、コールバックを設定
        joinAction.Enable();
        joinAction.performed += OnJoin;

        leaveAction.Enable();
        leaveAction.performed += OnLeave;

        startAction.Enable();
        startAction.performed += OnGameStarte;

        //-----Text非表示-----
        device1text.enabled = false;
        device2text.enabled = false;
        device3text.enabled = false;
        device4text.enabled = false;
    }


    private void OnDestroy()
    {
        joinAction.performed -= OnJoin;
        leaveAction.performed -= OnLeave;
        startAction.RemoveAllBindingOverrides();
    }


    //-----参加-----
    private void OnJoin(InputAction.CallbackContext context)
    {
        InputDevice device = context.control.device;

        if (joinDevices.Contains(device)) { return; }
        if (joinDevices.Count >= maxPlayers) return;

        joinDevices.Add(device);
        int playerIndex = joinDevices.Count - 1;

        // deviceId → playerIndex
        playerMap[device.deviceId] = playerIndex;
        Debug.Log($"Join : DeviceID {device.deviceId} → Player{playerIndex}");
        UpdateDeviceTexts();
    }

    //-----退出-----
    void OnLeave(InputAction.CallbackContext context)
    {
        InputDevice device = context.control.device;
        if (!joinDevices.Contains(device)) { return; }

        joinDevices.Remove(device);

        if (playerMap.ContainsKey(device.deviceId))
        {
            playerMap.Remove(device.deviceId);
        }

        RebuildMap();
        UpdateDeviceTexts();
    }
    private void RebuildMap()
    {
        playerMap.Clear();

        for (int i = 0; i < joinDevices.Count; i++)
        {
            playerMap[joinDevices[i].deviceId] = i;
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
            /*            startAction.Disable();
                        joinAction.Disable();
                        leaveAction.Disable();*/

            PlayerDataHolder.Instance.SetData(joinDevices, playerMap);
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
