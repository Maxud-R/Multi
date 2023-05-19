using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
//using Photon.Realtime;

public class UIScript : MonoBehaviour {
	public Text hptext;
	public Text desc;
	public Image hbar;
	public GameObject player;
	private GameObject cam;
	private PlayerControls script;
	public RaycastHit look;
    void Start() {
		StartCoroutine(RareChecks());
    }
    IEnumerator RareChecks() {
		bool playerDead = false;
		script = player.GetComponent<PlayerControls>();
		cam = GameObject.FindWithTag("MainCamera");
		for (;;) {
			//HealthUpdate
			if (player != null) {
				if (playerDead) {
					script = player.GetComponent<PlayerControls>();
					playerDead = false;
				}
				hptext.text = script.health + "/100";
				hbar.fillAmount = script.health/100f;
			} else {
				playerDead = true;
			}
			//Description text
			if (Physics.Raycast(cam.transform.position, cam.transform.forward, out look, 10f)) {
				if (look.collider.gameObject.CompareTag("Player")) {
					desc.text = look.collider.gameObject.GetComponent<PhotonView>().Controller.NickName;
				} else {
					desc.text = look.collider.gameObject.name;
				}
			} else {
				desc.text = "";
			}
			yield return new WaitForSeconds(.3f);
		}
	}
}
