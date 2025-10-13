using UnityEngine;

public class MobileTiltControl : MonoBehaviour
{
    public float sensitivity = 2f;
    public float smoothing = 5f;

    private Quaternion targetRotation;

    void Start()
    {
        targetRotation = transform.rotation;
    }

    void Update() 
    {
        // Read device tilt
        Vector3 accel = Input.acceleration;

        // Adjust axes to match expected orientation
        float tiltX = -accel.x; // invert if needed
        float tiltY = accel.y;

        // Convert to rotation
        Quaternion tiltRotation = Quaternion.Euler(tiltY * 90f * sensitivity, tiltX * 90f * sensitivity, 0f);

        // Smooth rotation
        targetRotation = Quaternion.Slerp(targetRotation, tiltRotation, Time.deltaTime * smoothing);
        transform.rotation = targetRotation;
    }
}
