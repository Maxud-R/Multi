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
	public LayerMask layerMask;
	private GameObject prevFragment;
	private GameObject[,] list = new GameObject[RAY_COUNT, LENGTH];
	private Vector3 throwDir;
	private int[] sizearr = new int[RAY_COUNT];
	
	void Start() {
		//
	}
	
	void Update () {
		if (Input.GetMouseButtonDown(1)) Destroy(gameObject);
	}
	
    void OnDestroy() {
		for (int i = 0; i < RAY_COUNT; i++) {
			for (int b = 0; b < LENGTH; b++) {
				Destroy(list[i, b]);
			}
		}
	}
    
	void Generate() {
		int attempts = 10;
		for (int i = 0; i < RAY_COUNT; i++) {
			Vector3 pos = new Vector3(Random.Range(-RADIUS, RADIUS), Random.Range(-RADIUS, RADIUS), Random.Range(-RADIUS, RADIUS));
			pos += throwDir/3f;
			if (!Physics.Raycast(transform.position, pos, 0.36f, layerMask) && Physics.Raycast(transform.position, pos, (float)LENGTH*0.6f, layerMask)) { //proceed only we can generate first fragment and obstacle not too far for sticking to
				Vector3 point = transform.position;
				for (int b = 0; b < LENGTH; b++) {
					if (!Physics.Raycast(point, pos, DISTANCE, layerMask)) { //if next fragment will not overlap obstacle
						Vector3 pointSum = point + pos.normalized/1.8f;
						if (b == 0) pointSum = (pointSum - transform.position)*0.42f + transform.position; //first fragmen distance fix
						list[i, b] = Instantiate(fragment, pointSum, Quaternion.LookRotation(pos));
						if (b > 0) prevFragment.GetComponentInChildren<HingeJoint>().connectedBody = list[i, b].GetComponentInChildren<Rigidbody>();
						else list[i, b].GetComponentInChildren<Rigidbody>().isKinematic = true; //first fragment connection to the world
						point = list[i, b].transform.position;
						prevFragment = list[i, b];
						sizearr[i]++;
					} else {
						if (prevFragment != null) prevFragment.GetComponentInChildren<Rigidbody>().isKinematic = true;
					}
				}
				if (sizearr[i] > 1) list[i, 1].GetComponentInChildren<ElectricImpulse>().doImpulse = true;
				if (sizearr[i] > 3) list[i, sizearr[i]-2].GetComponentInChildren<ElectricImpulse>().doImpulse = true;
			} else {
				attempts--;
				i--;
			}
			if (attempts <= 0) break;
		}
	}
	
	void OnCollisionEnter(Collision data) {
		if (data.gameObject.name == "PlayerInterface(Clone)") Debug.Log("A");//PhotonNetwork.Destroy(gameObject);
		else {
			gameObject.GetComponent<Rigidbody>().isKinematic = true;
			throwDir = data.GetContact(0).normal;
			Generate();
			for (int i = 0; i < sizearr.Length; i++) {
				return;
			}
		}
	}
}
