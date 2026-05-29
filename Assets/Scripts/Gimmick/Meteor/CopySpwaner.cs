using UnityEngine;

public class CopySpwaner : MonoBehaviour
{
    [Header("↓何をコピーする？")]
    public GameObject original;

    [Header("↓サドンデスを判断するscriptを↓")]
    //public CheckMode isMode;

    private int amount = 0;

    //テスト用
    public bool isSuddenDeath = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isSuddenDeath && amount == 1)
        {
            Paste();
        }
    }

    public void Paste()
    {
        GameObject copyObject = Instantiate(original);
        amount++;
    }
}
