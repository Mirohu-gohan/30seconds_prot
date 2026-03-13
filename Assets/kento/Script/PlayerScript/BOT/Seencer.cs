using UnityEngine;

public class Seencer : MonoBehaviour
{
    private int layer;
    bool isGround = false;

    private void Start()
    {
        layer = LayerMask.NameToLayer("Stage");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == layer)
        {
            isGround = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == layer)
        {
            isGround = false;
        }
    }

    public bool CheckLayer()
    {
        return isGround;
    }
}
