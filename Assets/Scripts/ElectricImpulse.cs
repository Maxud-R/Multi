using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricImpulse : MonoBehaviour {
	
	private Rigidbody fragment;
	public bool doImpulse = false;
	
    void Start() {
		if (!doImpulse) Destroy(this);
		fragment = GetComponent<Rigidbody>();
        StartCoroutine(Impulse());
    }
    
    IEnumerator Impulse() {
		for (;;) {
			float dir =  Random.Range(4f, 9f);
			fragment.AddForce(new Vector3(dir, dir, dir), ForceMode.Impulse);
			yield return new WaitForSeconds(Random.Range(0.7f, 2f));
		}
	}
}
