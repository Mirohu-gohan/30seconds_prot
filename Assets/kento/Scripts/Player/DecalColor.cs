using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class DecalColor : MonoBehaviour
{
    private DecalProjector projector;
    [SerializeField] private Material matreial1;
    [SerializeField] private Material matreial2;
    [SerializeField] private Material matreial3;
    [SerializeField] private Material matreial4;

    private PlayerInput input;

    void Start()
    {
        projector = GetComponent<DecalProjector>();

        input = transform.parent.GetComponent<PlayerInput>();


        if (input != null)
        {
            if (input.playerIndex == 0)
            {
                projector.material = matreial1;
            }
            else if (input.playerIndex == 1)
            {
                projector.material = matreial2;
            }
            else if (input.playerIndex == 2)
            {
                projector.material = matreial3;
            }
            else if (input.playerIndex == 3)
            {
                projector.material = matreial4;
            }
        }

    }
}
