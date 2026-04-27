using UnityEditor.Profiling;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ChargeSpike : MonoBehaviour
{
    //[SerializeField] private DecalProjector targetRender;
    [SerializeField] private float MaxChargeTime = 1.5f;

    private static int nextID = 0;
    public int ID { get;private set; }

    //private float charge;
    private Material mat;
    private AtackController ac;
    private PlayerStateManager stateManager;
    //-----------------
    //[SerializeField] private Image MeterImage;
    [SerializeField] private Image[] MeterImage;
    private Image image;
   
    private float meterSpeed = 1.0f;
    private Coroutine meter;
    //---------------

    /*  private void OnEnable()
      {
          mat = new Material(targetRender.material);
          targetRender.material = mat;
      }*/

    private void Awake()
    {
        ID = nextID;
        nextID++;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ac = GetComponent<AtackController>();
        stateManager = GetComponent<PlayerStateManager>();
        image = MeterImage[ID];
        image.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float speed = 1f / MaxChargeTime;
        if (stateManager.ActionState == ActionState.Charge)
        {
            //charge += Time.deltaTime / MaxChargeTime;
            image.fillAmount += speed * Time.deltaTime;
        }
        else
        {
            //charge = 0f;
            image.fillAmount = 0;
        }

        if (stateManager.State == State.Knockback)
        {
            //charge = 0f;
            image.fillAmount = 0;
        }

        // 0〜1 の範囲に制限
        image.fillAmount = Mathf.Clamp01(image.fillAmount);

        // Player のタックル力 (t) に反映
        ac.SetCharge(image.fillAmount * ac.chargeMax);
    }
}
