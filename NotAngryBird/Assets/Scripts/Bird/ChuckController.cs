using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChuckController : BirdController
{
	[SerializeField] float abilityPower;

	public override void OnClick()
	{
		GetComponent<Rigidbody2D>().AddForce(Vector2.right * abilityPower, ForceMode2D.Impulse);

		canUseAbility = false;
	}
}
