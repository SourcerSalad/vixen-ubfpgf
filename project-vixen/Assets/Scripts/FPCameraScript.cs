using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPCameraScript : MonoBehaviour
{
	public float viewRange = 80.0f;
	public float pickUpRange = 1.0f;
	
	public GameObject player;
	public GameObject target;
	PlayerControl plrctl;
	Rigidbody hprgdb;
	Light lght;
	
	float mDelta;
	float mHorizontal;
	float mVertical = 0.0f;
	float mVerticalClamp;
	float cameraPosY;
	
	bool holdingObject = false;
	

    // Start is called before the first frame update
    void Start()
    {
       player = this.transform.parent.gameObject;
	   plrctl = player.GetComponent<PlayerControl>();
	   hprgdb = target.GetComponent<Rigidbody>();
	   lght = GetComponent<Light>();
	   cameraPosY = (plrctl.playerHeight / 2) * plrctl.headOffsetY;
	   transform.localPosition = new Vector3 (0, cameraPosY, plrctl.headOffsetZ);
    }

    // Update is called once per frame
    void Update()
    {
		bool flashKey = Input.GetButtonUp("Light");
		if(flashKey)
		{
			if(lght.enabled)
			{
				lght.enabled = false;
			}
			else
			{
				lght.enabled = true;
			}
		}
		MouseToRotate();
		
		float camCrouchHeight = (plrctl.playerHeight * plrctl.crouchFactor) / 2;
		bool crouchButton = Input.GetButton("Crouch");
		if(crouchButton)
		{
			//transform.Translate(0.0f, -0.1f, 0.0f * Time.deltaTime);
			transform.localPosition = new Vector3 (0, camCrouchHeight, plrctl.headOffsetZ);
		}
		else
		{
			transform.localPosition = new Vector3 (0, cameraPosY, plrctl.headOffsetZ);
		}
		
		
    }
	
	void FixedUpdate()
	{
		bool useKey = Input.GetButtonDown("Use");
		if(useKey)
		{
			Interact();
		}
	}
	
	void MouseToRotate()
	{
		mVertical += Input.GetAxis("Mouse Y");
		transform.localEulerAngles = new Vector3 (Mathf.Clamp(-mVertical, -viewRange, viewRange), 0.0f, 0.0f);
		mHorizontal += Input.GetAxis("Mouse X");
		player.transform.localEulerAngles = new Vector3 (0.0f, mHorizontal, 0.0f);
	}
	
	void Interact()
	{
		RaycastHit interactHit;
		if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out interactHit, pickUpRange))
		{
			if(interactHit.transform.tag == "Prop_dynamic" && interactHit.rigidbody.mass <= 2.0f && !holdingObject)
			{
				EntityScript_Prop_dynamic nttscp = interactHit.transform.GetComponent<EntityScript_Prop_dynamic>();
				nttscp.PickedUp(hprgdb);
				holdingObject = true;
			}
			else
			{
				EntityScript_Prop_dynamic nttscp = interactHit.transform.GetComponent<EntityScript_Prop_dynamic>();
				if(nttscp)
				{
					nttscp.Dropped();
					holdingObject = false;
				}
			}
		}
	}
}
