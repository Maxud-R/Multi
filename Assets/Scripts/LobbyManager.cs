using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LobbyManager : MonoBehaviourPunCallbacks {
	//in-editor defined links
	public Text LogText;
	public InputField nickField;
	private static string nickname;
	
    void Start() {
		PhotonNetwork.AutomaticallySyncScene = true;
		if (string.IsNullOrEmpty(nickname)) nickname = "Player " + Random.Range(1000, 10000);
		PhotonNetwork.NickName = nickname;
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
		if (nickField.text != "") {
			PhotonNetwork.NickName = nickField.text;
			nickname = nickField.text;
			Log("Your name is set to: " + PhotonNetwork.NickName);
		}
	}
}
