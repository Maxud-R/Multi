using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour {
	public Text hptext;
	public GameObject player;
	public PlayerControls script;
    void Update() {
        hptext.text = script.health + "/100";
        if (Input.GetButtonDown("Cancel")) Debug.Log(script.health);
    }
}
