using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WebBomb : MonoBehaviour {

	public GameObject fragment;
	public Material errorMaterial;
	private GameObject thatFragment;
	private GameObject prevFragment;
	private const float RADIUS = 0.3f;
	private const float DISTANCE = 0.5f;
	private const int LENGTH = 7;
	private const int RAY_COUNT = 5;
	
	void Start() {
		StartCoroutine(Delay());
	}
	
    IEnumerator Delay() {
		yield return new WaitForSeconds(2f); //not needed to stop, it is automatically when destroy;
		PhotonNetwork.Destroy(gameObject); //this bomb will be destroyed in 2 seconds
    }
    
	void OnDestroy() {
		int attempts = 5;
		for (int i = 0; i < RAY_COUNT; i++) {
			Vector3 pos = new Vector3(Random.Range(-RADIUS, RADIUS), Random.Range(-RADIUS, RADIUS), Random.Range(-RADIUS, RADIUS));
			if (Physics.Raycast(transform.position, pos, (float)LENGTH*0.36f)) {
				Vector3 point = transform.position;
				for (int b = 0; b < LENGTH; b++) {
					bool createNext = false;
					if (Physics.Raycast(point, pos, DISTANCE)) createNext = true;
					thatFragment = Instantiate(fragment, point + pos.normalized/1.7f, Quaternion.LookRotation(pos));
					if (createNext) thatFragment.transform.GetChild(0).gameObject.GetComponent<Renderer>().material = errorMaterial;
					if (b > 0) prevFragment.GetComponentInChildren<HingeJoint>().connectedBody = thatFragment.GetComponentInChildren<Rigidbody>();
					point = thatFragment.transform.position;
					prevFragment = thatFragment;
				}
			} else {
				attempts--;
				i--;
			}
			if (attempts <= 0) break;
		}
	}
	
	void OnCollisionEnter(Collision data) {
		if (data.gameObject.name == "PlayerInterface(Clone)") PhotonNetwork.Destroy(gameObject);
		else gameObject.GetComponent<Rigidbody>().isKinematic = true;
	}
}
