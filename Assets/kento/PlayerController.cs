using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("通常移動")]

    [SerializeField]     private float speed; //歩きスピード
    [SerializeField] private float dash_speed;//ダッシュスピード
    private float currentSpeed = 0;
    private Vector2 inputVer;

    [Header("ブリンク設定")]

    [SerializeField] private float tackleForce = 15.0f;  //ブリンクパワー
    [SerializeField] private float tackleDuration = 0.5f;//ブリンク状態の持続時間
    [SerializeField] private float tackleCooldown = 1.0f;//ブリンクのクールダウン時間
    [SerializeField] private float knockbackForce = 5.0f;//ノックバック力

    private Rigidbody rb;
    private bool isTackling = false;
    private float lastTackleTime = 0f; // 最後のブリンク時間

    //----------
    private float t = 0;
    private bool isStart = false;
    [SerializeField] private float chageMax = 5.0f;




    public void OnMove(InputAction.CallbackContext context)
    {
            inputVer = context.ReadValue<Vector2>();
    }

    public void OnDash(InputAction.CallbackContext context) 
    {
        if (context.performed)
        {
            currentSpeed = speed * dash_speed;
        }
        else if (context.canceled)
        {
            currentSpeed = speed;
        }
    }
    public void OnBrink(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isStart = true;
        }
        if (context.canceled && !isTackling && Time.time > lastTackleTime * tackleCooldown)
        {
            isStart = false;
            Brink();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>(); 
        currentSpeed = speed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();

        if (isStart)
        {
            if (t < chageMax)
            {
                t += Time.deltaTime;
            }
        }
        else if(!isStart)
        {
            t = 0f;
        }
    }

    void Move()
    {
        Vector3 move = new Vector3(inputVer.x, 0f, inputVer.y) * currentSpeed * Time.deltaTime;
        //transform.position += move;
        rb.MovePosition(rb.position + move);

        if (move != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = rot;
        }
    }

  
    void Brink()
    {
        isTackling = true;
        lastTackleTime = Time.time;

        // キャラクターが向いている方向に力を加える
        rb.AddForce(transform.forward * tackleForce * t, ForceMode.Impulse);

        // 一定時間後にタックル状態を解除する
        Invoke("EndTackle", tackleDuration);
    }

    void EndTackle()
    {
        isTackling = false;
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
