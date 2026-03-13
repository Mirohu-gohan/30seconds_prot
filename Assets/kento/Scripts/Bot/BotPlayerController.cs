using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BotPlayerController : MonoBehaviour
{
   /* [Header("移動設定")]

    [SerializeField] private float speed = 5.0f; //移動スピード
    [SerializeField] private float ChargeMoveSpeedRate = 0.3f; //チャージ・硬直中の速度倍率
    private float speed2 = 0; //チャージ中のスピード
    private float curentSpeed = 0;  //現在のスピード
    [SerializeField] private float rotSpeed = 10.0f; //旋回スピード
    [SerializeField] private float ChargeRotateSpeedRate = 0.7f; //チャージ・硬直中の旋回倍率
    private float rotSpeed2 = 0;　//チャージ中旋回スピード
    private float curentRotSpeed = 0;//現在の旋回スピード

    [Header("攻撃設定")]

    [SerializeField] private float tackleForce;    //ブリンク力
    [SerializeField] private float tackleDuration = 0.5f;//持続時間
    [SerializeField] private float tackleCooldown = 1.0f;//クールダウン時間

    //-----硬直-----
    [SerializeField] private float StrongRecoveryTime = 1.0f; //硬直時間
    private float curentRecoveryTime;
    private bool isfinish = false;
    private float r;

    private bool isPrese = false; //攻撃キー入力フラグ
    [HideInInspector] public bool isStrt = false;//チャージ開始フラグ
    private float t = 0f; //チャージ量
    [HideInInspector] public float chargeMax = 5.0f; //チャージ上限
    private bool isMax = false;//チャージがMaxかのフラグ

    bool isAttack1 = false;
    bool isAttack2 = false;

    [Header("ノックバック,無敵設定")]
    [SerializeField] private float WeakKnockbackForce = 2.5f; //弱ブリンクノックバック
    [SerializeField] private float StrongKnockbackForce = 5.0f;//強ブリンクノックバック
    private float curentknockbackForce = 0f;//現在のノックバック力


    private Rigidbody rb;
    private bool isTackling = false;
    private float lastTackleTime = 0f; // 最後のタックル時間

    [Header("サーチ設定")]
    [SerializeField] private float searchInterval = 0.5f;
    [SerializeField] private float searchRange = 15f;

    private float searchTimer = 0f;

    [Header("ステージ範囲")]
    [SerializeField] private Vector3 stageMin; // ステージの最小座標
    [SerializeField] private Vector3 stageMax; // ステージの最大座標

    [Header("当たり判定設定")]
    [SerializeField] private SphereCollider searchArea;
    [SerializeField] private float angle = 45f;

    //-----その他-----
    public List<GameObject> players = new List<GameObject>();  //Player達
    public List<GameObject> outPlayers = new List<GameObject>();  //場外Player達
    private float minDistance = Mathf.Infinity; //最短距離をだすための目安値

    public GameObject target;       //攻撃対象
    private float distance;          //攻撃対象との距離

    Reception reception;
    Animator animator;


    void Awake()
    {
        speed2 = speed * ChargeMoveSpeedRate;
        rotSpeed2 = rotSpeed * ChargeRotateSpeedRate;
        curentRecoveryTime = StrongRecoveryTime;
    }

    public void SetCharge(float value)
    {
        t = value;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        reception = GetComponent<Reception>();
    }

    // Update is called once per frame
    void Update()
    {
        searchTimer += Time.deltaTime;
        if (searchTimer >= searchInterval)
        {
            CollectPlayers();
            SearchTarget();
            searchTimer = 0f;
        }
        if (target == null) return;

        // ステージ外チェック
        if (IsOutOfStage(transform.position) || IsOutOfStage(target.transform.position))
        {
            ResetTarget();
            return;
        }

        distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance > 5f)
        {
            Move();
        }

        if (distance < 15f)
        {
            Atack(true);
        }

        if (distance < r)
        {
            Atack(false);
        }

        if (isStrt)
        {
            t += Time.deltaTime;

            if (t >= chargeMax)
            {
                t = chargeMax;
                isMax = true;
            }
        }
        else
        {
            t = 0f;
            isMax = false;
        }
        float mag = rb.linearVelocity.magnitude;
        animator.SetFloat("Speed", mag);
        animator.SetBool("IsChage", isStrt);
        animator.SetBool("isAttack1", isAttack1);
        animator.SetBool("isAttack2", isAttack2);
    }
    void Move()
    {
        if (isPrese)
        {
            curentSpeed = speed2;
            curentRotSpeed = rotSpeed2;
        }
        else
        {
            curentSpeed = speed;
            curentRotSpeed = rotSpeed;
        }

        if (!isTackling)
        {
            Vector3 dir = (target.transform.position - transform.position).normalized;

            Vector3 move = dir * curentSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + move);

            if (move != Vector3.zero)
            {
                Quaternion Rot = Quaternion.LookRotation(dir, Vector3.up);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, Rot, curentRotSpeed * Time.deltaTime));
            }
        }
    }

    void Atack(bool a)
    {
        if (a)
        {
            isfinish = false;

            if (!isTackling && Time.time > lastTackleTime + tackleCooldown)
            {
                if (!isStrt)
                {
                    r = Random.Range(5f, 10f);
                }
                isStrt = true;
                isPrese = true;
            }
        }
        if (!a)
        {
            isPrese = false;
            if (isStrt && !isTackling && Time.time > lastTackleTime + tackleCooldown)
            {
                Tackle();
            }
            isStrt = false;
        }
    }
    void Tackle()
    {
        if (isfinish) { return; }
        if (reception != null && reception.isKnockback) return;
        isTackling = true;
        lastTackleTime = Time.time;

        if (isMax)
        {
            curentknockbackForce = StrongKnockbackForce;
            isAttack2 = true;
        }
        else
        {
            curentknockbackForce = WeakKnockbackForce;
            isAttack1 = true;
        }

        rb.AddForce(transform.forward * tackleForce, ForceMode.Impulse);

        Invoke("EndTackle", tackleDuration);
    }
    void EndTackle()
    {
        rb.linearVelocity = Vector3.zero;
        isTackling = false;

        //ここで硬直処理
        if (isMax)
        {
            isfinish = true;
        }

        isMax = false;
        isAttack1 = false;
        isAttack2 = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 posDir = other.transform.position - this.transform.position;
            float target_angle = Vector3.Angle(this.transform.forward, posDir);

            var dist = Vector3.Distance(other.transform.position, transform.position);

            if (target_angle > angle) { return; }

            if (target_angle <= angle)
            {
                if (Physics.Raycast(this.transform.position + Vector3.up * 0.5f, posDir, out RaycastHit hit))
                {
                    if (hit.collider == other)
                    {
                        if (isTackling)
                        {
                            Reception p = other.gameObject.GetComponent<Reception>();
                            if (p.isHit) { return; }
                            p.KnockBack(rb.linearVelocity.normalized, curentknockbackForce);

                            //当たった時点でInvokeをキャンセルしてタックルを止める
                            CancelInvoke("EndTackle");
                            EndTackle();
                        }
                    }
                }
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var pos = transform.position;
        pos.y = 1.0f;
        Handles.color = Color.red;
        Handles.DrawSolidArc(pos, Vector3.up, Quaternion.Euler(0.0f, -angle, 0f) * transform.forward, angle * 2f, searchArea.radius);
    }
#endif

    void CollectPlayers()
    {
        players.Clear();

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (obj == gameObject) continue; // 自分は除外
            if (IsOutOfStage(obj.transform.position))
            {
                // ステージ外のプレイヤーはoutPlayersに追加
                if (!outPlayers.Contains(obj))
                    outPlayers.Add(obj);
            }
            else
            {
                players.Add(obj);
            }
        }
    }

    void SearchTarget()
    {
        if (isTackling || isStrt) return;

        target = null;
        minDistance = Mathf.Infinity;

        foreach (GameObject obj in players)
        {
            if (obj == null) continue;

            float dist = Vector3.Distance(transform.position, obj.transform.position);
            if (dist > searchRange) continue;

            if (dist < minDistance)
            {
                minDistance = dist;
                target = obj;
            }
        }

        // targetがステージ外に出た場合はリセットしてoutPlayersに追加
        if (target != null && IsOutOfStage(target.transform.position))
        {
            if (!outPlayers.Contains(target))
                outPlayers.Add(target);
            target = null;
        }
    }

    bool IsOutOfStage(Vector3 pos)
    {
        // x, y, z がすべて範囲内かチェック
        if (pos.x < stageMin.x || pos.x > stageMax.x) return true;
        if (pos.z < stageMin.z || pos.z > stageMax.z) return true;

        return false; // 全部範囲内ならステージ内
    }
    void ResetTarget()
    {
        target = null;

        isStrt = false;
        isPrese = false;
        isMax = false;
        t = 0f;

        isAttack1 = false;
        isAttack2 = false;

        CancelInvoke(nameof(EndTackle));
        isTackling = false;
    }*/
}
