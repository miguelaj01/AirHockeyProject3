using UnityEngine;

public class SimpleAIPaddleController : MonoBehaviour
{
    public Transform puck;
    public float speed = 4.5f;

    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;

    private Vector3 homePosition;

    void Start()
    {
        homePosition = transform.position;
    }

    void Update()
    {
        if (puck == null)
        {
            return;
        }

        Vector3 target = homePosition;

        bool puckInMyZone = puck.position.x >= minX && puck.position.x <= maxX;

        if (puckInMyZone)
        {
            target = puck.position;
        }
        else
        {
            target.z = puck.position.z;
        }

        target.y = transform.position.y;
        target.x = Mathf.Clamp(target.x, minX, maxX);
        target.z = Mathf.Clamp(target.z, minZ, maxZ);

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
}
