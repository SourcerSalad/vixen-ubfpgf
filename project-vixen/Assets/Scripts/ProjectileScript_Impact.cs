using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript_Impact : MonoBehaviour
{
	[Header("GameObjects")]
	public GameObject impactGeneric;
	public GameObject impactBlood;
    // Start is called before the first frame update
    void OnAwake()
    {
        Destroy(gameObject, 3.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	void OnCollisionEnter(Collision collision)
	{
		ContactPoint contact = collision.contacts[0];
		Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
		Vector3 pos = contact.point;
		if(contact.otherCollider.tag != "Player")
		{
			GameObject impactGO = Instantiate(impactGeneric, pos, rot);
			Destroy(impactGO, 0.5f);
			Destroy(gameObject);
		}
	}
}
