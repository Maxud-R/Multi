using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyBomb : RegularBomb {
	
	private float scale;
	
    private void Start() {
		StartCoroutine(Delay());
		scale = transform.localScale.x; //bomb is round and symmetrical
    }

	override protected void OnCollisionEnter(Collision data) {
		GetComponent<Rigidbody>().isKinematic = true;
		transform.SetParent(data.gameObject.transform);
		while (transform.parent != null && transform.parent.parent != null) { //with moving objects works only those have no parents, or scaling brokes
			transform.SetParent(transform.parent.parent);
		}
		//restoring original scale after inheritance
		transform.SetLocalPositionAndRotation(transform.localPosition, Quaternion.identity);
		transform.localScale = new Vector3(scale / transform.parent.localScale.x,
														scale / transform.parent.localScale.y,
														scale / transform.parent.localScale.z);
	}
}
