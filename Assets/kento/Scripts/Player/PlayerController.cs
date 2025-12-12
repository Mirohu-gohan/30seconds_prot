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


    [Header("パンチ設定")]

    [SerializeField] private GameObject box;
    private BoxCollider boxCollider;
    //[SerializeField] private float Power = 10.0f;
    [SerializeField] private float WeakKnockbackForce = 0.5f; //弱パンチノックバック
    [SerializeField] private float StrongKnockbackForce = 5.0f;//強パンチノックバック
    private float curentknockbackForce = 0f;//現在のノックバック力
    //private float tackleCooldown = 1.0f;//タックルのクールダウン時間
    [SerializeField] private float HitDuration = 0.2f; //攻撃判定の持続時間
    [SerializeField] private float wait = 0.25f;

    [SerializeField] private float StrongRecoveryTime = 1.0f; //硬直時間
    private float curentRecoveryTime;
    private bool isfinish = false;

    private Rigidbody rb;
    private bool isTackling = false;
    private float lastTackleTime = 0f; // 最後のタックル時間

    private bool isPrese = false; //押されているかフラグ
    [HideInInspector] public bool isStrt = false;//タイマスタートフラグ
    private float t = 0f; //タイマー
    public float chargeMax = 5.0f; //タイマー上限
    private bool isMax = false;//チャージがMaxかのフラグ

    [SerializeField] private Text IDtext; //PlayerIDText
    private int playerID; //PlayerID
    private PlayerInput playerInput; //PlayerInput

    private Animator animator;

    private void Awake()
    {
        speed2 = speed * ChargeMoveSpeedRate;
        rotSpeed2 = rotSpeed * ChargeRotateSpeedRate;
        curentRecoveryTime = StrongRecoveryTime;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputVer = context.ReadValue<Vector2>();
    }

    public void OnTackle(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            box.SetActive(true);
            isfinish = false;
            isPrese = true;
            isStrt = true;

            animator.SetBool("Charge",true);
        }
        if (context.canceled)
        {
            animator.SetBool("Charge", false);
            isPrese = false;
            if (isStrt)
            {
                Invoke("Panti", wait);
                if (isMax)
                {
                    animator.SetBool("AtackKyou", true);
                }
                else if (!isMax)
                {
                    animator.SetBool("AtackJaku", true);
                }
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

        box.SetActive(false);
        boxCollider = box.GetComponent<BoxCollider>();
        animator = GetComponentInChildren<Animator>();

        boxCollider.enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
            Move();
        if(!isfinish)
        {
            float mag = inputVer.magnitude;
            animator.SetFloat("speed", mag);
        }
        

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
        if (isfinish)
        {
            if (curentRecoveryTime > 0)
            {
                curentRecoveryTime -= Time.deltaTime;
            }
            if (curentRecoveryTime <= 0)
            {
                isfinish = false;
                curentRecoveryTime = StrongRecoveryTime;
            }
        }
    }

    void Move()
    {
        if (isfinish) { return; }
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



    void Panti()
    {
        if (isfinish) { return; }
        isTackling = true;

       
        boxCollider.enabled = true;

        Invoke("EndTackle", HitDuration);

    }

    void EndTackle()
    {
        isTackling = false;

        //ここで硬直処理
        if (isMax)
        {
            isfinish = true;
            
        }
        box.SetActive(false);
        boxCollider.enabled = false;
        isMax = false;
        animator.SetBool("AtackKyou", false);
        animator.SetBool("AtackJaku", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody enemyrb = other.gameObject.GetComponent<Rigidbody>();
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

            Vector3 knockBackDir = other.transform.position - transform.position;
            knockBackDir.y = 0.0f; // 垂直ノックバックを付けたくない場合
            Debug.Log(curentknockbackForce);
            enemyrb.AddForce(knockBackDir.normalized * curentknockbackForce, ForceMode.Impulse);
        }
    }
}
