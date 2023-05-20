using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LobbyManager : MonoBehaviourPunCallbacks {
	public Text LogText;
	public InputField nickField;
    // Start is called before the first frame update
    void Start() {
		PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = "Player " + Random.Range(1000, 10000);
        Log("Your name is set to: " + PhotonNetwork.NickName + "\nWait until connecting to main matchmaking server...");
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
		Log("Connected main server. You can create or join room");
	}
	public void CreateRoom() {
		PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 2 });
	}
	public void JoinRoom() {
		PhotonNetwork.JoinRandomRoom();
	}
	public override void OnJoinedRoom() {
		Log("Joined the room");
		PhotonNetwork.LoadLevel("Game");
	}
    private void Log(string message) {
		Debug.Log(message);
		LogText.text += "\n" + message;
	}
	public void ApplyNickname() {
		PhotonNetwork.NickName = nickField.text;
		Log("Your name is set to: " + PhotonNetwork.NickName);
	}
}
