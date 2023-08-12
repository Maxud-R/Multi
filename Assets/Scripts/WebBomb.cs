using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WebBomb : MonoBehaviour {

	private const float RADIUS = 0.3f;
	private const float DISTANCE = 0.6f;
	private const int LENGTH = 12;
	private const int RAY_COUNT = 5;
	public GameObject fragment;
	public LayerMask layerMask;
	private GameObject prevFragment;
	private GameObject[,] list = new GameObject[RAY_COUNT, LENGTH];
	private LineRenderer[] flashList = new LineRenderer[RAY_COUNT];
	private Vector3 throwDir;
	private List<int> sizearr = new List<int>();
	private Light flashLight;
	
	void Start() {
		flashLight = GetComponentInChildren<Light>();
	}
	
	void Update () {
		if (Input.GetMouseButtonDown(1)) Destroy(gameObject);
	}
	
	IEnumerator FlashUpdate() {
		for (;;) {
			for (int i = 0; i < sizearr.Count; i++) {
				flashList[i].SetPosition(0, transform.position);
				if (sizearr[i] == 0) Debug.Log("A");
				flashList[i].SetPosition(flashList[i].positionCount-1, list[i, sizearr[i]-1].transform.position + new Vector3(Random.Range(-.1f, .1f), Random.Range(-.1f, .1f), Random.Range(-.1f, .1f)));
				for (int b = 2; b < flashList[i].positionCount; b += 2) {
					Vector3 noise = new Vector3(Random.Range(-.1f, .1f), Random.Range(-.1f, .1f), Random.Range(-.1f, .1f));
					flashList[i].SetPosition(b-1, list[i, b/2].transform.position + noise);
					noise = new Vector3(Random.Range(-.1f, .1f), Random.Range(-.1f, .1f), Random.Range(-.1f, .1f));
					flashList[i].SetPosition(b, list[i, b/2].transform.position + noise);
				}
			}
			flashLight.intensity = Random.Range(.3f, 1f);
			yield return new WaitForSeconds(Random.Range(.05f, .2f));
		}
	}
    void OnDestroy() {
		if (sizearr.Count == 0) return;
		for (int i = 0; i < sizearr.Count; i++) {
			for (int b = 0; b < sizearr[i]; b++) {
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
						if (sizearr.Count-1 < i) sizearr.Add(0);
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
		GameObject origLine = transform.GetChild(0).gameObject;
		if (data.gameObject.name == "PlayerInterface(Clone)" || data.gameObject.name == "shock") PhotonNetwork.Destroy(gameObject);
		else {
			gameObject.GetComponent<Rigidbody>().isKinematic = true;
			throwDir = data.GetContact(0).normal;
			Generate();
			flashLight.transform.position = transform.position + throwDir;
			if (sizearr.Count == 0) return;
			flashList[0] = origLine.GetComponent<LineRenderer>();
			flashList[0].positionCount = sizearr[0]*2;
			for (int i = 1; i < sizearr.Count; i++) {
				flashList[i] = Instantiate(origLine, transform.position, Quaternion.identity).GetComponent<LineRenderer>();;
				flashList[i].positionCount = sizearr[i]*2;
				flashList[i].transform.parent = transform;
			}
			StartCoroutine(FlashUpdate());
		}
	}
}
