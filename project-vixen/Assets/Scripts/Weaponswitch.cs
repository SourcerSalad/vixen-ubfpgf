using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weaponswitch : MonoBehaviour
{
    public int selectedWeapon = 0;
	
    void Start()
    {
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {
		int previousWeapon = selectedWeapon;
		
        if(Input.GetButtonDown("Switch"))
		{
			if(selectedWeapon >= transform.childCount - 1)
			{
				selectedWeapon = 0;
			}
			else
			{
				selectedWeapon++;
			}
		}
		
		if(previousWeapon != selectedWeapon)
		{
			SelectWeapon();
		}
    }
	
	void SelectWeapon()
	{
		int i = 0;
		foreach (Transform weapon in transform)
		{
			if(i == selectedWeapon)
			{
				weapon.gameObject.SetActive(true);
			}
			else
			{
				weapon.gameObject.SetActive(false);
			}
			i++;
			//print(foreach (Transform weapon in transform);
		}
	}
}
