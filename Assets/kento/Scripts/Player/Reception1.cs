using System.Collections;
using UnityEngine;

public class Reception1 : MonoBehaviour
{
    [Header("ノックバック,無敵設定")]
    private float knockbackTime = 0.3f;
    private float knockbackCounter;

    private Vector3 knockbackDir;
    public bool isKnockback = false;
    public bool isHit = false;

    private Collider col;

    [SerializeField] private float StunInvincibleTime = 1.0f; //無敵時間

    [SerializeField] private ParticleSystem hit;

    Rigidbody rb;
    Animator animator;

    void Start()
    {
        hit.Stop();

        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        col = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
      
        if (isKnockback)
        {
            knockbackCounter -= Time.deltaTime;
            //rb.linearVelocity = knockbackDir * curentknockbackForce;
            if (knockbackCounter <= 0)
            {
                isKnockback = false;
                rb.linearVelocity = Vector3.zero;
            }
        }

        if (!animator) { return; }
        animator.SetBool("IsHit",isKnockback);
    }

    private void FixedUpdate()
    {
        if (isKnockback)
        {
            rb.linearVelocity = knockbackDir;
        }
    }

    public void KnockBack(Vector3 pos, float force)
    {
        if (hit != null)
        {
            hit.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            hit.Play(true);
        }

        isKnockback = true;
        knockbackCounter = knockbackTime;

        knockbackDir = pos.normalized * force;
        rb.linearVelocity = Vector3.zero;
        StopCoroutine(nameof(Hit));
        StartCoroutine(Hit());
    }

    IEnumerator Hit()
    {
        isHit = true;
        col.enabled = false;
        rb.useGravity = false;
        yield return new WaitForSeconds(StunInvincibleTime);

        rb.useGravity = true;
        col.enabled = true;
        isHit = false;
        if (hit != null)
        {
            hit.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
