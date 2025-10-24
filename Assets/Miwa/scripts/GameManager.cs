using UnityEngine;
using UnityEngine.UI;


public class NewMonoBehaviourScript : MonoBehaviour
{
    public float time = 60f;//制限時間

    public Text timetext;//UIのTextコンポーネント

    private float currentTime;//残り時間
    private bool isTimeUP = false;//時間切れフラグ

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTime = time;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimeUP) return;

        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            currentTime = 0;
            isTimeUP = true;

        }
       UpdateTimeUI();
    }
     void UpdateTimeUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds= Mathf.FloorToInt(currentTime % 60);

        timetext.text=string.Format("{0:00}:{1:00}",minutes,seconds);

        if (currentTime <= 10f)
        {
            timetext.color = Color.red ;
        }

    }
}
