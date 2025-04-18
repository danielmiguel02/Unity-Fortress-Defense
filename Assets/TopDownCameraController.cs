using System.Collections.Generic;
using UnityEngine;

public class TopDownCameraController : MonoBehaviour
{
    private Camera camera;
    [SerializeField] Transform boundaries;
    [SerializeField] float moveSpeed = 45f;
    [SerializeField] float rSpeed = 45f;

    private List<Transform> boundariesList;

    private void Start()
    {
        camera = Camera.main;

        boundariesList = new List<Transform>();
        foreach (Transform t in boundaries)
        {
            boundariesList.Add(t);
        }
    }

    private void Update()
    {
        if (camera == null) return;

        // Camera rotation (Q/E)
        if (Input.GetKey(KeyCode.Q))
            camera.transform.Rotate(Vector3.up, -rSpeed * Time.deltaTime, Space.World);
        if (Input.GetKey(KeyCode.E))
            camera.transform.Rotate(Vector3.up, rSpeed * Time.deltaTime, Space.World);

        // Handle movement (W, A, S, D)
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = 0f;

        if (Input.GetKey(KeyCode.W)) verticalInput = 1f;
        else if (Input.GetKey(KeyCode.S)) verticalInput = -1f;

        Vector3 forward = camera.transform.forward;
        Vector3 right = camera.transform.right;

        forward.y = 0f;  // Ignore vertical component
        right.y = 0f;    // Ignore vertical component

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * verticalInput + right * horizontalInput).normalized;

        // Calculate the desired position
        Vector3 targetPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;  // Using Time.deltaTime for smoothness

        // 🔎 Raycast to check if the path is clear
        RaycastHit hit;
        float checkDistance = moveSpeed * Time.deltaTime * 1.5f;  // Moderate check distance

        if (Physics.Raycast(transform.position, moveDirection, out hit, checkDistance))
        {
            // If ray hits something (boundary), prevent movement in that direction
            Debug.Log("Movement blocked by boundary");
            targetPosition = transform.position;  // Don't move if blocked
        }

        // Smoothly move the camera towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.3f);  // Moderate smoothing factor
    }


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Camera collided with: " + collision.gameObject.name);
    }
}
