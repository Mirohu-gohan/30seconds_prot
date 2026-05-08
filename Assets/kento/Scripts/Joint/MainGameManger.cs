//using Unity.Services.Authentication;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MainGameManger : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab = default; //Player
    [SerializeField] private GameObject botPrefab = default;    //Bot
    [SerializeField] private Transform[] pos = default;         //生成位置
    [SerializeField] private GameObject timeUpPanel;

    IEnumerator Start()
    {
        yield return null; // 1フレーム待つ
        timeUpPanel.gameObject.SetActive(false);
    }

    void Awake()
    {
        // シーンに事前配置されたテスト用Playerオブジェクトを削除（増殖バグ防止）
        foreach (var existing in GameObject.FindGameObjectsWithTag("Player"))
        {
            Destroy(existing);
        }

        //インスタンスがない場合はreturn
        if(PlayerDataHolder.Instance == null) { return; }

        //インスタンスで保持しているデバイス情報と人数を取得
        var devices = PlayerDataHolder.Instance.GetDevices();
        int count   = PlayerDataHolder.Instance.GetPlayerCount();

        //人数分Playerの生成,PlayerID
        for (int i = 0; i < pos.Length; i++)
        {
            if (i < count && devices[i] != null)
            {
                /*// 指定デバイスで PlayerInput を持つプレイヤーを生成
                var obj = PlayerInput.Instantiate(
                    prefab: playerPrefab,
                    playerIndex: i,
                    pairWithDevice: devices[i]
                 );
                //生成後この位置にセット
                obj.transform.position = pos[i].position;
                obj.transform.rotation = pos[i].rotation;*/
                var obj = Instantiate(
                    playerPrefab,
                    pos[i].position,
                    pos[i].rotation
                 );

                // PlayerInput取得
                PlayerInput input = obj.GetComponent<PlayerInput>();

                // デバイスを明示的にペアリング
                input.user.UnpairDevices();
                InputUser.PerformPairingWithDevice(devices[i], input.user);

                Debug.Log($"Player{i + 1} : {devices[i].displayName}");

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
            //else
            //{
            //    Instantiate(botPrefab, pos[i].position, pos[i].rotation);
            //}
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
