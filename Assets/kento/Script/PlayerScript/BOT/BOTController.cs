using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Users;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class BOTController : MonoBehaviour
{
    [SerializeField] private GameObject point;
    GameObject pointTarget;
    GameObject playerTarget;
    GameObject near = null;
    float minDist;

/*    //Player操作反転
    public int playerID;
    private bool isInverted = false;*/

    Vector2 moveInput;
    public Vector2 MoveInput => moveInput;


    private float attackPrepareTime = 1f;
    bool preparingAttack = false;
    float prepareCounter = 0f;

    Quaternion targetRotation;
    bool isRota = false;

    bool attackRest = false;
    private float restTime = 3f;

    private PlayerStateManager stateManager;
    private MoveController move;
    private AtackController atack;
    private Seencer sencer;

    Rigidbody rb;



    void Start()
    {
        stateManager = GetComponent<PlayerStateManager>();
        move = GetComponent<MoveController>();
        atack = GetComponent<AtackController>();
        sencer = GetComponentInChildren<Seencer>();
        rb = GetComponent<Rigidbody>();

        CreatePoint();
    }

    void Update()
    {
        if(stateManager.State == State.Knockback) { return; }
        //false
        if (!sencer.CheckLayer())
        {
            StopAndCreate();
            if (!isRota)
            {
                targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 180f, 0);
                isRota = true;
            }

            Rota();
            return;
        }
        Serch();

        if (!attackRest && near != null && minDist < 10f)
        {
            playerTarget = near;
        }

        if (pointTarget == null)
        {
            CreatePoint();
        }
        if(playerTarget != null)
        {
            Attack();
        }

        MoveToPoint();

        if(pointTarget != null)
        {
            float dist = Vector3.Distance(transform.position, pointTarget.transform.position);
            if (dist < 1f)
            {
                Destroy(pointTarget);
                CreatePoint();
            }
        }
      
        if(!attackRest && playerTarget != null && stateManager.ActionState == ActionState.Attack)
        {
            StartCoroutine(RestTime());
        }
        if (stateManager.ActionState == ActionState.Attack)
        {
            preparingAttack = false;
        }
    }

    void MoveToPoint()
    {
        GameObject target = null;

        if(playerTarget != null)
        {
            target = playerTarget;
            Destroy(pointTarget);
        }
        else if(pointTarget != null)
        {
            target = pointTarget;
        }

        if (target == null) return;

        Vector3 dir = target.transform.position - transform.position;
        moveInput = new Vector2(dir.normalized.x, dir.normalized.z);
       
        /*if (isInverted)
        {
            //操作反転
            moveInput *= -1;
        }*/
        OnMove(moveInput);
    }

    void CreatePoint()
    {
        Vector3 pos = transform.position;
        Vector2 random = Random.insideUnitCircle * 10f;
        Vector3 pointPos = new Vector3(pos.x + random.x, pos.y, pos.z + random.y);

        GameObject p = Instantiate(point, pointPos, Quaternion.identity);

        pointTarget = p;
    }
    
    void StopAndCreate()
    {
        OnMove(Vector2.zero);

        if(pointTarget != null)
        {
            Destroy(pointTarget);
        }
        CreatePoint();
    }

    void Rota()
    {
        // Slerpで滑らかに回転
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, 5 * Time.deltaTime);

        // ほぼ目標向きになったら停止
        if (Quaternion.Angle(rb.rotation, targetRotation) < 0.5f)
        {
            rb.rotation = targetRotation;
            isRota = false;
        }
    }

    void Attack()
    {
        if (playerTarget == null) return;

        Vector3 dir = playerTarget.transform.position - transform.position;
        dir.y = 0;
        transform.forward = dir.normalized;
        float dist = dir.magnitude;
        if (!preparingAttack && dist < 6)
        {
            preparingAttack = true;
            prepareCounter = attackPrepareTime;
            OnMove(Vector2.zero); //止まる
        }
        // 溜め時間
        if (preparingAttack)
        {
            prepareCounter -= Time.deltaTime;

            // チャージ開始
            if (stateManager.ActionState == ActionState.None)
            {
                atack.Shot(0);
            }

            // 溜め完了
            if (prepareCounter <= 0f)
            {
                preparingAttack = false;

                if (stateManager.ActionState == ActionState.Charge)
                {
                    atack.Shot(1);
                }
            }
        }
    }

    void Serch()
    {
        near = null;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");


        minDist = Mathf.Infinity;

        foreach (var player in players)
        {
            if (player == gameObject) continue;

            float dist = Vector3.Distance(transform.position, player.transform.position);
            if(dist < minDist)
            {
                minDist = dist;
                near = player;
            }
        }
    }

    public void OnMove(Vector2 context)
    {
        stateManager.UpdateMoveState(context);
        move.SetMoveInput(context);
    }
 /*   public void SetReverse(bool value)
    {
        isInverted = value;
    }*/

    IEnumerator RestTime()
    {
        attackRest = true;

        playerTarget = null;
        CreatePoint();
        yield return new WaitForSeconds(restTime);

        attackRest = false;
    }
}
