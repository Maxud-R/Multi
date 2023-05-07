using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BombBehavior : MonoBehaviour {
	public GameObject expl;
    void Start() {
		Destroy(gameObject, 2f);
    }
    void OnDestroy() {
		Destroy(Instantiate(expl, transform.position, Quaternion.identity), 0.1f);
	}
}
