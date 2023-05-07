using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoving : MonoBehaviour {

	//camera variables
	private float mouseX = 300f;
	private float mouseY = 200f;
	private float CamRotationX = 0f;
	private float CamRotationY = 0f;
	private bool lockedCursor;
	private Vector3 offset = new Vector3(0f, 1.1f, 0f);
	public float mouseSens = 200f;
	
	public Transform cameraObject;
	public GameObject target;
	
	void Start() {
		lockedCursor = true;
	}
	void Update() {
		//Camera rotation
		if (Input.GetButtonDown("Cancel") && Application.isFocused) lockedCursor = !lockedCursor;
		if (lockedCursor && !Application.isFocused) lockedCursor = false;
		if (lockedCursor) {
			if (Cursor.lockState == CursorLockMode.None) Cursor.lockState = CursorLockMode.Locked;
			mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
			mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;
			CamRotationX -= mouseY;
			CamRotationY += mouseX;
			CamRotationX = Mathf.Clamp(CamRotationX, -90f, 90f);
			cameraObject.localRotation = Quaternion.Euler(CamRotationX, CamRotationY, 0f);
			target.transform.Rotate(Vector3.up * mouseX); //amis but easy
		} else {
				Cursor.lockState = CursorLockMode.None;
		}
	}
	void LateUpdate() {
		//Camera motion
		cameraObject.position = target.transform.position + offset;
	}
}
