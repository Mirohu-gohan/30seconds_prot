using UnityEngine;

public class Meteor : MonoBehaviour
{
    public GameObject Smoke;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Stage"))
        {
            // もし接触したオブジェクトが他のものであれば
            Destroy(collision.gameObject); // 接触したオブジェクトを消去
            Destroy(gameObject); // 自分自身も消去

            Vector3 contactPoint = collision.contacts[0].point;

            if (Smoke != null)
            {
                Instantiate(Smoke, contactPoint, Quaternion.identity);
            }
        }
        
    }
}
