using UnityEngine;

public class damageMat : MonoBehaviour
{
    public Material[] materials;  // Inspector‚Åƒ}ƒeƒŠƒAƒ‹‚ð•À‚×‚é
    int currentIndex = 0;
    Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (materials.Length > 0)
            rend.material = materials[0];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            NextMaterial();
    }

    public void NextMaterial()
    {
        if (materials.Length == 0) return;
        currentIndex = (currentIndex + 1) % materials.Length;
        rend.material = materials[currentIndex];
    }
}