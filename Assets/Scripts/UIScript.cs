﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
//using Photon.Realtime;

public class UIScript : MonoBehaviour {
	//owned variables
	public bool lockedCursor;
	
	//in-script defined links
	private PlayerControls playerScript;
	private CameraMoving camScript;
	private GameObject cam;
	public RaycastHit look;
	public GameObject lookObject;
	public GameObject player;
	
	//in-editor defined links
	public GameObject panel;
	public Text hptext;
	public Text desc;
	public Image hbar;
	
    void Start() {
		lockedCursor = true;
		playerScript = player.GetComponent<PlayerControls>();
		cam = GameObject.FindWithTag("MainCamera");
		camScript = cam.GetComponent<CameraMoving>();
		StartCoroutine(RareChecks());
    }
    void Update () {
		//cursor lock & in-game menu show
		if (Input.GetButtonDown("Cancel") && Application.isFocused) lockedCursor = !lockedCursor;
		if (lockedCursor && !Application.isFocused) lockedCursor = false;
		if (lockedCursor == panel.activeSelf) {
			panel.SetActive(!lockedCursor);
		}
	}
    IEnumerator RareChecks() {
		bool playerDead = false;
		for (;;) {
			//HealthUpdate
			if (player != null) {
				if (playerDead) {
					playerScript = player.GetComponent<PlayerControls>();
					playerDead = false;
				}
				hptext.text = playerScript.health + "/100";
				hbar.fillAmount = playerScript.health/100f;
			} else {
				playerDead = true;
			}
			//Description text
			if (Physics.Raycast(cam.transform.position, cam.transform.forward, out look, 10f)) {
				lookObject = look.collider.gameObject;
				if (lookObject != null && lookObject != player) {
					if (lookObject.CompareTag("Player")) {
						desc.text = lookObject.GetComponent<PhotonView>().Controller.NickName;
					} else {
						desc.text = lookObject.gameObject.name;
					}
				}
			} else {
				desc.text = "";
			}
			yield return new WaitForSeconds(.3f);
		}
	}
}
