using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
	public float fuse = 3f;
	public float blastRadius = 5f;
	public float blastPressure = 777f;
	[Tooltip("Particle prefab to use during detonation")]
	public GameObject explosionEffect;
	
	float countdown;
	bool hasDetonated = false;
    // Start is called before the first frame update
    void Start()
    {
        countdown = fuse;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
		if(countdown <= 0f && !hasDetonated)
		{
			Explode();
			hasDetonated = true;
		}
    }
	
	void Explode()
	{
		GameObject explosionGO = Instantiate(explosionEffect, transform.position, transform.rotation);
		Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);
		foreach(Collider nearbyObject in colliders)
		{
			Rigidbody rgdb = nearbyObject.GetComponent<Rigidbody>();
			if(rgdb != null)
			{
				rgdb.AddExplosionForce(blastPressure, transform.position, blastRadius);
			}
		}
		Destroy(explosionGO, 2f);
		Destroy(gameObject);
	}
}
