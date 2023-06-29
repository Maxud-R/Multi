using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebGrow : MonoBehaviour {
	
	private bool createNext = true;
	private const float DISTANCE = 0.5f;
	public int count = 7;
	public GameObject fragmentPrefab;
	
    void Start() {
		//I know, that is not too effective way, but I left it as is. p.s. I GONNA DO BETTER
		createNext = !Physics.Raycast(transform.position, transform.up, DISTANCE);
        if (createNext) {
			if (count > 0) {
				GameObject nextFragment = Instantiate(fragmentPrefab, transform.position + transform.up/1.8f, Quaternion.LookRotation(transform.up));
				if (count == 7) {
					GetComponent<Rigidbody>().isKinematic = true;
				}
				GetComponent<HingeJoint>().connectedBody = nextFragment.GetComponentInChildren<Rigidbody>();
				nextFragment.GetComponentInChildren<WebGrow>().count = count-1;
			}
			if (count == 0) {
				Destroy(GetComponent<HingeJoint>());
			}
		} else {
			if (count > 0) GetComponent<Rigidbody>().isKinematic = true;
		}
		Destroy(transform.parent.gameObject, 10f);
    }
}
