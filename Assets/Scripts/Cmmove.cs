using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ClampedFullGliderFixed : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public Transform gliderBody;

    [Header("Physics")]
    public float liftCoefficient = 0.3f;
    public float dragCoefficient = 0.05f;
    public float thrustPower = 4f;
    public float gravity = 9.81f;
    public float mass = 1f;
    public float maxLiftMultiplier = 2f;

    [Header("Control & Clamping")]
    public float turnSmoothness = 2f;
    public float bankAmount = 25f;
    public float pitchMin = -20f;
    public float pitchMax = 30f;

    [Header("Yaw Rotation Speed")]
    public float yawAngleLimit = 45f;     // angle at which rotation speed caps
    public float minYawSpeed = 2f;        // rotation speed for small angle
    public float maxYawSpeed = 3.5f;      // max rotation speed

    [Header("Delay Settings")]
    public float startDelay = 5f;

    private Rigidbody rb;
    private float timer = 0f;
    private bool physicsEnabled = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = mass;
        rb.useGravity = false;
        rb.drag = 0.01f;
        rb.angularDrag = 2f;
        rb.velocity = Vector3.zero;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    void FixedUpdate()
    {
        if (!cameraTransform || !gliderBody) return;

        // --- 5-second delay ---
        if (!physicsEnabled)
        {
            timer += Time.fixedDeltaTime;
            if (timer >= startDelay)
            {
                physicsEnabled = true;
                rb.velocity = gliderBody.forward * 2f;
            }
            return;
        }

        // --- Camera rotation ---
        Vector3 currentEuler = gliderBody.rotation.eulerAngles;

        // --- Clamp pitch ---
        float camPitch = cameraTransform.eulerAngles.x;
        if (camPitch > 180) camPitch -= 360;
        camPitch = Mathf.Clamp(camPitch, pitchMin, pitchMax);

        float newPitch = Mathf.LerpAngle(currentEuler.x, camPitch, Time.fixedDeltaTime * turnSmoothness);

        // --- Yaw rotation with proportional speed and clamping ---
        float targetYaw = cameraTransform.eulerAngles.y;
        if (targetYaw > 180) targetYaw -= 360;

        float yawDelta = Mathf.DeltaAngle(currentEuler.y, targetYaw);
        float absYawDelta = Mathf.Abs(yawDelta);

        // Scale rotation speed proportional to angle, clamp at max
        float rotSpeed = absYawDelta < yawAngleLimit
            ? minYawSpeed * (absYawDelta / yawAngleLimit)
            : maxYawSpeed;

        // Clamp yawDelta to maximum allowed rotation per frame
        yawDelta = Mathf.Clamp(yawDelta, -rotSpeed, rotSpeed);

        float newYaw = currentEuler.y + yawDelta;

        // --- Banking ---
        float targetBank = Mathf.Clamp(-yawDelta * 0.5f, -bankAmount, bankAmount);
        float newRoll = Mathf.LerpAngle(currentEuler.z, targetBank, Time.fixedDeltaTime * turnSmoothness);

        gliderBody.rotation = Quaternion.Euler(newPitch, newYaw, newRoll);

        // --- Forces ---
        Vector3 forward = gliderBody.forward;
        float airspeed = rb.velocity.magnitude;

        float liftMag = liftCoefficient * airspeed * airspeed * Mathf.Max(Vector3.Dot(forward, Vector3.up), -0.5f);
        liftMag = Mathf.Clamp(liftMag, -mass * gravity * maxLiftMultiplier, mass * gravity * maxLiftMultiplier);
        Vector3 liftForce = Vector3.up * liftMag;

        Vector3 dragForce = Vector3.zero;
        if (rb.velocity.sqrMagnitude > 0.0001f)
            dragForce = -rb.velocity.normalized * dragCoefficient * airspeed * airspeed;

        Vector3 thrustForce = forward * thrustPower;

        float pitchRad = Mathf.Deg2Rad * newPitch;
        Vector3 forwardGravity = Vector3.down * mass * gravity * Mathf.Sin(pitchRad);
        Vector3 gravityForce = Vector3.down * mass * gravity + forwardGravity * 0.5f;

        Vector3 totalForce = liftForce + dragForce + thrustForce + gravityForce;

        rb.AddForce(totalForce * Time.fixedDeltaTime * 50f);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, gliderBody.rotation, Time.fixedDeltaTime * turnSmoothness));
    }

    void OnDrawGizmos()
    {
        if (!gliderBody) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + gliderBody.forward * 3f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + gliderBody.up * 3f);
    }
}