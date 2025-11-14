using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Text timer;
    private float t = 30;
    
    void Start()
    {
       
    }


    void Update()
    {
        t -= Time.deltaTime;

        if (t < 0)
        {
            t = 0;
        }

        timer.text = t.ToString("f0");
    }
}
