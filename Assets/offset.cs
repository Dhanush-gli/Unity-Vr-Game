using UnityEngine;

public class CardboardCameraAnchor : MonoBehaviour
{
    [Tooltip("Local position offset of the camera relative to the glider")]
    public Vector3 cameraOffset = new Vector3(0f, 0.8f, 0f);

    [Tooltip("If true, resets rotation to identity (no tilt) each frame")]
    public bool lockRotation = true;

    void LateUpdate()
    {
        // Maintain offset even after Cardboard resets camera each frame
        transform.localPosition = cameraOffset;

        if (lockRotation)
            transform.localRotation = Quaternion.identity;
    }
}
