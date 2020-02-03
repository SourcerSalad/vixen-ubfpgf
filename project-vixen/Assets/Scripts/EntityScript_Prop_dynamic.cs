using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityScript_Prop_dynamic : MonoBehaviour
{
    public float health = 100f;
	
	[Header("Joint Properties")]
	public float jointSpring = 100f;
	public float jointDamper = 0.2f;
	public float jointBreakForce = 100f;
	[Header("Anchor")]
	public float anchorX = 0f;
	public float anchorY = 0.5f;
	public float anchorZ = 0f;
	[Header("Connected Anchor")]
	public float connectedX = 0f;
	public float connectedY = 0.5f;
	public float connectedZ = 0f;
	
	Vector3 Holder;
	
	Rigidbody rgdb;
	
	void Start()
	{
		rgdb = transform.GetComponent<Rigidbody>();
	}
	
	public void PickedUp(Rigidbody hprgdb)
	{
		SpringJoint spjn = gameObject.AddComponent<SpringJoint>() as SpringJoint;
		spjn.autoConfigureConnectedAnchor = false;
		spjn.spring = jointSpring;
		spjn.damper = jointDamper;
		spjn.breakForce = jointBreakForce;
		spjn.connectedBody = hprgdb;
		spjn.anchor = new Vector3(anchorX, anchorY, anchorZ);
		spjn.connectedAnchor = new Vector3(connectedX, connectedY, connectedZ);
		rgdb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
	}
	
	public void Dropped()
	{
		print("dropping");
		rgdb.constraints = RigidbodyConstraints.None;
		Destroy(transform.GetComponent<SpringJoint>());
	}
	
	public void TakeDamage(float amount)
	{
		health -= amount;
		if(health <= 0)
		{
			Die();
		}
	}
	
	void Die()
	{
		Destroy(gameObject);
	}
}
