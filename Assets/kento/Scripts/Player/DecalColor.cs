using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class DecalColor : MonoBehaviour
{
    private DecalProjector projector;
    private Renderer render;

    private PlayerInput input;

    void Start()
    {
        projector = GetComponent<DecalProjector>();
        render = GetComponent<Renderer>();

        input = transform.parent.GetComponent<PlayerInput>();

        //Color ranColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        //foreach (var projector in projectors)
        //{
        //    projector.material.SetColor("_BaseColor", ranColor);
        //}

        if (input != null)
        {
            if(input.playerIndex == 1)
            {
                projector.material.SetColor("_BaseColor", Color.red);
                render.material.color = Color.red;
            }
            else if (input.playerIndex == 2)
            {
                projector.material.SetColor("_BaseColor", Color.green);
                render.material.color = Color.green;
            }
            else if (input.playerIndex == 3)
            {
                projector.material.SetColor("_BaseColor", Color.yellow);
                render.material.color = Color.yellow;
            }
            else if (input.playerIndex == 4)
            {
                projector.material.SetColor("_BaseColor", Color.blue);
                render.material.color = Color.blue;
            }
        }

    }
}
