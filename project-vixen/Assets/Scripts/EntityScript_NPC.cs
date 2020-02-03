using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityScript_NPC : MonoBehaviour
{
	public float health = 100f;
	
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
