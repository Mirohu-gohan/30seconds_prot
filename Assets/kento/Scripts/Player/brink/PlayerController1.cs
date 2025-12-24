
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
//using static System.Net.Mime.MediaTypeNames;

public class PlayerController1 : MonoBehaviour
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


    [Header("ブリンク設定")]

    [SerializeField] private float tackleForce = 15.0f;    //�^�b�N���p���[
    [SerializeField] private float tackleDuration = 0.5f;//�^�b�N����Ԃ̎�������
    [SerializeField] private float tackleCooldown = 1.0f;//�^�b�N���̃N�[���_�E������
    [SerializeField] private float WeakKnockbackForce = 2.5f; //��p���`�m�b�N�o�b�N
    [SerializeField] private float StrongKnockbackForce = 5.0f;//強パンチノックバック
    private float curentknockbackForce = 0f;//現在のノックバック力
  

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
  
   

    private int playerID;
    private PlayerInput playerInput;
    [SerializeField] private Text IDtext;

    private Animator animator;

    //private float y = -5.0f;


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
            isfinish = false;
            isPrese = true;
            if (!isTackling && Time.time > lastTackleTime + tackleCooldown)
            {
                isStrt = true;
                animator.SetBool("Charge", true);
            }
        }
        if (context.canceled )
        {
            animator.SetBool("Charge", false);
            isPrese = false;
            if (isStrt && !isTackling && Time.time > lastTackleTime + tackleCooldown)
            {
                Tackle();
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
        
        IDtext.text += $"Player {playerID + 1}\n";
       
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    void FixedUpdate()
    {
            Move();
        if (!isfinish)
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
            if (t >= chargeMax)
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
        if(!isPrese)
        {
            curentSpeed = speed;
            curentRotSpeed = rotSpeed;
        }
        Vector3 move = new Vector3(inputVer.x, 0f, inputVer.y) * curentSpeed * Time.deltaTime;
        //rb.MovePosition(rb.position + move);
        transform.position += move;

        if (move != Vector3.zero)
        {
            Quaternion Rot = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation,Rot,curentRotSpeed * Time.deltaTime);
            //rb.MoveRotation(Quaternion.Slerp(rb.rotation, Rot, curentRotSpeed * Time.fixedDeltaTime));
        }
    }

    void Tackle()
    {
        if (isfinish) { return; }
        isTackling = true;
        lastTackleTime = Time.time;

        rb.AddForce(transform.forward * tackleForce * t, ForceMode.Impulse);

        Invoke("EndTackle", tackleDuration);
    }

   

    void EndTackle()
    {
        isTackling = false;
        rb.linearVelocity = Vector3.zero;

        //ここで硬直処理
        if (isMax)
        {
            isfinish = true;
        }
        isMax = false;
        animator.SetBool("AtackKyou", false);
        animator.SetBool("AtackJaku", false);

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
