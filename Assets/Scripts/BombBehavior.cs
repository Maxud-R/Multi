using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BombBehavior : MonoBehaviour {
	public GameObject expl;
	public GameObject fragment;
	private const float RADIUS = 0.3f;
	private bool sticky;
	private bool noExpl;
	
	void Start() {
		StartCoroutine(Delay());
		switch (gameObject.name) {
			case "Bomb2(Clone)": 
				sticky = true;
				break;
			case "Bomb3(Clone)": 
				noExpl = true;
				sticky = true;
				break;
		}
	}
    IEnumerator Delay() {
		yield return new WaitForSeconds(2f); //not needed to stop, it is automatically when destroy;
		PhotonNetwork.Destroy(gameObject); //this bomb will be destroyed in 2 seconds
    }
    void OnDestroy() {
		if (noExpl) {
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
		} else {
			var blast = Instantiate(expl, transform.position, Quaternion.identity);
			Destroy(blast.transform.GetChild(0).gameObject, .1f); //shockwawe
			Destroy(blast, 1f); //destroy whole object (particles) after 1 second
		}
	}
	void OnCollisionEnter(Collision data) {
		if (data.gameObject.name == "PlayerInterface(Clone)") PhotonNetwork.Destroy(gameObject);
		if (sticky && data.gameObject.name != "Bomb2(Clone)") gameObject.GetComponent<Rigidbody>().isKinematic = true;
	}
}
