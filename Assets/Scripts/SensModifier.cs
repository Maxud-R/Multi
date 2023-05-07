using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensModifier : MonoBehaviour
{
	public GameObject cam;
	public Slider slider;
	CameraMoving followScript;
	// Start is called before the first frame update
	void Start()
	{
		followScript = cam.GetComponent("CameraMoving") as CameraMoving;
		followScript.mouseSens = slider.value;
	}

	// Update is called once per frame
	public void Apply() {
		followScript.mouseSens = slider.value;
	}
}
