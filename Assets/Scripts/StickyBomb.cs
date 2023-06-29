using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StickyBomb : MonoBehaviour {
	
	public GameObject expl;
	
    void Start() {
        StartCoroutine(Delay());
    }
    
    IEnumerator Delay() {
		yield return new WaitForSeconds(2f); //not needed to stop, it is automatically when destroy;
		PhotonNetwork.Destroy(gameObject); //this bomb will be destroyed in 2 seconds
    }
    void OnDestroy() {
		GameObject blast = Instantiate(expl, transform.position, Quaternion.identity);
		Destroy(blast.transform.GetChild(0).gameObject, .1f); //shockwawe
		Destroy(blast, 1f); //destroy whole object (particles) after 1 second
	}
	void OnCollisionEnter(Collision data) {
		if (data.gameObject.name == "PlayerInterface(Clone)") PhotonNetwork.Destroy(gameObject);
		else gameObject.GetComponent<Rigidbody>().isKinematic = true;
	}
}
