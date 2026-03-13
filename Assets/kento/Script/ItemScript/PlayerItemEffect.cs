using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerItemEffect : MonoBehaviour
{
    Vector3 defaultScale;

    private void Start()
    {
        defaultScale = transform.localScale;
    }

    public void ApplyItem(Item item)
    {
        switch (item.type)
        {
            case Item.Type.RandomBox:
                RandomEfect(item);
                break;
            case Item.Type.SmallBox:
                StartCoroutine(SmallEfect(item));
                break;
            case Item.Type.BigBox:
                StartCoroutine(BigEfect(item));
                break;
        }
    }

    void RandomEfect(Item item)
    {
        int r = Random.Range(0, 2);

        if (r == 0)
        {
            StartCoroutine(BigEfect(item));
        }
        else
        {
            StartCoroutine(SmallEfect(item));

        }
    }
    IEnumerator BigEfect(Item item)
    {
        transform.localScale = defaultScale * item.effectValue;
        yield return new WaitForSeconds(item.duration);
        transform.localScale = defaultScale;
    }

    IEnumerator SmallEfect(Item item)
    {
        transform.localScale = defaultScale / item.effectValue;
        yield return new WaitForSeconds(item.duration);
        transform.localScale = defaultScale;
    }
}
