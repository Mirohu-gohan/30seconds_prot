using UnityEngine;

public class FrameAnimator : MonoBehaviour
{
    [Header("égópëfçﬁâÊëú")]
    public Texture2D[] frames;
    public float fps = 60f;

    private Renderer rend;
    private int currentFrame = 0;
    private float timer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material = new Material(Shader.Find("Unlit/Transparent"));

        if (frames.Length > 0 )
        {
            rend.material.mainTexture = frames[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f / fps)
        {
            timer -= 1f / fps;
            currentFrame++;

            if (currentFrame >= frames.Length)
            {
                Destroy(gameObject);
                return;
            }

            rend.material.mainTexture = frames[currentFrame];
        }
    }
}
