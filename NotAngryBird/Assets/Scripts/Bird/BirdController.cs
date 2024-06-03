using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    public bool canUseAbility;
	private void Update()
	{
		if (canUseAbility)
		{
			if (Input.GetMouseButtonUp(0))
			{
				OnClick();
				canUseAbility = false;
			}
		}
	}

	public virtual void OnClick()
    {

    }

	private void OnCollisionEnter2D(Collision2D collision)
	{
		//canUseAbility = false;
	}
}
