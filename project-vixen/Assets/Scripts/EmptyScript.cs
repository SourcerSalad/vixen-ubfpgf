using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmptyScript : MonoBehaviour
{
	[Header("Melee")]
	public float meleeDamage = 50f;
	public float meleeRange = 0.5f;
	public float meleeForce = 10f;
	
	Animator nmtr;
	Text cntr;
	
	public Camera playerCamera;
	public GameObject impactGeneric;
	public GameObject impactBlood;
	public GameObject ammoPanel;
	public GameObject text;
	
	void Start()
	{
		nmtr = GetComponent<Animator>();
		cntr = ammoPanel.GetComponent<Text>();
	}
	
	void OnEnable()
	{
		//print(transform);
	}
	// Update is called once per frame
	void Update()
	{
		if(Input.GetButtonDown("Fire1"))
		{
			text.gameObject.SetActive(true);
			Invoke("Empty", 1.0f);
		}
		if(Input.GetButtonDown("Melee"))
		{
			nmtr.SetTrigger("melee");
			Invoke("Melee", 0.2f);
		}
		cntr.text = "0/0	| 000";
	}
	
	void Empty()
	{
		text.gameObject.SetActive(false);
	}
	
	void Melee()
	{
		print("wumpus");
		RaycastHit meleeHit;
			if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out meleeHit, meleeRange))
			{
				EntityScript_NPC nttscp = meleeHit.transform.GetComponent<EntityScript_NPC>();
				if(nttscp != null)
				{
					nttscp.TakeDamage(meleeDamage);
				}
				if(meleeHit.rigidbody != null)
				{
					meleeHit.rigidbody.AddForce(-meleeHit.normal * meleeForce);
				}
				//particle management
				if(meleeHit.transform.tag == "NPC")
				{
					GameObject impactGO = Instantiate(impactBlood, meleeHit.point, Quaternion.LookRotation(meleeHit.normal));
					Destroy(impactGO, 2f);
				}
			}
	}
}
