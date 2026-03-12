using UnityEngine;
public class ScoreItem : MonoBehaviour
{
    [SerializeField] private int _value = 100;
    private void OnTriggerEnter(Collider other)
    {
        var health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            ScoreManager.Instance.AddScore(health.playerIndex, _value);
            Destroy(gameObject);
        }
    }
}
