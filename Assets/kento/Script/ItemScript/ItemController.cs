using UnityEngine;



public class ItemController : MonoBehaviour
{
    public Item item;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerItemEffect player = other.GetComponent<PlayerItemEffect>();

            if (player != null)
            {
                player.ApplyItem(item);
                Destroy(gameObject);
            }
        }
        
    }
}
