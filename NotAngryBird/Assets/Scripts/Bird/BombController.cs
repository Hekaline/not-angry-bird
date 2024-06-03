using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : BirdController
{
	[SerializeField] float checkRadius;
	[SerializeField] float damage;
	[SerializeField] float explosionForce;
	[SerializeField] GameObject boomEffectGO;
	public override void OnClick()
	{
		GameObject effectGo = Instantiate(boomEffectGO, transform.position, Quaternion.identity);
		Explode();

		canUseAbility = false;
	}

	private void Explode()
	{
		int enemyLayer = LayerMask.GetMask("Enemy");
		int obstacleLayer = LayerMask.GetMask("Obstacle");
		int protectedLayer = LayerMask.GetMask("Protected");
		int birdLayer = LayerMask.GetMask("Bird");

		Collider2D[] colliders;

		colliders = Physics2D.OverlapCircleAll(transform.position, checkRadius, obstacleLayer);

		List<Collider2D> wasProtected = new();
		foreach (Collider2D collider in colliders)
		{
			print(collider);

			BlockController bc = collider.GetComponent<BlockController>();
			bc.Hp -= damage;

			if (bc.Hp <= 0)
			{
				if (bc.containsEnemy)
				{
					Transform childTransform = collider.transform.GetChild(0);
					childTransform.SetParent(null);
					//childTransform.gameObject.layer = LayerMask.NameToLayer("Enemy");
					childTransform.GetComponent<EnemyController>().Hp += bc.Hp; // ��� ������ �ʰ��� ��ŭ ü�� ����

					wasProtected.Add(childTransform.GetComponent<CircleCollider2D>());
				}
			}


			AddForceToCollider(collider);
		}

		colliders = Physics2D.OverlapCircleAll(transform.position, checkRadius, enemyLayer);

		foreach (Collider2D collider in colliders)
		{
			print(collider);
			collider.GetComponent<EnemyController>().Hp -= damage;

			AddForceToCollider(collider);
		}

		foreach (Collider2D collider in wasProtected)
		{
			collider.gameObject.layer = LayerMask.NameToLayer("Enemy");
			collider.enabled = true;

			AddForceToCollider(collider);
			print("enabled");
		}

		colliders = Physics2D.OverlapCircleAll(transform.position, checkRadius, birdLayer);
		foreach (Collider2D collider in colliders)
		{
			//collider.GetComponent<BirdController>().Hp -= damage;
			AddForceToCollider(collider);
		}

		//colliders = Physics2D.OverlapCircleAll(transform.position, checkRadius, protectedLayer);
		//foreach(Collider2D collider in colliders)
		//{
		//	AddForceToCollider(collider);
		//}


	}

	private void AddForceToCollider(Collider2D coll2D)
	{
		Rigidbody2D rb2D = coll2D.GetComponentInChildren<Rigidbody2D>();
		if (rb2D != null)
		{
			Vector2 dir = coll2D.transform.position - transform.position;
			var actualForce = explosionForce * (1f / dir.magnitude);
			
			if (float.IsNaN(actualForce))
				rb2D.AddForce(dir * actualForce);
		}
	}
}
