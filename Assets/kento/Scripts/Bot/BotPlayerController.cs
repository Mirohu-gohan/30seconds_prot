using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BotPlayerController : MonoBehaviour
{
    [Header("ˆÚ“®İ’è")]

    [SerializeField] private float speed = 5.0f; //ˆÚ“®ƒXƒs[ƒh
    [SerializeField] private float ChargeMoveSpeedRate = 0.3f; //ƒ`ƒƒ[ƒWEd’¼’†‚Ì‘¬“x”{—¦
    private float speed2 = 0; //ƒ`ƒƒ[ƒW’†‚ÌƒXƒs[ƒh
    private float curentSpeed = 0;  //Œ»İ‚ÌƒXƒs[ƒh
    [SerializeField] private float rotSpeed = 10.0f; //ù‰ñƒXƒs[ƒh
    [SerializeField] private float ChargeRotateSpeedRate = 0.7f; //ƒ`ƒƒ[ƒWEd’¼’†‚Ìù‰ñ”{—¦
    private float rotSpeed2 = 0;@//ƒ`ƒƒ[ƒW’†ù‰ñƒXƒs[ƒh
    private float curentRotSpeed = 0;//Œ»İ‚Ìù‰ñƒXƒs[ƒh

    [Header("UŒ‚İ’è")]

    [SerializeField] private float tackleForce;    //ƒuƒŠƒ“ƒN—Í
    [SerializeField] private float tackleDuration = 0.5f;//‘±ŠÔ
    [SerializeField] private float tackleCooldown = 1.0f;//ƒN[ƒ‹ƒ_ƒEƒ“ŠÔ

    //-----d’¼-----
    [SerializeField] private float StrongRecoveryTime = 1.0f; //d’¼ŠÔ
    private float curentRecoveryTime;
    private bool isfinish = false;
    private float r;

    private bool isPrese = false; //UŒ‚ƒL[“ü—Íƒtƒ‰ƒO
    [HideInInspector] public bool isStrt = false;//ƒ`ƒƒ[ƒWŠJnƒtƒ‰ƒO
    private float t = 0f; //ƒ`ƒƒ[ƒW—Ê
    [HideInInspector] public float chargeMax = 5.0f; //ƒ`ƒƒ[ƒWãŒÀ
    private bool isMax = false;//ƒ`ƒƒ[ƒW‚ªMax‚©‚Ìƒtƒ‰ƒO

    bool isAttack1 = false;
    bool isAttack2 = false;

    [Header("ƒmƒbƒNƒoƒbƒN,–³“Gİ’è")]
    [SerializeField] private float WeakKnockbackForce = 2.5f; //ãƒuƒŠƒ“ƒNƒmƒbƒNƒoƒbƒN
    [SerializeField] private float StrongKnockbackForce = 5.0f;//‹­ƒuƒŠƒ“ƒNƒmƒbƒNƒoƒbƒN
    private float curentknockbackForce = 0f;//Œ»İ‚ÌƒmƒbƒNƒoƒbƒN—Í


    private Rigidbody rb;
    private bool isTackling = false;
    private float lastTackleTime = 0f; // ÅŒã‚Ìƒ^ƒbƒNƒ‹ŠÔ

    [Header("ƒT[ƒ`İ’è")]
    [SerializeField] private float searchInterval = 0.5f;
    [SerializeField] private float searchRange = 15f;

    private float searchTimer = 0f;

    //-------------------------------------
    [Header("ƒXƒe[ƒW”ÍˆÍ")]
    //lŠpŒ`
    /* [SerializeField] private Vector3 stageMin; // ƒXƒe[ƒW‚ÌÅ¬À•W
     [SerializeField] private Vector3 stageMax; // ƒXƒe[ƒW‚ÌÅ‘åÀ•W*/
    //‰~Œ`
    [SerializeField] private Vector3 stageCenter; // ƒXƒe[ƒW’†S
    [SerializeField] private float stageRadius = 20f; // ƒXƒe[ƒW”¼Œa
    //-------------------------------------

    [Header("“–‚½‚è”»’èİ’è")]
    [SerializeField] private SphereCollider searchArea;
    [SerializeField] private float angle = 45f;

    [Header("ƒGƒtƒFƒNƒg")]
    [SerializeField] private ParticleSystem run;
    [SerializeField] private ParticleSystem charge;
    [SerializeField] private ParticleSystem weak;
    [SerializeField] private ParticleSystem strong;

    //-----‚»‚Ì‘¼-----
    public List<GameObject> players = new List<GameObject>();  //Player’B
    public List<GameObject> outPlayers = new List<GameObject>();  //êŠOPlayer’B
    private float minDistance = Mathf.Infinity; //Å’Z‹——£‚ğ‚¾‚·‚½‚ß‚Ì–ÚˆÀ’l

    public GameObject target;       //UŒ‚‘ÎÛ
    private float distance;          //UŒ‚‘ÎÛ‚Æ‚Ì‹——£

    Reception reception;
    Animator animator;

    GameManager_M gm;
    GameObject ob;


    void Awake()
    {
        speed2 = speed * ChargeMoveSpeedRate;
        rotSpeed2 = rotSpeed * ChargeRotateSpeedRate;
        curentRecoveryTime = StrongRecoveryTime;

        run.Stop();
        charge.Stop();
        weak.Stop();
        strong.Stop();
    }

    public void SetCharge(float value)
    {
        t = value;
    }

    void Start()
    {
        ob = GameObject.Find("Timebox");
        gm = ob.GetComponent<GameManager_M>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        reception = GetComponent<Reception>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager_M.Instance != null && !GameManager_M.Instance.IsGameStartedProperty) return; 
        searchTimer += Time.deltaTime;
        if (searchTimer >= searchInterval)
        {
            CollectPlayers();
            SearchTarget();
            searchTimer = 0f;
        }
        if (target == null) return;

        // ƒXƒe[ƒWŠOƒ`ƒFƒbƒN
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

        if (distance < 15f /*&& distance > r*/)
        {
            if (IsOutOfStage(target.transform.position))
            {
                ResetTarget();
            }
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
        float mag = rb.linearVelocity.magnitude;
        if (mag < 0.01f) { run.Stop(); }
        else if(mag > 0.01f)
        {
            run.Play();
        }
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
                    isStrt = true;
                    charge.Play();
                }

                isPrese = true;
            }
        }
        if (!a)
        {
            isPrese = false;
            if (isStrt && !isTackling && Time.time > lastTackleTime + tackleCooldown)
            {
                charge.Stop();
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
            strong.Play();
            isAttack2 = true;
        }
        else
        {
            curentknockbackForce = WeakKnockbackForce;
            weak.Play();
            isAttack1 = true;
        }

        if (gm.CurrentModeState == GameManager_M.Mode.SuddenDeath)
        {
            curentknockbackForce *= 10f;
        }

        rb.AddForce(transform.forward * tackleForce, ForceMode.Impulse);

        Invoke("EndTackle", tackleDuration);
    }
    void EndTackle()
    {
        rb.linearVelocity = Vector3.zero;
        isTackling = false;
        strong.Stop();
        weak.Stop();

        //‚±‚±‚Åd’¼ˆ—
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

                            //“–‚½‚Á‚½“_‚ÅInvoke‚ğƒLƒƒƒ“ƒZƒ‹‚µ‚Äƒ^ƒbƒNƒ‹‚ğ~‚ß‚é
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

        // ===== ƒXƒe[ƒW”ÍˆÍi’Ç‰Áj =====
        Handles.color = Color.green;

        Vector3 center = stageCenter;
        center.y = 0f; // XZ•½–Ê‚ÉŒÅ’è

        // ‰~‚ÌŠO˜g
        Handles.DrawWireDisc(center, Vector3.up, stageRadius);

        // ’†S“_
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(center, 0.3f);
    }
#endif

    void CollectPlayers()
    {
        players.Clear();

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (obj == gameObject) continue; // ©•ª‚ÍœŠO
            if (IsOutOfStage(obj.transform.position))
            {
                // ƒXƒe[ƒWŠO‚ÌƒvƒŒƒCƒ„[‚ÍoutPlayers‚É’Ç‰Á
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
        //if (isTackling || isStrt) return;

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

        // target‚ªƒXƒe[ƒWŠO‚Éo‚½ê‡‚ÍƒŠƒZƒbƒg‚µ‚ÄoutPlayers‚É’Ç‰Á
        if (target != null && IsOutOfStage(target.transform.position))
        {
            if (!outPlayers.Contains(target))
                outPlayers.Add(target);
            target = null;
        }
    }
    //-------------------------------------

    bool IsOutOfStage(Vector3 pos)
    {
        /* // x,z ‚ª‚·‚×‚Ä”ÍˆÍ“à‚©ƒ`ƒFƒbƒN
         if (pos.x < stageMin.x || pos.x > stageMax.x) return true;
         if (pos.z < stageMin.z || pos.z > stageMax.z) return true;

         return false; // ‘S•””ÍˆÍ“à‚È‚çƒXƒe[ƒW“à*/

        // Y‚Í–³‹‚µ‚ÄXZ•½–Ê‚¾‚¯‚Å”»’è
        //Vector3 centerXZ = new Vector3(stageCenter.x, 0f, stageCenter.z);
        Vector3 posXZ = new Vector3(pos.x, 0f, pos.z);

        float distance = Vector3.Distance(stageCenter, posXZ);

        return distance > stageRadius;
    }
    //-------------------------------------

    void ResetTarget()
    {
        target = null;

        isStrt = false;
        isPrese = false;
        isMax = false;
        t = 0f;
        r = 0f;

        isAttack1 = false;
        isAttack2 = false;

        CancelInvoke(nameof(EndTackle));
        isTackling = false;
    }


    public void ResetBotState()
    {
        // 1. å…¨ã¦ã®å…¥åŠ›ãƒ»è¡Œå‹•ãƒ•ãƒ©ã‚°ã‚’æŠ˜ã‚‹
        isPrese = false;
        isStrt = false;
        isTackling = false;
        isMax = false;
        isfinish = false;
        isAttack1 = false;
        isAttack2 = false;

        // 2. æ•°å€¤ãƒ‘ãƒ©ãƒ¡ãƒ¼ã‚¿ã®ãƒªã‚»ãƒƒãƒˆ
        t = 0f;
        r = 0f;
        searchTimer = 0f;
        lastTackleTime = 0f;
        curentRecoveryTime = StrongRecoveryTime;

        // 3. ã‚¿ãƒ¼ã‚²ãƒƒãƒˆã‚’ã‚¯ãƒªã‚¢
        target = null;
        players.Clear();
        outPlayers.Clear();

        // 4. å…¨ã‚¨ãƒ•ã‚§ã‚¯ãƒˆã‚’å¼·åˆ¶åœæ­¢
        if (run != null) run.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (charge != null) charge.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (weak != null) weak.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (strong != null) strong.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // 5. Invoke (EndTackle) ã®ã‚­ãƒ£ãƒ³ã‚»ãƒ«
        CancelInvoke(nameof(EndTackle));

        // 6. ç‰©ç†ã®ãƒªã‚»ãƒƒãƒˆ
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 7. ã‚¢ãƒ‹ãƒ¡ãƒ¼ã‚¿ãƒ¼ã®ãƒªã‚»ãƒƒãƒˆ
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetBool("IsChage", false);
            animator.SetBool("isAttack1", false);
            animator.SetBool("isAttack2", false);
        }
    }
}
