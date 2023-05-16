using System;
using System.Collections;
using System.Collections.Generic;
using Random=UnityEngine.Random;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class ManagerGame : MonoBehaviourPunCallbacks
{
	//links
    public GameObject PlayerPrefab;
    public GameObject uiCanvas;
    
    private GameObject MyPlayer;
    void Start() {
		SpawnMe();
    }
    void Update() {
		if (Input.GetButton("Respawn") && MyPlayer == null) {
			SpawnMe();
		}
	}
	public void SpawnMe() {
		Vector3 pos = new Vector3(Random.Range(-5f, 5f), 5f, Random.Range(-5f, 5f));
		MyPlayer = PhotonNetwork.Instantiate(PlayerPrefab.name, pos, Quaternion.identity);
		if (MyPlayer == null) {
			MyPlayer = Instantiate(PlayerPrefab, pos, Quaternion.identity);
			MyPlayer.GetComponent<PlayerControls>().offline = true;
		}
		GameObject camera = GameObject.FindWithTag("MainCamera");
		CameraMoving followScript = camera.GetComponent<CameraMoving>();
		followScript.CamRotationX = 0f;
		followScript.CamRotationY = 0f;
		followScript.target = MyPlayer;
		uiCanvas.GetComponent<UIScript>().player = MyPlayer;
	}
	public void Leave() {
		PhotonNetwork.LeaveRoom();
	}
    public override void OnLeftRoom() {
		Cursor.lockState = CursorLockMode.None;
		SceneManager.LoadScene(0);
	}
	public override void OnPlayerEnteredRoom(Player newPlayer) {
		Debug.LogFormat("Player {0} entered room", newPlayer.NickName);
	}
	public override void OnPlayerLeftRoom(Player otherPlayer) {
		Debug.LogFormat("Player {0} left room", otherPlayer.NickName);
	}
}
