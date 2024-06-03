using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    CircleCollider2D circleColl2D;
	[SerializeField] float explosionRadius;
	[SerializeField] float explosionExpandTime;
	[SerializeField] float destroyTime;

	private void Start()
	{
		circleColl2D = GetComponent<CircleCollider2D>();
		//StartCoroutine(Explode());
		StartCoroutine(DelayedDestroy());
	}


	IEnumerator Explode()
	{
		float elapsedTime = 0f;
		while(elapsedTime < explosionExpandTime)
		{
			circleColl2D.radius = explosionRadius * (elapsedTime / explosionExpandTime);

			elapsedTime += Time.deltaTime;
			yield return null;
		}

		yield return new WaitForSeconds(0.3f);
		circleColl2D.radius = 0f;
	}

	IEnumerator DelayedDestroy()
	{
		yield return new WaitForSeconds(destroyTime);
		Destroy(this.gameObject);
	}
}
