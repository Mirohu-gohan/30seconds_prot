using UnityEngine;

public class Landmine : MonoBehaviour
{

    public float explosionForce = 10.0f;
    public float explosionRadius = 3.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("ãNîöÅI");
            Explosion();
        }
    }

    void Explosion()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.AddForce(Vector3.up * explosionForce, ForceMode.VelocityChange);
                }
            }
        }

        Destroy(gameObject);
    }
}
