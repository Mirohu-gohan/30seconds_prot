using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [Header("移動設定")]

    [SerializeField] private float speed = 5.0f; //移動スピード
 
    private Vector2 inputVer;


    [Header("タックル設定")]

    [SerializeField] private float tackleForce = 15.0f;    //タックルパワー
    [SerializeField] private float tackleDuration = 0.5f;//タックル状態の持続時間
    [SerializeField] private float tackleCooldown = 1.0f;//タックルのクールダウン時間
    [SerializeField] private float knockbackForce = 5.0f;//ノックバック力

    private Rigidbody rb;
    private bool isTackling = false;
    private float lastTackleTime = 0f; // 最後のタックル時間

    [HideInInspector] public bool isStrt = false;
    private float t = 0f;
    public float chargeMax = 5.0f;

    public void OnMove(InputAction.CallbackContext context)
    {
        inputVer = context.ReadValue<Vector2>();
    }

    public void OnTackle(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isTackling && Time.time > lastTackleTime + tackleCooldown)
            {
                isStrt = true;
            } 
        }
        if (context.canceled )
        {
            if(isStrt && !isTackling && Time.time > lastTackleTime + tackleCooldown)
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

        GetComponent<Renderer>().material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
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
                Debug.Log(t);
            }
        }
        else if(!isStrt)
        {
            t = 0f;
        }
    }

    void Move()
    {
        if(isTackling) {return;}
        Vector3 move = new Vector3(inputVer.x,0f,inputVer.y) * speed * Time.deltaTime;
        transform.position += move;

        if (move != Vector3.zero)
        {
            Quaternion Rot = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Rot;
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
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody enemyrb = collision.gameObject.GetComponent<Rigidbody>();
        if (enemyrb != null)
        {
            Vector3 knockBackDir = collision.transform.position - transform.position;
            //knockBackDir.y = 0f;
            enemyrb.AddForce(knockBackDir.normalized * knockbackForce, ForceMode.Impulse);
        }
    }
}
