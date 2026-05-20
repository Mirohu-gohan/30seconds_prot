using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDataHolder : MonoBehaviour
{
    public static PlayerDataHolder Instance { get; private set; } //Playerの接続データインスタンス
    public List<InputDevice> devices = new();

    public Dictionary<int, int> playerMap = new();

    private void Awake()
    {
        //既に存在する場合は、新しく生成された方を破棄する。
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        //インスタンスに自身を取得,シーンをまたいでも破壊されない
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetData(List<InputDevice> newDevices, Dictionary<int, int> newMap)
    {
        devices = new List<InputDevice>(newDevices);
        playerMap = new Dictionary<int, int>(newMap);
    }
}