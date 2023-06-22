using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BombBehavior : MonoBehaviour {
	public GameObject expl;
	
	void Start() {
		StartCoroutine(Delay());
	}
    IEnumerator Delay() {
		yield return new WaitForSeconds(5f); //not needed to stop, it is automatically when destroy;
		PhotonNetwork.Destroy(gameObject); //this bomb will be destroyed in 2 seconds
    }
    void OnDestroy() {
		//explosion doesn't have photonView, unity destroy is ok
		var blast = Instantiate(expl, transform.position, Quaternion.identity);
		Destroy(blast.transform.GetChild(0).gameObject, .1f); //shockwawe
		Destroy(blast, 1f); //destroy whole object (particles) after 1 second
	}
	void OnCollisionEnter(Collision data) {
		if (data.gameObject.name == "PlayerInterface(Clone)") PhotonNetwork.Destroy(gameObject);
		if (gameObject.name == "Bomb2(Clone)" && data.gameObject.name != "Bomb2(Clone)") gameObject.GetComponent<Rigidbody>().isKinematic = true;
	}
}
