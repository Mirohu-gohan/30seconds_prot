using System.Collections;
using UnityEngine;

public class Reception : MonoBehaviour
{
    [Header("ƒmƒbƒNƒoƒbƒN,–³“Gİ’è")]
    private float knockbackTime = 0.3f;
    private float knockbackCounter;

    private Vector3 knockbackDir;
    public bool isKnockback = false;
    public bool isHit = false;

    [SerializeField] private float StunInvincibleTime = 1.0f; //–³“GŠÔ


    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
        isKnockback = true;
        knockbackCounter = knockbackTime;

        knockbackDir = (transform.position - pos).normalized * force;
        rb.linearVelocity = Vector3.zero;
        Debug.Log("–³“GŠJn");
        StartCoroutine(Hit());

    }

    IEnumerator Hit()
    {
        isHit = true;

        yield return new WaitForSeconds(StunInvincibleTime);

        Debug.Log("–³“GI—¹");
        isHit = false;
    }
}
