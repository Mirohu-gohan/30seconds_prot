using UnityEngine;

public class Durability : MonoBehaviour
{
    [SerializeField]
    private int durability;

    private int count = 0;

    void Update()
    {
        if ((durability - count) == 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Meteor"))
        {
            count++;

        }
    }
}
