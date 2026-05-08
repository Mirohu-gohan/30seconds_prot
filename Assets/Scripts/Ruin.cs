using UnityEngine;

public class Ruin : MonoBehaviour
{
    private int con = 0;

    public int durability = 5;

    void Update()
    {
        if (durability == con)
        {
            Destroy(gameObject);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Meteor"))
        {
            con++;
        }
    }
}
