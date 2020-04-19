using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBaseAnimator : MonoBehaviour
{
	Animator nmtr;
    // Start is called before the first frame update
    void Start()
    {
        nmtr = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
		float hAxis = Input.GetAxis("Horizontal");
		float vAxis = Input.GetAxis("Vertical");
        nmtr.SetFloat("yAxis", vAxis);
		nmtr.SetFloat("xAxis", hAxis);
	}
}
