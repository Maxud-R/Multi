using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {
	//gravity variables
	public Transform groundCheck;
	public LayerMask groundMask;
	private bool isGrounded;
	
	//moving variables
	private float xAxis = 0f;
	private float zAxis = 0f;
	public Vector3 move;
	private float speed = 6f;
	private float vSpeed = 0f;
	private float gravity = .01f;
	private float jumpHeight = .17f;
	private Vector3 boom;
	
	//other variables
	public int health = 100;
	public bool offline = false;
	public const int BOMB_COUNT = 3;
	
	//debug var
		
	//in-script defined links
	private PhotonView photonView;
	private CharacterController controller;
	private GameObject cam;
	private Animator animator;
	private string explName;
	
	//in-editor defined links
	public GameObject[] bombPref = new GameObject[BOMB_COUNT];
	public GameObject expl;
	public UIScript uiscr;
	
	void Start() {
		photonView = GetComponent<PhotonView>();
		controller = GetComponent<CharacterController>();
		animator = GetComponentInChildren<Animator>();
		cam = GameObject.FindWithTag("MainCamera");
		explName = expl.transform.GetChild(0).name;
		if (photonView.IsMine || offline) transform.GetChild(1).GetChild(1).gameObject.layer = 10;
		StartCoroutine(RareChecks());
	}

	void Update() {
		if (!photonView.IsMine && !offline) {
			return;
		}
		
		//preventing stick to flooring
		if ((controller.collisionFlags & CollisionFlags.Above) != 0) vSpeed = -gravity;
		
		//shooting
		if (Input.GetMouseButtonDown(0) && uiscr.lockedCursor) {
				Vector3 pos = transform.TransformPoint(new Vector3(.85f, .2f, 1f));
				GameObject bomb;
				if (!offline) {
					bomb = PhotonNetwork.Instantiate(bombPref[uiscr.chosenBomb].name, pos, transform.rotation);
				} else {
					bomb = Instantiate(bombPref[uiscr.chosenBomb], pos, transform.rotation);
				}
				bomb.GetComponent<Rigidbody>().AddForce(cam.transform.forward+new Vector3(0f, .5f, 0f), ForceMode.Impulse);
		}
	}
	void FixedUpdate() {
		//horizontal moving
		xAxis = Input.GetAxis("Horizontal");
		zAxis = Input.GetAxis("Vertical");
		if (uiscr.lockedCursor) move = (transform.right * xAxis + transform.forward * zAxis) * speed * Time.deltaTime;
		else move = Vector3.zero;

		//animation
		if (move.x != 0 || move.y != 0) {
			animator.SetBool("isWalking", true);
		} else {
			animator.SetBool("isWalking", false);
		}
		
		//gravity and ground
		isGrounded = Physics.CheckSphere(groundCheck.position, groundCheck.localScale.x/2, groundMask);
		if (this.isGrounded) {
			vSpeed = -gravity;
		} else {
			if (vSpeed > -1f) vSpeed -= gravity;
		}
		
		//Jumping
		if (vSpeed < 0 && Input.GetButton("Jump") && controller.isGrounded && uiscr.lockedCursor) {
			vSpeed = jumpHeight;
		}
		move.y = vSpeed;
		
		//Bomb blast pushing vector
		if (boom.magnitude > .05f) {
			boom = boom / 1.05f;
		} else boom = Vector3.zero;
		
		//Applying move
		controller.Move(move + boom);
	}
	void OnControllerColliderHit(ControllerColliderHit hit) {
		//preventing sticking to walls
		if (controller.isGrounded && !this.isGrounded && (controller.collisionFlags & CollisionFlags.Sides) != 0) {
			controller.Move((-hit.moveDirection) * Time.deltaTime);
		}

	}
	void OnTriggerEnter(Collider other) {
		//calculating pushing vector and applying health damage
		if (other.name == explName || other.name == "Web") {
			boom = (transform.position - other.transform.position).normalized * (1/Vector3.Distance(other.transform.position, groundCheck.position)) * 2;
			health -= (int)(boom.magnitude * 10f); //health after death may be negative
		}
		if (other.name == "Web") {
			boom = (transform.position - other.transform.position).normalized * (1/Vector3.Distance(other.transform.position, groundCheck.position));
			health -= (int)(boom.magnitude * 20f);
		}
	}
	IEnumerator RareChecks() {
		for (;;) {
			//fall over from world protection
			if (transform.position.y < -20f) {
				Debug.Log("Eto fiasko bratan!");
				uiscr.ChatSystemSend("poznal chto est' fiasko");
				PhotonNetwork.Destroy(gameObject);
			}
			//death
			if (health < 1) {
				Debug.Log("You are dead");
				uiscr.ChatSystemSend("dead.");
				PhotonNetwork.Destroy(gameObject);
			}
			yield return new WaitForSeconds(.3f);
		}
	}
}
