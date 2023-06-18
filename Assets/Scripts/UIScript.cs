using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UIScript : MonoBehaviour {
	//owned variables
	public bool lockedCursor;
	private Color color;
	private Color defaultColor;
	private string lastMsgTime = "0";
	
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
	public Text chatText;
	public Text[] chatLines = new Text[3];
	public Image hbar;
	public ChatScript chscr;
	public InputField chatInputField;
	
    void Start() {
		lockedCursor = true;
		playerScript = player.GetComponent<PlayerControls>();
		cam = GameObject.FindWithTag("MainCamera");
		camScript = cam.GetComponent<CameraMoving>();
		StartCoroutine(RareChecks());
		color = chatLines[0].color;
		defaultColor = chatLines[0].color;
		color.a = 0;
    }
    void Update () {
		//cursor lock & in-game menu show
		if ((Input.GetButtonDown("Cancel") || Input.GetButtonDown("Submit")) && Application.isFocused) lockedCursor = !lockedCursor;
		if (lockedCursor){
			if (!Application.isFocused) lockedCursor = false;
			if (Cursor.lockState == CursorLockMode.None) {
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		} else {
			Cursor.lockState = CursorLockMode.None;
			if (!Cursor.visible) Cursor.visible = true;
		}
		if (lockedCursor == panel.activeSelf) {
			panel.SetActive(!lockedCursor);
		}
		if (panel.activeSelf && Input.GetButtonDown("Submit") && !chatInputField.isFocused) {
			chatInputField.Select();
			chatInputField.ActivateInputField();
		}
	}
    IEnumerator RareChecks() {
		bool playerDead = false;
		for (;;) {
			
			//chatMenuUpdate
			chatText.text = "";
		    foreach(List<string> x in chscr.messages) {
     			chatText.text += "\n"+x[1];
			}
			
			//chat preview update & alpha
			if (chscr.messages.Count == 0) {
				for (int i = 0; i < 3; i++) {
					chscr.messages.Add(new List<string>());
					chscr.messages[i].Add(Time.time.ToString());
					chscr.messages[i].Add("");
				}
			}
			if (chscr.messages[chscr.messages.Count-1][0] != lastMsgTime) {
				chatLines[2].color = chatLines[1].color;
				chatLines[1].color = chatLines[0].color;
				chatLines[0].color = defaultColor;
				lastMsgTime = chscr.messages[chscr.messages.Count-1][0];
			}
			for (int i = 0; i < 3; i++) {
				chatLines[i].text = chscr.messages[chscr.messages.Count-i-1][1];
				chatLines[i].color = Color.Lerp(chatLines[i].color, color, (Time.time - float.Parse(chscr.messages[chscr.messages.Count-i-1][0])) * Time.deltaTime);
			}
			
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
	public void ChatInputSend() {
		if (!playerScript.offline) chscr.inputLine = chatInputField.text;
		else {
			chscr.messages.Add(new List<string>());
			chscr.messages[chscr.messages.Count-1].Add(Time.time.ToString());
			chscr.messages[chscr.messages.Count-1].Add("[Me]:"+chatInputField.text);
		}
		chatInputField.text = "";
	}
	public void ChatSystemSend(string msg) {
		if (!playerScript.offline) chscr.inputLine = $"[system]:{msg}";
		else {
			chscr.messages.Add(new List<string>());
			chscr.messages[chscr.messages.Count-1].Add(Time.time.ToString());
			chscr.messages[chscr.messages.Count-1].Add($"[system]:{msg}");
		}
		if (chscr.messages.Count > chscr.messagesLimit) chscr.messages.RemoveAt(0);
	}
}
