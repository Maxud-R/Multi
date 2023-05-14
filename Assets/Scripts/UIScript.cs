using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour {
	public Text hptext;
	public GameObject player;
    void Update() {
        hptext.text = player.GetComponent<PlayerControls>().health + "/100";
        //if (Input.GetButtonDown("Cancel")) Debug.Log(.health);
    }
}
