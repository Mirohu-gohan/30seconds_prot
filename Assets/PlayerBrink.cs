using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBrink : MonoBehaviour
{
    //-----押している時間でパワーが増す
    [Header("ブリンク設定")]
    [SerializeField] private float brinkForce = 15.0f;   //ブリンクパワー
    [SerializeField] private float brinkDuration = 0.5f; //ブリンク状態の持続時間
    [SerializeField] private float brinkCooldown = 1.0f; //ブリンククールダウン時間
    [SerializeField] private float knockbackForce = 5.0f;//ノックバック力
    private bool isBrinkling = false;                    //ブリンクフラグ
    private float lastBrinkTime = 0f;                    //最後のブリンク時間
 
    private bool isStrt = false;                         //押されている
    private float t;                                     //タイマー

    private Rigidbody rb;

    public void OnBrink(InputAction.CallbackContext context)
    {
        if (context.performed )
        {
           isStrt = true;
            

        }
        if(context.canceled && !isBrinkling && Time.time > lastBrinkTime + brinkCooldown)
        {
            isStrt = false;
            Brink();
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
       
    }

    private void Update()
    {
        if(isStrt)
        {
            t += Time.deltaTime;
        }
        else if (!isStrt)
        {
            t = 0f;
        }
    }

    void Brink()
    {
        isBrinkling = true;
        lastBrinkTime = Time.time;
        // キャラクターが向いている方向に力を加える
        rb.AddForce(transform.forward * brinkForce * t, ForceMode.Impulse);

        // 一定時間後にタックル状態を解除する
        Invoke("EndBrink", brinkDuration);
    }

    void EndBrink()
    {
        isBrinkling = false;
        // 勢いを止める（急ブレーキ）
        rb.linearVelocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody enemyrb = collision.gameObject.GetComponent<Rigidbody>();
        if (enemyrb != null)
        {
            Vector3 knockBackDir = collision.transform.position - transform.position;
            knockBackDir.y = 0f;
            enemyrb.AddForce(knockBackDir.normalized * knockbackForce, ForceMode.Impulse);
        }
    }
}
