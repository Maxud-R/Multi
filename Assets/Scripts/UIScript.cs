using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour {
	public Text hptext;
	public Image hbar;
	public GameObject player;
    void Start() {
		StartCoroutine(HealthUpdate());
    }
    IEnumerator HealthUpdate() {
		bool playerDead = false;
		PlayerControls script = player.GetComponent<PlayerControls>();
		for (;;) {
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
			yield return new WaitForSeconds(.3f);
		}
	}
}
