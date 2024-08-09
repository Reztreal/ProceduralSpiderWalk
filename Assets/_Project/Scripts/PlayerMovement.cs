using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 100f;

    private void Update()
    {
        // Read input
        float moveDirection = Input.GetAxis("Vertical"); // W and S keys
        float rotationDirection = Input.GetAxis("Horizontal"); // A and D keys

        // Calculate movement and rotation
        Vector3 movement = transform.forward * moveDirection * moveSpeed * Time.deltaTime;
        float rotation = rotationDirection * rotationSpeed * Time.deltaTime;

        // Apply movement and rotation
        transform.position += movement;
        transform.Rotate(0, rotation, 0);
    }
}
