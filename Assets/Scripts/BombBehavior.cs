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
		yield return new WaitForSeconds(2f);
		PhotonNetwork.Destroy(gameObject); //this bomb will be destroyed in 2 seconds
    }
    void OnDestroy() {
		//explosion doesn't have photonView, unity destroy is ok
		Destroy(Instantiate(expl, transform.position, Quaternion.identity), .1f); //destroy explosion after 0.1 second
	}
}
