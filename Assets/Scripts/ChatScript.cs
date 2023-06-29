using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ChatScript : MonoBehaviourPunCallbacks, IPunObservable {
	//owned variables
	public List<List<string>> messages = new List<List<string>>();
	public string inputLine;
	public int messagesLimit = 24; //maximum that can fit in text field, if want to increase - resize text.
	
	//in-editor defined links
	public UIScript uiscr;

    void Update() {
		if (!string.IsNullOrEmpty(inputLine)) {
			photonView.RPC("Chat", RpcTarget.AllViaServer, inputLine);
			inputLine = "";
		}
    }
    
	[PunRPC]
	public void Chat(string newLine, PhotonMessageInfo mi) {
		string senderName = "anonymous";

		if (mi.Sender != null && !string.IsNullOrEmpty(mi.Sender.NickName)) {
			senderName = mi.Sender.NickName;
		}
		AddMessage($"[{senderName}]: {newLine}");
		if (messages.Count > messagesLimit) messages.RemoveAt(0);
	}
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		throw new System.NotImplementedException();
	}
	public void AddMessage(string msg) {
		messages.Add(new List<string>());
		messages[messages.Count-1].Add(Time.time.ToString());
		messages[messages.Count-1].Add(msg);
	}
}
