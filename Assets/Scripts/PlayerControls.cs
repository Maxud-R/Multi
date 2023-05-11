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
	
	//camera variables
	//delete if no err/private bool ActiveSwitch;
		
	//components
	private Transform playerBody;
	private PhotonView photonView;
	private CharacterController controller;
	
	//links
	public GameObject bomb;
	public Rigidbody bombPh;
	public Collider expl;
	
	void Start()
	{
		photonView = GetComponent<PhotonView>();
		controller = GetComponent<CharacterController>();
		playerBody = GetComponent<Transform>();
		if (photonView.IsMine) gameObject.layer = 10;
	}

	void Update()
	{
		if (!photonView.IsMine) {
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
		//fall over from world protection
		if (playerBody.position.y < -20f) {
			Debug.Log("Eto fiasko bratan!");
			playerBody.position = new Vector3(Random.Range(-5f, 5f), 5f, Random.Range(-5f, 5f));
			return;
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
				Vector3 pos = playerBody.TransformPoint(new Vector3(1.4f, 0.5f, 1f));
				bomb = PhotonNetwork.Instantiate("Bomb", pos, playerBody.rotation);
				bomb.GetComponent<Rigidbody>().AddForce(bomb.transform.forward, ForceMode.Impulse);
		}

		//death
		if (health < 1) {
			Debug.Log("You are dead");
			gameObject.SetActive(false);
			health = 100;
		}
		
	}
	void OnTriggerEnter(Collider expl) {
		boom = (playerBody.transform.position - expl.transform.position).normalized * (1/Vector3.Distance(expl.transform.position, groundCheck.position)) * 2;
		Debug.Log(boom.magnitude);
		health -= (int)(boom.magnitude * 10f);
		velocity.y = 0f;
	}
}
