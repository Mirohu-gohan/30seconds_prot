using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [Header("移動設定")]

    [SerializeField] private float speed = 5.0f; //移動スピード
    [SerializeField] private float ChargeMoveSpeedRate = 0.3f; //チャージ・硬直中の速度倍率
    private float speed2 = 0; //チャージ中のスピード
    private float curentSpeed = 0;  //現在のスピード
    [SerializeField] private float rotSpeed = 10.0f; //旋回スピード
    [SerializeField] private float ChargeRotateSpeedRate = 0.7f; //チャージ・硬直中の旋回倍率
    private float rotSpeed2 = 0;　//チャージ中旋回スピード
    private float curentRotSpeed = 0;//現在の旋回スピード

    private Vector2 inputVer;


    [Header("タックル設定")]

    [SerializeField] private float tackleForce = 15.0f;    //タックルパワー
    [SerializeField] private float tackleDuration = 0.5f;//タックル状態の持続時間
    [SerializeField] private float tackleCooldown = 1.0f;//タックルのクールダウン時間
    [SerializeField] private float WeakKnockbackForce = 2.5f; //弱パンチノックバック
    [SerializeField] private float StrongKnockbackForce = 5.0f;//強パンチノックバック
    private float curentknockbackForce = 0f;//現在のノックバック力

    private Rigidbody rb;
    private bool isTackling = false;
    private float lastTackleTime = 0f; // 最後のタックル時間

    private bool isPrese = false; //押されているかフラグ
    [HideInInspector] public bool isStrt = false;//タイマスタートフラグ
    private float t = 0f; //タイマー
    public float chargeMax = 5.0f; //タイマー上限
    [SerializeField] private bool isMax = false;//チャージがMaxかのフラグ

    [SerializeField] private Text IDtext; //PlayerIDText
    private int playerID; //PlayerID
    private PlayerInput playerInput; //PlayerInput

    private void Awake()
    {
        speed2 = speed * ChargeMoveSpeedRate;
        rotSpeed2 = rotSpeed * ChargeRotateSpeedRate;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputVer = context.ReadValue<Vector2>();
    }

    public void OnTackle(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isPrese = true;
            if (!isTackling && Time.time > lastTackleTime + tackleCooldown)
            {
                isStrt = true;
            }
        }
        if (context.canceled)
        {
            isPrese = false;
            if (isStrt && !isTackling && Time.time > lastTackleTime + tackleCooldown)
            {
                Tackle();
            }

            isStrt = false;
        }
    }

    public void SetCharge(float value)
    {
        t = value;
    }

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerID = playerInput.playerIndex;
        }

        IDtext.text += $"Player{playerID + 1}\n";

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
            Move();

        if (isStrt)
        {
            if (t < chargeMax)
            {
                t += Time.deltaTime;
            }
            if (t == chargeMax)
            {
                isMax = true;
            }
        }
        else if(!isStrt)
        {
            t = 0f;
        }
    }

    void Move()
    {
        if (isPrese)
        {
            curentSpeed = speed2;
            curentRotSpeed = rotSpeed2;
        }
        if (!isPrese)
        {
            curentSpeed = speed;
            curentRotSpeed = rotSpeed;
        }
        Vector3 move = new Vector3(inputVer.x, 0f, inputVer.y) * curentSpeed * Time.deltaTime;
        transform.position += move;

        if (move != Vector3.zero)
        {
            Quaternion Rot = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, Rot, curentRotSpeed * Time.deltaTime);
        }
    }



    void Tackle()
    {
        isTackling = true;
        lastTackleTime = Time.time;

        rb.AddForce(transform.forward * tackleForce * t, ForceMode.Impulse);

        Invoke("EndTackle", tackleDuration);
    }

    void EndTackle()
    {
        isTackling = false;
        // 勢いを止める（急ブレーキ）
        rb.linearVelocity = Vector3.zero;

        if (isMax)
        {
            //ここで硬直処理
        }
        isMax = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody enemyrb = collision.gameObject.GetComponent<Rigidbody>();
        if (enemyrb != null)
        {
            if (isMax)
            {
                curentknockbackForce = StrongKnockbackForce;
            }
            else
            {
                curentknockbackForce = WeakKnockbackForce;
            }
            Vector3 knockBackDir = collision.transform.position - transform.position;
            knockBackDir.y = 0f;
            Debug.Log(curentknockbackForce);
            enemyrb.AddForce(knockBackDir.normalized * curentknockbackForce, ForceMode.Impulse);
        }
    }
}
