using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class ManagerGame : MonoBehaviourPunCallbacks
{
    public GameObject PlayerPrefab;
    public GameObject uiCanvas;
    void Start()
    {
		Vector3 pos = new Vector3(Random.Range(-5f, 5f), 5f, Random.Range(-5f, 5f));
        GameObject MyPlayer = PhotonNetwork.Instantiate(PlayerPrefab.name, pos, Quaternion.identity);
        GameObject camera = GameObject.FindWithTag("MainCamera");
        CameraMoving followScript = camera.GetComponent("CameraMoving") as CameraMoving;
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
