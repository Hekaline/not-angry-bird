using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TNT_Controller : BlockController
{
	[SerializeField] float checkRadius;
	[SerializeField] float damage;
	[SerializeField] float explosionForce;
	[SerializeField] GameObject boomEffectGO;
	public override float Hp 
	{
		get => hp;
		set
		{
			hp = value;
			if (hp <= 0)
			{
				GameObject effectGo = Instantiate(boomEffectGO, transform.position, Quaternion.identity);
				Destroy(gameObject);
			}
		}
	}
	private void Awake()
	{
		
	}

	private void OnDestroy()
	{
		BeforeDestroy();
	}

	protected override void BeforeDestroy()
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
			float actualForce = explosionForce * (1f / dir.magnitude);
			rb2D.AddForce(dir * actualForce);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, checkRadius);
		Gizmos.color = Color.red;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{

		// �浹�� ���
		float collisionForce = collision.relativeVelocity.magnitude / (collision.collider.GetComponent<Rigidbody2D>().mass);

		// ������ ü�� ����
		Hp -= collisionForce;

	}
}
