using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StickyBomb : MonoBehaviour {
	
	public GameObject expl;
	private float scale;
	private readonly float detonationTime = 20f;
	private readonly float particlesLifetime = 1f;
	private readonly float shockwaveDuration = .1f;
	
    void Start() {
        StartCoroutine(Delay());
		scale = gameObject.transform.localScale.x; //bomb is round and symmetrical
    }
    
    IEnumerator Delay() {
		yield return new WaitForSeconds(detonationTime);
		PhotonNetwork.Destroy(gameObject);
    }
    void OnDestroy() {
		GameObject blast = Instantiate(expl, transform.position, Quaternion.identity);
		Destroy(blast.transform.GetChild(0).gameObject, shockwaveDuration); //destroy only shockwawe
		Destroy(blast, particlesLifetime);
    }
	void OnCollisionEnter(Collision data) {
        gameObject.transform.SetParent(data.gameObject.transform);
		gameObject.GetComponent<Rigidbody>().isKinematic = true;
		if (data.gameObject.transform.parent == null) { //restoring original scale after inheritance
			gameObject.transform.SetLocalPositionAndRotation(gameObject.transform.localPosition, Quaternion.identity);
			gameObject.transform.localScale = new Vector3(scale / gameObject.transform.parent.localScale.x,
				scale / gameObject.transform.parent.localScale.y,
				scale / gameObject.transform.parent.localScale.z);
		}
		Debug.Log(gameObject.transform.localScale / scale);
	}
}
