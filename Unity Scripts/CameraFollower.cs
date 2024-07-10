using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Transform target; // The target object to follow
    public Vector3 offset = new Vector3(0, 5, -10); // Offset from the target object
    public float smoothSpeed = 0.5f; // Speed of camera movement

    void LateUpdate()
    {
        if (target != null)
        {
            // Calculate the desired position for the camera
            Vector3 desiredPosition = target.position + offset;

            // Smoothly interpolate between current camera position and desired position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            // Move the camera to the smoothed position
            transform.position = smoothedPosition;

            // Ensure the camera always looks at the target object
            transform.LookAt(target);
        }
    }
}
