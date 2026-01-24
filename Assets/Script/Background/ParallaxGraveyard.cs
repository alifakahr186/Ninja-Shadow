using UnityEngine;

public class ParallaxGraveyard : MonoBehaviour
{
    [Range(0f, 0.5f)]
    public float parallaxRange = 0.15f;
    // 0   = bilkul door (almost no parallax)
    // 0.5 = thora kareeb feel

    private Vector3 startPos;
    private float camStartX;

    void Start()
    {
        startPos = transform.position;
        camStartX = Camera.main.transform.position.x;
    }

    void LateUpdate()
    {
        float camDeltaX = Camera.main.transform.position.x - camStartX;

        // sirf soft depth offset — object apni jagah hi rehta hai
        float depthOffset = camDeltaX * parallaxRange;

        transform.position = new Vector3(
            startPos.x + depthOffset,
            startPos.y,
            startPos.z
        );
    }
}
