using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	[SerializeField]
	private float maxHp;
	private float hp;
	public virtual float Hp
	{
		get => hp;
		set
		{
			hp = value;
			if (hp <= 0f)
			{
				GameObject effectGO = Instantiate(deathEffectGO, transform.position, Quaternion.identity);

				Destroy(gameObject);
			} else if (hp <= maxHp / 3f)
			{
				mySprite.sprite = heavilyInjuredSprite;
			} else if (hp <= maxHp / 3f * 2f)
			{
				mySprite.sprite = slightlyInjutedSprite;
			} else
			{
				mySprite.sprite = notInjuredSprite;
			}
		}
	}

	private SpriteRenderer mySprite;

	[SerializeField]
	private Sprite heavilyInjuredSprite;

	[SerializeField]
	private Sprite slightlyInjutedSprite;

	[SerializeField]
	private Sprite notInjuredSprite;
	

	[SerializeField]
	private GameObject deathEffectGO;


	void Start()
	{
		hp = maxHp;
		mySprite = GetComponent<SpriteRenderer>();
	}


	private void OnCollisionEnter2D(Collision2D collision)
	{
		float collisionForce = collision.relativeVelocity.magnitude;
		if (collision.collider.TryGetComponent<Rigidbody2D>(out var rb2D))
		{
			collisionForce = collision.relativeVelocity.magnitude * rb2D.mass;
			
		} else
		{
			collisionForce = collision.relativeVelocity.magnitude;
		}
		Hp -= collisionForce;
	}


	private void OnDestroy()
	{
		GameManager.Instance.EnemyCntInScene -= 1;
	}
}
