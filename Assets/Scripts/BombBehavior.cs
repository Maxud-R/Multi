using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BombBehavior : MonoBehaviour {
	public GameObject expl;
    void Start() {
		Destroy(gameObject, 2f); //this bomb will be destroyed in 2 seconds
    }
    void OnDestroy() {
		Destroy(Instantiate(expl, transform.position, Quaternion.identity), .1f); //destroy explosion after 0.1 second
	}
}
