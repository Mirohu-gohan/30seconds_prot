using UnityEditor.Profiling;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ChargeSpike : MonoBehaviour
{
    //[SerializeField] private DecalProjector targetRender;
    [SerializeField] private float MaxChargeTime = 1.5f;
   
    //private float charge;
    private Material mat;
    private AtackController ac;
    private PlayerStateManager stateManager;
    //-----------------
    [SerializeField] private Image MeterImage;

    private float meterSpeed = 1.0f;
    private Coroutine meter;
    //---------------

  /*  private void OnEnable()
    {
        mat = new Material(targetRender.material);
        targetRender.material = mat;
    }*/

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ac = GetComponent<AtackController>();
        stateManager = GetComponent<PlayerStateManager>();
        MeterImage.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float speed = 1f / MaxChargeTime;
        if (stateManager.ActionState == ActionState.Charge)
        {
            //charge += Time.deltaTime / MaxChargeTime;
            MeterImage.fillAmount += speed * Time.deltaTime;
        }
        else
        {
            //charge = 0f;
            MeterImage.fillAmount = 0;
        }

        if(stateManager.State == State.Knockback)
        {
            //charge = 0f;
            MeterImage.fillAmount = 0;
        }

        // 0〜1 の範囲に制限
        MeterImage.fillAmount = Mathf.Clamp01(MeterImage.fillAmount);

        // Player のタックル力 (t) に反映
        ac.SetCharge(MeterImage.fillAmount * ac.chargeMax);

      /*  // 0〜1 の範囲に制限
        charge = Mathf.Clamp01(charge);

        mat.SetFloat("_Charge", charge);

        // Player のタックル力 (t) に反映
        ac.SetCharge(charge * ac.chargeMax);*/
    }
}
