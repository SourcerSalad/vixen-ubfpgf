using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunScript : MonoBehaviour
{
	[Header("Base Values")]
	public float damage = 10f;
	public float range = 100f;
	[Tooltip("Lower values = tighter spread")]
	public float accuracyFactor = 0.1f;
	public bool automatic = false;
	[Tooltip("Rounds per second")]
	public float fireRate = 15f;
	public float reloadTime = 3f;
	public float impactForce = 10f;
	[Header("Melee")]
	public float meleeDamage = 50f;
	public float meleeRange = 0.5f;
	public float meleeForce = 10f;
	
	//ammo vars
	[Header("Ammunition")]
	[Tooltip("Maximum ammo player can carry")]
	public int maxAmmoReserve = 32;
	[Tooltip("Ammo carried in the weapon per reload")]
	public int clipSize = 8;
	int currentAmmo;
	int ammoReserve;
	//public bool auto = false;
	
	float nextTimeToFire = 0f;
	Animator nmtr;
	AudioSource fireSound;
	Text cntr;
	
	[Header("GameObjects")]
	[Tooltip("Camera that represents player's perspective")]
	public Camera playerCamera;
	[Tooltip("Particle system to spawn on firing")]
	public ParticleSystem muzzleFlash;
	public GameObject impactGeneric;
	public GameObject impactBlood;
	[Tooltip("Text panel in HUD that displays ammunition count")]
	public GameObject text;
	
	void Start()
	{
		nmtr = GetComponent<Animator>();
		fireSound = GetComponent<AudioSource>();
		currentAmmo = clipSize;
		ammoReserve = maxAmmoReserve;
		cntr = text.GetComponent<Text>();
	}
	
	void OnEnable()
	{
		//print(transform);
		cntr.text = currentAmmo + "/" + clipSize + " | " + ammoReserve;
	}
	// Update is called once per frame
	void Update()
	{
		if(automatic)
		{
			if(Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
			{
				nextTimeToFire = Time.time + 1f / fireRate;
				Shoot();
			}
		}
		else if(!automatic)
		{
			if(Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
			{
				nextTimeToFire = Time.time + 1f / fireRate;
				Shoot();
			}
		}
		if(Input.GetButtonDown("Reload"))
		{
			Reload();
		}
		if(Input.GetButtonDown("Melee"))
		{
			nmtr.SetTrigger("melee");
			Invoke("Melee", 0.2f);
		}
		cntr.text = currentAmmo + "/" + clipSize + " | " + ammoReserve;
	}
	
	void Shoot()
	{
		if(currentAmmo > 0)
		{
			muzzleFlash.Play();
			nmtr.SetTrigger("shoot");
			fireSound.Play();
			currentAmmo -= 1;
			Vector3 inaccuracy = new Vector3((Random.value - 0.5f) * accuracyFactor, (Random.value - 0.5f) * accuracyFactor, (Random.value - 0.5f) * accuracyFactor);
			
			RaycastHit hit;
			if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward + inaccuracy, out hit, range))
			{
				EntityScript_NPC nttscp = hit.transform.GetComponent<EntityScript_NPC>();
				if(nttscp != null)
				{
					nttscp.TakeDamage(damage);
				}
				if(hit.rigidbody != null)
				{
					hit.rigidbody.AddForce(-hit.normal * impactForce);
				}
				//particle management
				if(hit.transform.tag == "NPC")
				{
					GameObject impactGO = Instantiate(impactBlood, hit.point, Quaternion.LookRotation(hit.normal));
					Destroy(impactGO, 2f);
				}
				else
				{
					GameObject impactGO = Instantiate(impactGeneric, hit.point, Quaternion.LookRotation(hit.normal));
					Destroy(impactGO, 2f);
				}
			}
		}
		else
		{
			Reload();
		}
	}
	
	void Reload()
	{
		if(currentAmmo < clipSize && ammoReserve > 0)
			{
				nextTimeToFire = Time.time + reloadTime;
				nmtr.SetTrigger("reload");
				if(currentAmmo == 0 && ammoReserve >= clipSize)
				{
					ammoReserve -= clipSize;
					currentAmmo = clipSize;
				}
				else
				{
					int ammoDiff = clipSize - currentAmmo;
					if(ammoDiff < ammoReserve)
					{
						ammoReserve -= ammoDiff;
						currentAmmo = clipSize;
					}
					else
					{
						currentAmmo += ammoReserve;
						ammoReserve = 0;
					}
				}
			}
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
					meleeHit.rigidbody.AddForce(-meleeHit.normal * impactForce);
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
