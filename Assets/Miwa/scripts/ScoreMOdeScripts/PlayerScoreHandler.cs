using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerScoreHandler : MonoBehaviour
{
    private PlayerHealth _health;
    [SerializeField] private GameObject _itemPrefab;
    private string _lastHitTag;
    private void Awake()
    {
        _health = GetComponent<PlayerHealth>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Gimmick") || collision.gameObject.CompareTag("Player"))
        {
            _lastHitTag = collision.gameObject.tag;
        }
    }
    public void HandleDeath()
    {
        int score = ScoreManager.Instance.GetScore(_health.playerIndex);
        int penalty = score / 2;
        if (_lastHitTag == "Gimmick") penalty += 500;
        ScoreManager.Instance.RemoveScore(_health.playerIndex, penalty);
        int count = penalty / 100;
        for (int i = 0; i < count; i++)
        {
            GameObject item = Instantiate(_itemPrefab, transform.position, Quaternion.identity);
            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb)
            {
                Vector3 force = new Vector3(Random.Range(-1f, 1f), 1.5f, Random.Range(-1f, 1f)) * 5f;
                rb.AddForce(force, ForceMode.Impulse);
            }
        }
        _lastHitTag = "";
    }
}
