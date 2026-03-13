using UnityEngine;



public class MoveController : MonoBehaviour
{
    [Header("à⁄ìÆê›íË")]
    [SerializeField] private float Speed = 5.0f;
    private float Speed2 = 0f;
    [SerializeField] private float chargingmoveSpeedRate = 0.3f;
    private float curentSpeed = 0f;

    [SerializeField] private float rotSpeed = 10.0f;
    private float rotSpeed2 = 0f;
    [SerializeField] private float ChargeingRotSpeedRate = 0.7f;
    private float curentRotSpeed = 0f;

    Vector2 inputVer;

    Rigidbody rb;

    //-----ScriptéQè∆-----
    private PlayerStateManager stateManager;
    private AtackController ac;

    private void Awake()
    {
        Speed2 = Speed * chargingmoveSpeedRate;
        rotSpeed2 = rotSpeed * ChargeingRotSpeedRate;
        curentSpeed = Speed;
    }

    private void Start()
    {
        stateManager = GetComponent<PlayerStateManager>();
        ac = GetComponent<AtackController>();
        rb = GetComponent<Rigidbody>();
    }

    public void SetMoveInput(Vector2 input)
    {
        inputVer = input;
    }

    // Update is called once per frame
    void Update()
    {
        //stateManager.UpdateMoveState(inputVer);
        Move();
    }

    public void Move()
    {
        if (ac.isRigid) { return; }
        if(stateManager.State == State.Knockback) { return; }

        curentSpeed = Speed;
        curentRotSpeed = rotSpeed;

        if (stateManager.ActionState == ActionState.Charge)
        {
            curentSpeed = Speed2;
            curentRotSpeed = rotSpeed2;
        }

        if (stateManager.MoveState == MoveState.Idle) { return; }

        if (stateManager.ActionState != ActionState.Attack)
        {
            Vector3 move = new Vector3(inputVer.x, 0, inputVer.y) * curentSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + move);

            if (move != Vector3.zero)
            {
                Quaternion Rot = Quaternion.LookRotation(move, Vector3.up);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, Rot, curentRotSpeed * Time.deltaTime));
            }
        }
    }
}
