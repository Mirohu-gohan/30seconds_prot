using UnityEngine;

public class PlayerHitController : MonoBehaviour
{
    public bool isHit = false;

    private PlayerController1 playerCon1;

    private void Start()
    {
        playerCon1 = GetComponent<PlayerController1>();
    }

   /* public void ChackHit()
    {
        isHit = true;
        Debug.Log("Hit‚µ‚Ü‚µ‚½");
    }*/

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // –³“G’†‚È‚ç‰½‚à‚µ‚È‚¢
            if (playerCon1.isInvincible) return;

            if (!playerCon1.isTackled)
            {
                PlayerHitController playerHitCon =
                    collision.gameObject.GetComponent<PlayerHitController>();

                /*if (playerHitCon != null)
                {
                    playerHitCon.ChackHit();
                }*/
            }
        }

    }
}
