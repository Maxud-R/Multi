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
	private float gravity = .005f;
	private Vector3 boom;
	
	//other variables
	public int health = 100;
	public bool offline = false;
	
	//debug var
	public bool scrgr;
	public bool contgr;
	private float jumpTime;
	private float jumpHeight;
	private bool jumpStart;
		
	//in-script defined links
	private PhotonView photonView;
	private CharacterController controller;
	private GameObject cam;
	private Animator animator;
	private string explName;
	
	//in-editor defined links
	public GameObject bombPref;
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
		//horizontal moving
		xAxis = Input.GetAxis("Horizontal");
		zAxis = Input.GetAxis("Vertical");
		if (uiscr.lockedCursor) move = (transform.right * xAxis + transform.forward * zAxis) * speed * Time.deltaTime;
		else move = Vector3.zero;
		
		//animation
		if (move != Vector3.zero) animator.SetBool("isWalking", true);
		else animator.SetBool("isWalking", false);
		
		//gravity and ground
		isGrounded = Physics.CheckSphere(groundCheck.position, groundCheck.localScale.x/2, groundMask);
		//*****gravity debug section*****
		scrgr = isGrounded;
		contgr = controller.isGrounded;
		
		if (jumpStart) {
			jumpTime += Time.deltaTime;
			if (jumpHeight < groundCheck.transform.position.y) jumpHeight = groundCheck.transform.position.y;
			if (this.isGrounded) {
				jumpStart = false;
				uiscr.ChatSystemSend($"jumpTime:{jumpTime}, Height: {jumpHeight}, {1/Time.deltaTime} FPS");
				jumpTime = 0;
				jumpHeight = 0;
			}
		}
		//*****gravity debug section ENDS*****
		if (this.isGrounded) {
			vSpeed = -0.005f;
		} else {
			if (controller.velocity.y > -30f) vSpeed -= gravity;
		}
		//Jumping
		if (vSpeed < 0 && Input.GetButton("Jump") && controller.isGrounded && uiscr.lockedCursor) {
			vSpeed = gravity*30f;
			jumpStart = true; //delete
		}
		if ((controller.collisionFlags & CollisionFlags.Above) != 0) vSpeed = -0.005f;
		
		//Bomb blast pushing vector
		if (boom.magnitude > .007f) {
			boom = boom / 1.05f;
		} else boom = Vector3.zero;
				
		//Applying move
		move.y = vSpeed;
		controller.Move(move + boom);
		
		//shooting
		if (Input.GetMouseButtonDown(0) && uiscr.lockedCursor) {
				Vector3 pos = transform.TransformPoint(new Vector3(.85f, .2f, 1f));
				GameObject bomb;
				if (!offline) {
					bomb = PhotonNetwork.Instantiate(bombPref.name, pos, transform.rotation);
				} else {
					bomb = Instantiate(bombPref, pos, transform.rotation);
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
		if (other.name == explName) {
			boom = (transform.position - other.transform.position).normalized * (1/Vector3.Distance(other.transform.position, groundCheck.position)) * 2;
			health -= (int)(boom.magnitude * 10f); //health after death may be negative
			velocity.y = 0f;
		}
	}
	IEnumerator RareChecks() {
		for (;;) {
			//fall over from world protection
			if (transform.position.y < -20f) {
				Debug.Log("Eto fiasko bratan!");
				uiscr.ChatSystemSend("poznal chto est' fiasko");
				move = Vector3.zero;
				transform.position = new Vector3(Random.Range(-5f, 5f), 5f, Random.Range(-5f, 5f));
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
