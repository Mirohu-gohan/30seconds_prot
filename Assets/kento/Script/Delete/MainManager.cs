using System.Collections;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;



public class MainManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab = default; //Player
    [SerializeField] private GameObject botPrefab = default;
    [SerializeField] private Transform[] pos = default;         //生成位置
    [SerializeField] private GameObject timeUpPanel;

    private GameObject joinobj;
    int i = 0;

    IEnumerator Start()
    {
        yield return null; // 1フレーム待つ
        timeUpPanel.gameObject.SetActive(false);
    }

    void Awake()
    {
        joinobj = GameObject.Find("JoinedManager");
        //インスタンスがない場合はreturn
        if (PlayerDataHolder.Instance == null) { return; }

        //インスタンスで保持しているデバイス情報と人数を取得
        var devices = PlayerDataHolder.Instance.devices;
        var map = PlayerDataHolder.Instance.playerMap;

        foreach (var device in devices)
        {
            if (device == null) return;
            int playerIndex = map[device.deviceId];

            var obj = PlayerInput.Instantiate(
                prefab: playerPrefab,
                playerIndex: playerIndex,
                pairWithDevice: device
            );
            obj.transform.position = pos[i].position;
            obj.transform.rotation = pos[i].rotation;

            i++;
        }
        while (i < 4)
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
            i++;
        }
    }

    public void OnReset()
    {
        SceneManager.LoadScene("Start");
        Destroy(joinobj);
    }
}
