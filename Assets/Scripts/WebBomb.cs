using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WebBomb : MonoBehaviour {

	public GameObject fragment;
	private const float RADIUS = 0.3f;
	
	void Start() {
		StartCoroutine(Delay());
	}
	
    IEnumerator Delay() {
		yield return new WaitForSeconds(2f); //not needed to stop, it is automatically when destroy;
		PhotonNetwork.Destroy(gameObject); //this bomb will be destroyed in 2 seconds
    }
    
    void OnDestroy() {
		int attempts = 10;
		for (int i = 0; i < 5; i++) {
			Vector3 pos = new Vector3(Random.Range(-RADIUS, RADIUS), Random.Range(-RADIUS, RADIUS), Random.Range(-RADIUS, RADIUS));
			if (!Physics.Raycast(transform.position, pos, 0.36f)) {
				GameObject thatFragment = Instantiate(fragment, transform.position + pos.normalized/3f, Quaternion.LookRotation(pos));
			} else {
				i--;
				attempts--;
			}
			if (attempts == 0) break;
		}
	}
	
	void OnCollisionEnter(Collision data) {
		if (data.gameObject.name == "PlayerInterface(Clone)") PhotonNetwork.Destroy(gameObject);
		else gameObject.GetComponent<Rigidbody>().isKinematic = true;
	}
}
