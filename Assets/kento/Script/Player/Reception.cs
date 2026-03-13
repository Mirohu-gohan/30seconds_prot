using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Reception : MonoBehaviour
{
    [Header("ノックバック,無敵設定")]
    private float knockbackTime = 0.3f;
    private float knockbackCounter;

    private Vector3 knockbackDir;


    [SerializeField] private float StunInvincibleTime = 1.0f; //無敵時間
    bool isKonckback = false;
    private bool isHit = false;
    Collider col;
    Rigidbody rb;
    //Animator animator;

    //-----Script参照-----
    private PlayerStateManager stateManager;
    private ChargeSpike cs;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        //animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
        stateManager = GetComponent<PlayerStateManager>();
        cs = GetComponent<ChargeSpike>();
    }

    private void Update()
    {
        if (isKonckback)
        {
            knockbackCounter -= Time.deltaTime;
            if (knockbackCounter <= 0)
            {
                isKonckback = false;
                stateManager.SetState(State.None);
                rb.linearVelocity = Vector3.zero;
            }
        }
    }
    private void FixedUpdate()
    {
        if(isKonckback)
        {
            rb.linearVelocity = knockbackDir;
        }
    }

    public void KnockBack(Vector3 pos,float force)
    {
        if (isHit) return;

        isKonckback = true;
        stateManager.SetState(State.Knockback);
        knockbackCounter = knockbackTime;
        Debug.Log("ノックバック");
        knockbackDir = pos.normalized * force;
        rb.linearVelocity = Vector3.zero;
        StartCoroutine(Hit());
    }

    IEnumerator Hit()
    {
        isHit = true;
        yield return new WaitForSeconds(0.05f);
        col.enabled = false;
        rb.useGravity = false;
        yield return new WaitForSeconds(StunInvincibleTime);

        rb.useGravity = true;
        col.enabled = true;
        isHit = false;
    }
}
