using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
	//gravity variables
	public Transform groundCheck;
	public LayerMask groundMask;
	private bool isGrounded;
	
	//moving variables
	private float xAxis = 0f;
	private float zAxis = 0f;
	public Vector3 move;
	public Vector3 velocity;
	private float speed = 6f;
	private float vSpeed = 0f;
	private float gravity = .25f;
	private Vector3 boom;
	
	//other variables
	public int health = 100;
	public bool offline = false; //==true for starting from the game scene instead of lobby
	//debug var
/*  public bool scrgr;
	public bool contgr;*/
		
	//in-script defined links
	private Transform playerBody;
	private PhotonView photonView;
	private CharacterController controller;
	private GameObject cam;
	private Animator animator;
	
	//in-editor defined links
	public GameObject bombPref;
	public Collider expl;
	public UIScript uiscr;
	
	void Start() {
		photonView = GetComponent<PhotonView>();
		controller = GetComponent<CharacterController>();
		playerBody = GetComponent<Transform>();
		animator = GetComponentInChildren<Animator>();
		cam = GameObject.FindWithTag("MainCamera");
		if (photonView.IsMine || offline) gameObject.layer = 10;
		StartCoroutine(RareChecks());
	}

	void Update() {
		if (!photonView.IsMine && !offline) {
			return;
		}
		//horizontal moving
		xAxis = Input.GetAxis("Horizontal");
		zAxis = Input.GetAxis("Vertical");
		if (uiscr.lockedCursor) move = (playerBody.right * xAxis + playerBody.forward * zAxis) * speed * Time.deltaTime;
		else move = Vector3.zero;
		
		//animation
		if (move != Vector3.zero) animator.SetBool("isWalking", true);
		else animator.SetBool("isWalking", false);
		
		//gravity and ground
		isGrounded = Physics.CheckSphere(groundCheck.position, groundCheck.localScale.x/2, groundMask);
		if (this.isGrounded) {
			vSpeed = -gravity;
		} else {
			if (controller.velocity.y > -30f) vSpeed -= gravity;
		}
		//Jumping
		if (Input.GetButton("Jump") && controller.isGrounded && uiscr.lockedCursor) {
			vSpeed = +7f;
		}
		if ((controller.collisionFlags & CollisionFlags.Above) != 0) vSpeed = -gravity;
		
		//Bomb blast pushing vector
		if (boom.magnitude > .01f) {
			boom = boom / 1.1f;
		} else boom = Vector3.zero;
				
		//Applying move
		move.y = vSpeed * Time.deltaTime;
		controller.Move(move + boom);
		
		//shooting
		if (Input.GetMouseButtonDown(0) && uiscr.lockedCursor) {
				Vector3 pos = playerBody.TransformPoint(new Vector3(1.4f, .5f, 1f));
				GameObject bomb;
				if (!offline) {
					bomb = PhotonNetwork.Instantiate(bombPref.name, pos, playerBody.rotation);
				} else {
					bomb = Instantiate(bombPref, pos, playerBody.rotation);
				}
				bomb.GetComponent<Rigidbody>().AddForce(cam.transform.forward+new Vector3(0f, .5f, 0f), ForceMode.Impulse);
		}
	}
	void OnControllerColliderHit(ControllerColliderHit hit) {
		//preventing sticking to walls
		if (controller.isGrounded && !this.isGrounded) controller.Move((groundCheck.position - hit.point)*Time.deltaTime);
	}
	void OnTriggerEnter(Collider other) {
		//calculating pushing vector and applying health damage
		if (other.name == expl.name+"(Clone)") {
			boom = (playerBody.transform.position - other.transform.position).normalized * (1/Vector3.Distance(other.transform.position, groundCheck.position)) * 2;
			health -= (int)(boom.magnitude * 10f); //health after death may be negative
			velocity.y = 0f;
		}
	}
	IEnumerator RareChecks() {
		for (;;) {
			//fall over from world protection
			if (playerBody.position.y < -20f) {
				Debug.Log("Eto fiasko bratan!");
				uiscr.ChatSystemSend("poznal chto est' fiasko");
				move = Vector3.zero;
				playerBody.position = new Vector3(Random.Range(-5f, 5f), 5f, Random.Range(-5f, 5f));
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
