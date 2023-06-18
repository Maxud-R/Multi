using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoving : MonoBehaviour {

	//camera variables
	private float mouseX = 300f;
	private float mouseY = 200f;
	public float CamRotationX = 0f;
	public float CamRotationY = 0f;
	public bool lockedCursor;
	private Vector3 offset = new Vector3(0f, 1.1f, 0f);
	public float mouseSens = 3f;
	private Vector3 viewPoint = new Vector3(0f, 5f, 0f);
	
	//in-script defined links
	public GameObject target;
	
	//in-editor defined links 
	public UIScript uiscr;

	void Start() {
		//
	}
	void Update() {
		//Camera rotation
		if (uiscr.lockedCursor) {
			mouseX = Input.GetAxis("Mouse X") * mouseSens;
			mouseY = Input.GetAxis("Mouse Y") * mouseSens;
			CamRotationX -= mouseY;
			CamRotationY += mouseX;
			CamRotationX = Mathf.Clamp(CamRotationX, -90f, 90f);
			transform.localRotation = Quaternion.Euler(CamRotationX, CamRotationY, 0f);
			if (target != null) target.transform.Rotate(Vector3.up * mouseX); //rotating player 
		}
	}
	void LateUpdate() {
		//Camera positioning
		if (target != null) {
			transform.position = target.transform.position + offset;
		} else { //if dead
			transform.position = viewPoint;
		}
	}
}
