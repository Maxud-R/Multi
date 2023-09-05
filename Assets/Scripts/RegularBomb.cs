using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RegularBomb : MonoBehaviour {
	
	public GameObject expl;
	protected readonly float detonationTime = 2f;
	protected readonly float particlesLifetime = 1f;
	protected readonly float shockwaveDuration = .1f;
	
    private void Start() {
        StartCoroutine(Delay());
    }
    
    protected IEnumerator Delay() {
		yield return new WaitForSeconds(detonationTime);
		PhotonNetwork.Destroy(gameObject); //this bomb will be destroyed in x seconds
    }
    protected void OnDestroy() {
		GameObject blast = Instantiate(expl, transform.position, Quaternion.identity);
		Destroy(blast.transform.GetChild(0).gameObject, shockwaveDuration); //destroy shockwawe only
		Destroy(blast, particlesLifetime); //destroy whole object (particles) after x seconds
	}
	virtual protected void OnCollisionEnter(Collision data) {
		if (data.gameObject.name == "PlayerInterface(Clone)") PhotonNetwork.Destroy(gameObject);
	}
}
