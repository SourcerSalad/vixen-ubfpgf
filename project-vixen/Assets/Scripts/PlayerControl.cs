using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
	//speed and force vars
	[Header("Forces/Speeds")]
	public float walkSpeed = 5;
	public float jumpForce = 5;
	[Tooltip("Factor by which the player's move speed is multiplied while crouched or walking")]
	public float speedDrop = 0.5f;
	[Tooltip("Factor by which the player's move speed is multiplied while strafing")]
	public float sideDrop = 0.77f;
	[Tooltip("Factor by which the player's move speed is multiplied while airborne")]
	public float flyDrop = 0.5f;
	public float climbSpeed = 5;
	
	[Header("Weapon Mechanics")]
	[Tooltip("How often the player can throw grenades (float, in seconds)")]
	public float grenadeDebounce = 2f;
	[Tooltip("Delay before grenade is spawned (float, in seconds")]
	public float throwDelay = 0.5f;
	public Rigidbody projectile;
	public Transform spawnpoint;
	
	//player dimension vars
	[Header("Player Dimensions")]
	[Tooltip("Height of the player")]
	public float playerHeight = 1.85f;
	[Tooltip("Factor by which the player's height is multiplied while crouched")]
	public float crouchFactor = 0.66f;
	[Tooltip("Position of the player's camera on the Y axis")]
	public float headOffsetY = 0.95f;
	[Tooltip("Position of the player's camera on the X axis")]
	public float headOffsetZ = 0.05f;
	[Tooltip("How far to check for ladders and surfaces (for raycasts)")]
	public float checkRadius = 0.3f;
	
	public GameObject footStep;
	
	//audio vars
	
	//component vars
	Rigidbody rgdb;
	CapsuleCollider cldr;
	
	//private vars
	bool jumpPressed = false;
	bool grenadePressed = false;
	bool isCrouched;
	bool isLadder;
	float crouchHeight;
	float nextTimeToThrow = 0f;
	float nextTimeToStep = 0f;
	bool hitDist;
	bool hasLadder;
	
	Vector3 size;
	float currentY;
	float previousY;
	float difference;
	
	//raycast points
	
	
    void Start()
    {
        rgdb = GetComponent<Rigidbody>();
		cldr = GetComponent<CapsuleCollider>();
		size = cldr.bounds.size;
		Cursor.lockState = CursorLockMode.Locked;
		crouchHeight = playerHeight * crouchFactor;
		float dropDiff = 1 - flyDrop;
    }

    void FixedUpdate()
    {
		isLadder = CheckLadder();
		WalkLogic();
		FootStep();
		JumpLogic();
		CrouchLogic();
		GrenadeLogic();
	}
	
	//movement methods
	void WalkLogic()
	{
		//axis vars
		float hAxis = Input.GetAxis("Horizontal");
		float vAxis = Input.GetAxis("Vertical");
		bool shiftAxis = Input.GetButton("Walk");
		bool cancelAxis = Input.GetButton("Cancel");
		bool jumpButton = Input.GetButton("Jump");
	   
		bool isFlying = CheckFlying();
		bool isSlow = isCrouched || shiftAxis;
		
		//factor ternary operators
		float flyFactor = isFlying ? flyDrop : 1;
		float rearFactor = vAxis < 0 ? speedDrop * 1.5f : 1;
		float slowFactor = isSlow ? speedDrop : 1;
		float climbFactor = isLadder ? climbSpeed : 0;
		
		float movementFactor = walkSpeed * flyFactor * slowFactor;
		
		Vector3 movement = new Vector3(hAxis * movementFactor * sideDrop * Time.deltaTime, vAxis * climbFactor * Time.deltaTime, vAxis * movementFactor * rearFactor * Time.deltaTime);
		Vector3 newPos = transform.position + transform.TransformDirection(movement);
	   
		rgdb.AddRelativeForce(movement, ForceMode.VelocityChange);
		rgdb.MovePosition(newPos);
		
		float stepSpeed = isSlow ? 2.5f : 0f;
		print(stepSpeed);
		if(Time.time >= nextTimeToStep && (Mathf.Abs(vAxis) > 0 || Mathf.Abs(hAxis) > 0))
		{	
			GameObject stepGO = Instantiate(footStep, transform.position, transform.rotation);
			nextTimeToStep = Time.time + (walkSpeed + stepSpeed) * 0.05f;
			Destroy(stepGO, 1f);
		}
		
	   if(cancelAxis)
		{
			Cursor.lockState = CursorLockMode.None;
		}
	}
	
	void JumpLogic()
	{
		//axis vars
		float jAxis = Input.GetAxis("Jump");
		float hAxis = Input.GetAxis("Horizontal");
		float vAxis = Input.GetAxis("Vertical");

		if(jAxis > 0)
		{
			bool isGrounded = CheckGrounded();
			if(!jumpPressed && isGrounded && !isLadder)
			{
				jumpPressed = true;
				Vector3 jumpVector = new Vector3(hAxis * jumpForce, jAxis * jumpForce, vAxis * jumpForce);
				rgdb.AddRelativeForce(jumpVector, ForceMode.VelocityChange);
			}
		}
		else
		{
			jumpPressed = false;
		}
	}
	
	void CrouchLogic()
	{
		//axis vars
		bool crouchButton = Input.GetButton("Crouch");
		//conditional action
		if(crouchButton)
		{
			cldr.height = crouchHeight;
			isCrouched = true;
		}
		else
		{
			cldr.height = playerHeight;
			isCrouched = false;
		}
	}
	
	
	void FootStep()
	{
		
	}
	
	void GrenadeLogic()
	{
		bool grenadeButton = Input.GetButtonDown("Fire2");
		if(grenadeButton && Time.time >= nextTimeToThrow)
		{
			if(!grenadePressed)
			nextTimeToThrow = Time.time + 1f * grenadeDebounce;
			grenadePressed = true;
			Invoke("ThrowGrenade", throwDelay);
		}
		else
		{
			grenadePressed = false;
		}
	}
	
	void ThrowGrenade()
	{
		Rigidbody clone;
		clone = (Rigidbody)Instantiate(projectile, spawnpoint.position, projectile.rotation);
		clone.velocity = spawnpoint.TransformDirection(Vector3.forward*20);
	}
	
	//checking functions
	public bool CheckGrounded()
	{
		Vector3 cAlpha1 = transform.position + new Vector3(0, -size.y / 2 + 0.01f, 0);
		Vector3 cBeta1 = transform.position + new Vector3(size.x / 2, -size.y / 4 + 0.01f, 0);
		Vector3 cGamma1 = transform.position + new Vector3(-size.x / 2, -size.y / 4 + 0.01f, 0);
		
		bool grndAlpha = Physics.Raycast(cAlpha1, -Vector3.up, 0.02f);
		bool grndBeta = Physics.Raycast(cBeta1, -Vector3.up, 0.52f);
		bool grndGamma = Physics.Raycast(cGamma1, -Vector3.up, 0.52f);
		
		return (grndAlpha || grndBeta || grndGamma);
	}
	
	bool CheckLadder()
	{
		RaycastHit hitLadderRight;
		RaycastHit hitLadderLeft;
		
		Ray ldrRayRight = new Ray(transform.position + transform.TransformDirection(0.25f, (-size.y / 2 ) + 0.1f, 0.25f), transform.forward);
		Ray ldrRayLeft = new Ray(transform.position + transform.TransformDirection(-0.25f, (-size.y / 2 ) + 0.1f,0.25f), transform.forward);
		
		if(Physics.Raycast(ldrRayRight, out hitLadderRight))
		{
			hitDist = hitLadderRight.distance < checkRadius;
			hasLadder = hitLadderRight.collider.tag == "Ladder";
		}
		else if(Physics.Raycast(ldrRayLeft, out hitLadderLeft))
		{
			hitDist = hitLadderLeft.distance < checkRadius;
			hasLadder = hitLadderLeft.collider.tag == "Ladder";
		}
		return (hitDist && hasLadder);
	}
	
	bool CheckFlying()
	{
		//checkfall raycast
		float maxDisp = (playerHeight / 2) + 0.1f;
		//ray declaration
		RaycastHit hit1;
		RaycastHit hit2;
		RaycastHit hit3;
		RaycastHit hit4;
		//defining ray LocRot
		Ray fwdRay = new Ray(transform.position + transform.TransformDirection(0, 0, checkRadius), -Vector3.up);
		Ray bckRay = new Ray(transform.position + transform.TransformDirection(0, 0, -checkRadius), -Vector3.up);
		Ray lftRay = new Ray(transform.position + transform.TransformDirection(-checkRadius, 0, 0), -Vector3.up);
		Ray ritRay = new Ray(transform.position + transform.TransformDirection(checkRadius, 0, 0), -Vector3.up);
		//executing raycasts, outputing hit data
		Physics.Raycast(fwdRay, out hit1);
		Physics.Raycast(bckRay, out hit2);
		Physics.Raycast(lftRay, out hit3);
		Physics.Raycast(ritRay, out hit4);
		//declaring booleans for easy return
		bool dispForward = hit1.distance > maxDisp;
		bool dispBack = hit2.distance > maxDisp;
		bool dispLeft = hit3.distance > maxDisp;
		bool dispRight = hit4.distance > maxDisp;
		return (dispForward && dispBack && dispLeft && dispLeft);
	}
}
