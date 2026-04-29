using UnityEngine;

public class Project3PaddleController : MonoBehaviour
{
    public int slotIndex;

    public float speed = 12f;

    public float minX;
    public float maxX;
    public float minZ = -2.7f;
    public float maxZ = 2.7f;

    void Update()
    {
        if (Project3SlotManager.localClaimedSlot != slotIndex)
        {
            return;
        }

        float moveX = 0f;
        float moveZ = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            moveX = -1f;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            moveX = 1f;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            moveZ = 1f;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            moveZ = -1f;
        }

        if (moveX == 0f && moveZ == 0f)
        {
            return;
        }

        Vector3 direction = new Vector3(moveX, 0f, moveZ);

        if (direction.magnitude > 1f)
        {
            direction.Normalize();
        }

        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);
        newPosition.y = transform.position.y;

        transform.position = newPosition;
    }
}