using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PowerMeter : MonoBehaviour
{
    [SerializeField] private Image MeterImage;

    private float meterSpeed = 1.0f;
    private Coroutine meter;

    [SerializeField] private float fillSpeed = 1.0f;

    private PlayerController pc;


    private void Start()
    {
        pc = GetComponent<PlayerController>();
        MeterImage.fillAmount = 0;
    }

    private void Update()
    {
        if(pc.isStrt)
        {
            MeterImage.fillAmount += fillSpeed * Time.deltaTime;
        }
        else if(!pc.isStrt) 
        {
            MeterImage.fillAmount -= fillSpeed * Time.deltaTime;
        }

        // 0〜1 の範囲に制限
        MeterImage.fillAmount = Mathf.Clamp01(MeterImage.fillAmount);

        // Player のタックル力 (t) に反映
        pc.SetCharge(MeterImage.fillAmount * pc.chargeMax);
    }
}
