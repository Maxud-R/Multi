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
	public LayerMask layerMask;
	private GameObject prevFragment;
	private GameObject[,] list = new GameObject[RAY_COUNT, LENGTH];
	private Vector3 throwDir;
	
	void Start() {
		//throwDir = transform.position;
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
    
	void Generate() {
		int attempts = 10;
		int[] sizearray = new int[RAY_COUNT];
		for (int i = 0; i < RAY_COUNT; i++) {
			GameObject[] pair = new GameObject[2];
			Vector3 connectionDistance;
			GameObject prevConFragment; 
			int[] rnd = new int[2];
			Vector3 pos = new Vector3(Random.Range(-RADIUS, RADIUS), Random.Range(-RADIUS, RADIUS), Random.Range(-RADIUS, RADIUS));
			pos += throwDir/3f;
			if (!Physics.Raycast(transform.position, pos, 0.36f, layerMask) && Physics.Raycast(transform.position, pos, (float)LENGTH*0.6f, layerMask)) { //proceed only we can generate first fragment and obstacle not too far for sticking to
				Vector3 point = transform.position;
				for (int b = 0; b < LENGTH; b++) {
					if (!Physics.Raycast(point, pos, DISTANCE, layerMask)) { //if next fragment will not overlap obstacle
						Vector3 pointSum = point + pos.normalized/1.7f;
						if (b == 0) pointSum = (pointSum - transform.position)*0.42f + transform.position; //first fragmen distance fix
						list[i, b] = Instantiate(fragment, pointSum, Quaternion.LookRotation(pos));
						if (b > 0) prevFragment.GetComponentInChildren<HingeJoint>().connectedBody = list[i, b].GetComponentInChildren<Rigidbody>();
						else list[i, b].GetComponentInChildren<Rigidbody>().isKinematic = true; //first fragment connection to the world
						point = list[i, b].transform.position;
						prevFragment = list[i, b];
						sizearray[i]++;
					} else {
						if (prevFragment != null) prevFragment.GetComponentInChildren<Rigidbody>().isKinematic = true;
					}
				}
				if (i > 0 && attempts > 0 && sizearray[i] > 0 && sizearray[i-1] > 0) {
					rnd[0] = Random.Range(0, sizearray[i]);
					rnd[1] = Random.Range(0, sizearray[i-1]);
					pair[0] = list[i, rnd[0]];
					pair[1] = list[i - 1, rnd[1]];
					Debug.Log("rnd[0]:"+rnd[0]+", sizearray[i]:"+sizearray[i]);
					Debug.Log("rnd[1]:"+rnd[1]+", sizearray[i-1]:"+sizearray[i-1]);
					connectionDistance = ((pair[1].transform.position - pair[0].transform.position)/Vector3.Distance(pair[0].transform.position, pair[1].transform.position))/1.7f;
					prevConFragment = pair[0];
					Debug.DrawRay(pair[0].transform.position, pair[1].transform.position, Color.blue, 4f);
					for (int c = 0; c < Mathf.Round(Vector3.Distance(pair[0].transform.position, pair[1].transform.position)/0.6f); c++) {;
						if (c == 0) {
							prevConFragment = Instantiate(fragment, pair[0].transform.position + connectionDistance * c, Quaternion.LookRotation(connectionDistance));
							pair[0].transform.GetChild(0).gameObject.AddComponent<HingeJoint>().connectedBody = prevConFragment.GetComponentInChildren<Rigidbody>();
						} else {
							prevConFragment.GetComponentInChildren<HingeJoint>().connectedBody = Instantiate(fragment, pair[0].transform.position + connectionDistance * c, Quaternion.LookRotation(connectionDistance)).GetComponentInChildren<Rigidbody>();
							prevConFragment = prevConFragment.GetComponentInChildren<HingeJoint>().connectedBody.gameObject;
							
						}
						if (c == Mathf.Round(Vector3.Distance(pair[0].transform.position, pair[1].transform.position)/0.6f)-1) {
							prevConFragment.GetComponentInChildren<HingeJoint>().connectedBody = pair[1].GetComponentInChildren<Rigidbody>();
						}
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
			throwDir = data.GetContact(0).normal;
			//Debug.DrawRay(transform.position, throwDir/3f, Color.blue, 10f);
			Generate();
		}
	}
}
