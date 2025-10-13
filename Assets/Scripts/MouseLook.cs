using System;
using UnityEngine;

public class MouseLook : MonoBehaviour {
	public float mouseSensitivity = 100.0f;
	public float clampAngle = 80.0f;
	public float angle_of_bound = 40.0f;
	
	private float rotY = 0.0f; // rotation around the up/y axis
	private float rotX = 0.0f; // rotation around the right/x axis
    public bool islookdown = false;
    public bool islookup = false;

    void Start () {
		Vector3 rot = transform.localRotation.eulerAngles;
		rotY = rot.y;
		rotX = rot.x;
		Cursor.visible = false;
	}

	void Update() {
		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = -Input.GetAxis("Mouse Y");

		rotY += mouseX * mouseSensitivity * Time.deltaTime;
		rotX += mouseY * mouseSensitivity * Time.deltaTime;

		rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

		Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
		transform.localRotation = localRotation;

		Quaternion camera_rot = transform.rotation;

		if (camera_rot.eulerAngles.x > angle_of_bound && camera_rot.eulerAngles.x < (360 - angle_of_bound))
		{
			if (camera_rot.eulerAngles.x < 120)
			{

				//Debug.Log(camera_rot.eulerAngles);
				//Debug.Log("Camera Down  " + Time.deltaTime);
				islookdown = true;
			}
			//else { islookdown = false; };

			if (camera_rot.eulerAngles.x > (360 - 120))
			{
				//Debug.Log(camera_rot.eulerAngles);
				//Debug.Log("Camera up  " + Time.deltaTime);
				islookup = true;
			}
			//else { islookup = false; };

		}
		else {islookdown = false; islookup = false; };

		//Debug.Log("Is Look Up: " + islookup + "  Is Look Dn: " + islookdown);
    }

	
	
}
