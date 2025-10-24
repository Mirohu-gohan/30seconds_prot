using UnityEngine;

public class Meteor : MonoBehaviour
{
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
        // もし接触したオブジェクトが他のものであれば
        Destroy(collision.gameObject); // 接触したオブジェクトを消去
        Destroy(gameObject); // 自分自身も消去
    }
}
