using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TImeController : MonoBehaviour
{
    public Text Timetext;

    [SerializeField] 
    private float time = 60.0f; //時間制限

    public bool isTimeUp = false;
    
    //時間制限時に表示するパネル
    public GameObject timeUpPanel;


     void Start()
    {
        Time.timeScale = 1f;
        isTimeUp = false;
        if(timeUpPanel != null )
        {
            timeUpPanel.SetActive(false);//パネル非表示
        }
    }

    void Update()
    {
      if(isTimeUp)
        {
            return;
        }
      if(time>0)
        {
            time -=Time.deltaTime;
            Timetext.text =time.ToString("F1");
        }
      else
        {
            time = 0;
            TimeUp();
        }
    }
    void TimeUp()
    {
        isTimeUp = true;
        Timetext.text = "崩壊！！！";
        //ゲームポーズ
        Time.timeScale = 0f;

        //パネルの表示
        if (timeUpPanel != null)
        {
            timeUpPanel .SetActive(true);
        }


    }
    public void ResetGame()
    {
        //時間のリセット
        Time.timeScale = 1f;
        //シーンのロード
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}