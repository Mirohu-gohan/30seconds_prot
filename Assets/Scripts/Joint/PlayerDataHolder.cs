using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDataHolder : MonoBehaviour
{
    public static PlayerDataHolder Instance { get; private set; } //Playerの接続データインスタンス

    private InputDevice[] devices;                                //参加中のPlayerデバイス
    private int playerCount;                                      //Playerの接続数

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


    public void SetDevices(InputDevice[] devis, int cout)
    {
        //人数分の配列制作
        devices = new InputDevice[cout];
        //作った配列にデバイス情報格納
        for (int i = 0; i < cout; i++)
        {
            devices[i] = devis[i];
            //playerCount = cout;
        }
        //Player数の保存
        playerCount = cout;
    }

    
    public InputDevice[] GetDevices() => devices; //Playerのデバイス取得
    public int GetPlayerCount() => playerCount;   //Playerの接続数取得
}