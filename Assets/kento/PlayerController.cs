using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("-----移動設定-----")]
    [SerializeField,Header("通常スピード")] private float speed = 3.0f;                //通常スピード
    private Vector2 inputVer;　　　　　　　              //入力値
    
    private float curentSpeed;                           //
    [Header("-----ダッシュ設定-----")]
    [SerializeField,Header("ダッシュスピード")] private float dash_speed = 5.0f;           //ダッシュスピード

    [Header("-----ブリンク設定-----")]
    [SerializeField,Header("パワー")] private float brinkForce = 15.0f;
    [SerializeField,Header("状態の持続時間")] private float brinkDuration = 0.5f;
    [SerializeField,Header("クールダウン時間")] private float brinkCooldown = 1.0f; 
    [SerializeField,Header("ノックバック力")] private float knockbackForce = 5.0f;
    private bool isBrinkling = false;                    //ブリンクフラグ
    private float lastBrinkTime = 0f;                    //最後のブリンク時間

    private Rigidbody rb;



    public void OnMove(InputAction.CallbackContext context)
    {
        inputVer = context.ReadValue<Vector2>();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            curentSpeed = speed * dash_speed; 
        }
        if (context.canceled)
        {
            curentSpeed = speed;
        }
    }

    public void OnBrink(InputAction.CallbackContext context)
    {
        if (context.performed && !isBrinkling && Time.time > lastBrinkTime + brinkCooldown)
        {
            Brink();
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        curentSpeed = speed;
    }

    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        Vector3 move = new Vector3(inputVer.x, 0f, inputVer.y) * curentSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + move);

        if (move != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = rot;
        }
    }

    void Brink()
    {
        isBrinkling = true;
        lastBrinkTime = Time.time;
        // キャラクターが向いている方向に力を加える
        rb.AddForce(transform.forward * brinkForce, ForceMode.Impulse);

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
