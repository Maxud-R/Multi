using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensModifier : MonoBehaviour {
	public Slider slider;
	
	public void Apply() {
		GameObject.FindWithTag("MainCamera").GetComponent<CameraMoving>().mouseSens = slider.value;
	}
}
