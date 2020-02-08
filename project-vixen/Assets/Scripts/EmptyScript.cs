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
	Text noWepTx;
	
	public Camera playerCamera;
	public GameObject impactGeneric;
	public GameObject impactBlood;
	
	GameObject ammoPanel;
	GameObject noWep;
	
	void Start()
	{
		nmtr = GetComponent<Animator>();
		ammoPanel = GameObject.Find("AmmoCount");
		cntr = ammoPanel.GetComponent<Text>();
		noWep = GameObject.Find("NoWeapon");
	}
	
	void OnEnable()
	{
		ammoPanel = GameObject.Find("AmmoCount");
		cntr = ammoPanel.GetComponent<Text>();
		cntr.text = "No Weapon Selected";
		noWep = GameObject.Find("NoWeapon");
		noWepTx = noWep.GetComponent<Text>();
	}
	// Update is called once per frame
	void Update()
	{
		if(Input.GetButtonDown("Fire1"))
		{
			noWepTx.text = "No Weapon";
			Invoke("Empty", 1.0f);
		}
		if(Input.GetButtonDown("Melee"))
		{
			nmtr.SetTrigger("melee");
			Invoke("Melee", 0.2f);
		}
	}
	
	void Empty()
	{
		noWepTx.text = "";
	}
	
	void Melee()
	{
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
