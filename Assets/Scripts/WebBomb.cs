using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WebBomb : MonoBehaviour {

	private const float RADIUS = 0.3f;
	private const float DISTANCE = 0.6f;
	private const int LENGTH = 7;
	private const int RAY_COUNT = 5;
	public GameObject fragment;
	public Material errorMaterial;
	private GameObject prevFragment;
	private GameObject[,] list = new GameObject[RAY_COUNT, LENGTH];
	private Vector3 throwDir;
	
	void Start() {
		throwDir = transform.position;
	}
	
	void Update () {
		if (Input.GetMouseButtonDown(1)) Destroy(gameObject);
	}
	
    IEnumerator Delay() {
		yield return new WaitForSeconds(2f); 
		Generate();
    }
    void OnDestroy() {
		for (int i = 0; i < RAY_COUNT; i++) {
			for (int b = 0; b < LENGTH; b++) {
				Destroy(list[i, b]);
			}
		}
	}
    
	void Generate() { //TOFIX: rays are generated only when enough room for full length. First fragment too far from bomb.
		int attempts = 5;
		for (int i = 0; i < RAY_COUNT; i++) {
			Vector3 pos = new Vector3(Random.Range(-RADIUS, RADIUS), Random.Range(-RADIUS, RADIUS), Random.Range(-RADIUS, RADIUS));
			pos += throwDir.normalized/2f;
			if (!Physics.Raycast(transform.position, pos, (float)LENGTH*0.36f)) {
				Vector3 point = transform.position;
				for (int b = 0; b < LENGTH; b++) {
					if (!Physics.Raycast(point, pos, DISTANCE)) {
						list[i, b] = Instantiate(fragment, point + pos.normalized/1.7f, Quaternion.LookRotation(pos));
						if (b > 0) prevFragment.GetComponentInChildren<HingeJoint>().connectedBody = list[i, b].GetComponentInChildren<Rigidbody>();
						else list[i, b].GetComponentInChildren<Rigidbody>().isKinematic = true;
						point = list[i, b].transform.position;
						prevFragment = list[i, b];
					} else {
						if (prevFragment != null) prevFragment.GetComponentInChildren<Rigidbody>().isKinematic = true;
					}
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
		else {
			gameObject.GetComponent<Rigidbody>().isKinematic = true;
			throwDir = Vector3.zero;
			//throwDir -= transform.position;
			Generate();
		}
	}
}
