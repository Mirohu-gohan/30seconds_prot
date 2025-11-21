using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MainGameManger : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab = default; //Player

    [SerializeField] private Transform[] pos = default;         //生成位置
 
    void Start()
    {
        //インスタンスがない場合はreturn
        if(PlayerDataHolder.Instance == null) { return; }

        //インスタンスで保持しているデバイス情報と人数を取得
        var devices = PlayerDataHolder.Instance.GetDevices();
        int count   = PlayerDataHolder.Instance.GetPlayerCount();

        //人数分Playerの生成,PlayerID
        for (int i = 0; i < count; i++)
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

            //もしかしたら総合数が表示される
            var ui = obj.GetComponent<PlayerUI>();
            ui.SetCount( count );
        }

       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
