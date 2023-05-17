using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
	//gravity variables
	public Transform groundCheck;
	private float groundDistance = 0.2f;
	public LayerMask groundMask;
	private bool isGrounded;
	
	//moving variables
	private float xAxis = 0f;
	private float zAxis = 0f;
	private Vector3 move;
	private Vector3 velocity;
	private float speed = 6f;
	private float gravity = 15f;
	private Vector3 boom;
	
	//other variables
	public int health = 100;
	public bool offline = false; //==true for starting from the game scene instead of lobby
		
	//components
	private Transform playerBody;
	private PhotonView photonView;
	private CharacterController controller;
	
	//links
	public GameObject bombPref;
	public Collider expl;
	
	void Start()
	{
		photonView = GetComponent<PhotonView>();
		controller = GetComponent<CharacterController>();
		playerBody = GetComponent<Transform>();
		if (photonView.IsMine || offline) gameObject.layer = 10;
		StartCoroutine(RareChecks());
	}

	void Update()
	{
		if (!photonView.IsMine && !offline) {
			return;
		}
				
		//Moving calculation
		isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
				
		xAxis = Input.GetAxis("Horizontal");
		zAxis = Input.GetAxis("Vertical");
		move = playerBody.right * xAxis + playerBody.forward * zAxis;
				
		//gravity and vertical speed of the player
		if (isGrounded && velocity.y < 0f) {
			velocity.y = -2f;
				
		} else {
			if (velocity.y > -15f) velocity.y -= gravity * Time.deltaTime;
		}
		
		//Jumping
		if (Input.GetButton("Jump") && isGrounded) {
			velocity.y = +7f;
		}
		
		//Bomb blast pushing
		if (boom.magnitude > 0.01f) {
			boom = boom / 1.1f;
		} else boom = Vector3.zero;
				
		//Applying move
		controller.Move(move * speed * Time.deltaTime + velocity * Time.deltaTime + boom);
		
		//shooting
		if (Input.GetMouseButtonDown(0)) {
				Vector3 pos = playerBody.TransformPoint(new Vector3(1.4f, .5f, 1f));
				GameObject bomb;
				if (!offline) {
					bomb = PhotonNetwork.Instantiate(bombPref.name, pos, playerBody.rotation);
				} else {
					bomb = Instantiate(bombPref, pos, playerBody.rotation);
				}
				bomb.GetComponent<Rigidbody>().AddForce(bomb.transform.forward, ForceMode.Impulse);
		}
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
				playerBody.position = new Vector3(Random.Range(-5f, 5f), 5f, Random.Range(-5f, 5f));
			}
			//death
			if (health < 1) {
				Debug.Log("You are dead");
				PhotonNetwork.Destroy(gameObject);
			}
			yield return new WaitForSeconds(.3f);
		}
	}
}
