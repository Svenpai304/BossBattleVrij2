using UnityEngine;

public class ParallaxScroller : MonoBehaviour
{
    private Vector2 startPos;
    private Vector2 length;
    private Camera cam;
    [SerializeField] private Vector2 parallaxEffect; // Use Vector2 to control x and y parallax effects separately

    private void Awake()
    {
        startPos = transform.position;
        length = GetComponent<SpriteRenderer>().bounds.size;
        cam = Camera.main;
    }

    private void FixedUpdate()
    {
        transform.position = new Vector3(cam.transform.position.x * parallaxEffect.x,
            cam.transform.position.y * parallaxEffect.y);

        /*
        Vector2 temp = new Vector2(
            cam.transform.position.x * (1 - parallaxEffect.x),
            cam.transform.position.y * (1 - parallaxEffect.y)
        );

        Vector2 dist = new Vector2(
            cam.transform.position.x * parallaxEffect.x,
            cam.transform.position.y * parallaxEffect.y
        );

        if (temp.x > startPos.x + length.x)
        {
            startPos.x += length.x;
        }
        else if (temp.x < startPos.x - length.x)
        {
            startPos.x -= length.x;
        }

        transform.position = new Vector3(startPos.x + dist.x, startPos.y + dist.y, transform.position.z);*/
    }
}
