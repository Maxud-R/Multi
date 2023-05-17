using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour {
	public Text hptext;
	public Image hbar;
	public GameObject player;
    void Update() {
		StartCoroutine(HealthUpdate());
    }
    IEnumerator HealthUpdate() {
		for (;;) {
			if (player != null) { //use here C# link variables type
				hptext.text = player.GetComponent<PlayerControls>().health + "/100";
				hbar.fillAmount = player.GetComponent<PlayerControls>().health/100f;
			}
			yield return new WaitForSeconds(.3f);
		}
	}
}
