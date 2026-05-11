using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab = default; //Player
    [SerializeField] private GameObject botPrefab = default;
    [SerializeField] private Transform[] pos = default;         //生成位置
    [SerializeField] private GameObject timeUpPanel;

    //private GameObject joinobj;

    [SerializeField] private string[] connectedDevices;

    IEnumerator Start()
    {
        yield return null; // 1フレーム待つ
        timeUpPanel.gameObject.SetActive(false);
    }

    void Awake()
    {
        Debug.Log("MainManager Awake");

        //joinobj = GameObject.Find("JoinedManager");
        //インスタンスがない場合はreturn
        if (PlayerDataHolder.Instance == null) { return; }

        //インスタンスで保持しているデバイス情報と人数を取得
        //var devices = PlayerDataHolder.Instance.GetDevices();
        int count = PlayerDataHolder.Instance.GetPlayerCount();

        /*connectedDevices = new string[devices.Count()];

        for (int i = 0; i < devices.Count(); i++)
        {
            if (devices[i] != null)
            {
                connectedDevices[i] = $"[{i}] {devices[i].displayName} ({devices[i].layout})";
            }
            else
            {
                connectedDevices[i] = $"[{i}] None";
            }
        }*/
        //------------

        var device = PlayerDataHolder.Instance.devices;

        for (int i = 0; i < pos.Length; i++)
        {
            if (i < device.Count && device[i] != null)
            {
                var obj = Instantiate(playerPrefab, pos[i].position, pos[i].rotation);

                var input = obj.GetComponent<PlayerInput>();

                input.user.UnpairDevices();
                InputUser.PerformPairingWithDevice(device[i], input.user);
            }
            else
            {
                // --- ここから書き換え ---
                // 1. Botを生成して、変数「botObj」に入れる
                GameObject botObj = Instantiate(botPrefab, pos[i].position, pos[i].rotation);

                // 2. 生成したBotから PlayerHealth スクリプトを探す
                PlayerHealth health = botObj.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    // 3. Botに「お前は i 番目（0, 1, 2, 3...）だよ」と教え込む
                    health.playerIndex = i;
                }
                // --- ここまで --- 
            }
        }

        //人数分Playerの生成,PlayerID
        /*for (int i = 0; i < pos.Length; i++)
        {
            if (i < count && devices[i] != null)
            {
                // 指定デバイスで PlayerInput を持つプレイヤーを生成
                var obj = PlayerInput.Instantiate(
                    prefab: playerPrefab,
                    playerIndex: i,
                    pairWithDevice: devices[i]
                 );
                //生成後この位置にセット
                obj.transform.position = pos[i].position;
                obj.transform.rotation = pos[i].rotation;

            }
            else
            {
                // --- ここから書き換え ---
                // 1. Botを生成して、変数「botObj」に入れる
                GameObject botObj = Instantiate(botPrefab, pos[i].position, pos[i].rotation);

                // 2. 生成したBotから PlayerHealth スクリプトを探す
                PlayerHealth health = botObj.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    // 3. Botに「お前は i 番目（0, 1, 2, 3...）だよ」と教え込む
                    health.playerIndex = i;
                }
                // --- ここまで --- 
            }
        }*/
    }
}
