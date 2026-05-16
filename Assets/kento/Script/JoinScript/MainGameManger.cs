//using Unity.Services.Authentication;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MainGameManger : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab = default;
    [SerializeField] private GameObject botPrefab = default;

    [SerializeField] private Transform[] pos = default;
    [SerializeField] private GameObject timeUpPanel;

    private GameObject joinObj;

    int i = 0;

    IEnumerator Start()
    {
        yield return null; // 1フレーム待つ
        timeUpPanel.gameObject.SetActive(false);
    }

    private void Awake()
    {
        joinObj = GameObject.Find("JoinedManager");
        //インスタンスがない場合はreturn
        if (JoinData.Instance == null) { return; }

        //インスタンスで保持しているデバイス情報を取得
        var devices = JoinData.Instance.GetDevices();

        //人数分Playerの生成
        /* foreach (var device in devices)
         {
             if (device != null)
             {
                 var obj = PlayerInput.Instantiate(
                      prefab: playerPrefab,
                      pairWithDevice: device
                 );
                 obj.transform.position = pos[i].position;
                 obj.transform.rotation = pos[i].rotation;
                 Debug.Log(i + "番" + device.displayName);
                 i++;
             }
         }*/
        foreach (var device in devices)
        {

            /* if (device != null)
             {
                 var obj = PlayerInput.Instantiate(
                      prefab: playerPrefab,
                      playerIndex: i,
                      pairWithDevice: device
                 );
                 obj.transform.position = pos[i].position;
                 obj.transform.rotation = pos[i].rotation;
                 Debug.Log(i + "番" + device.displayName);
                 i++;
             }*/
            if (device != null)
            {
                var obj = Instantiate(playerPrefab, pos[i].position, pos[i].rotation);

                var input = obj.GetComponent<PlayerInput>();

                // ★重要：完全解除
                input.user.UnpairDevices();

                // ★Join順で固定
                InputUser.PerformPairingWithDevice(device, input.user);

                Debug.Log($"P{i + 1}: {device.displayName}");

                i++;
            }
        }
        while (i < 4)
        {
            Instantiate(botPrefab, pos[i].position, pos[i].rotation);
            Debug.Log(i + "番");
            i++;
        }
    }

    public void OnReset()
    {
        SceneManager.LoadScene("Start");
        Destroy(joinObj);
    }
}
