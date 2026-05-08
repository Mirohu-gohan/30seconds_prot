using UnityEngine;

public class おふざけ : MonoBehaviour
{
    public GameObject button;
    public GameObject message;
    public GameObject img;

    public void Active()
    {
        button.SetActive(false);
        img.SetActive(false);
        message.SetActive(true);
    }
}
