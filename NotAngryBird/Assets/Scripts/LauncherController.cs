using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LauncherController : MonoBehaviour
{
	Vector2 startPos;
	Vector2 endPos;
	Vector2 distance;

	[SerializeField]
	private GameObject birdSet;
	public GameObject selectedBird;
	//public TrailRenderer trajTrailRenderer;
	public LineRenderer trajLineRenderer;
	[SerializeField] float maxPowerLimit;
	[SerializeField] float powerMultiple;
	[SerializeField] GameObject firePos;
	bool isDragging = false;
	 

	private void Start()
	{
		//trajTrailRenderer = GetComponent<TrailRenderer>();//gameObject.AddComponent<TrailRenderer>();
		trajLineRenderer = GetComponent<LineRenderer>();
		//GameManager.Instance.BirdCntInScene = birdSet.transform.childCount;
		ReloadBirdSet();
	}

	private void ReloadBirdSet()
	{
		if (birdSet.transform.childCount >= 1)
		{
			GameManager.Instance.BirdCntInScene = birdSet.transform.childCount;
			selectedBird = birdSet.transform.GetChild(0).gameObject;
			selectedBird.transform.position = firePos.transform.position;

			for (int i = 1; i < birdSet.transform.childCount; i++)
			{
				// �� ����
				Transform otherBird = birdSet.transform.GetChild(i);
				float x = firePos.transform.position.x - (i * 1);
				float y = firePos.transform.position.y - 1;
				otherBird.position = new Vector2(x, y);
			}
		} else
		{
			selectedBird = null;
		}
	}
	
	private void Update()
	{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

		if (Input.GetMouseButtonDown(0))
		{
			StartDragging();
		}

		if (Input.GetMouseButton(0))
		{
			ContinueDragging();
		}

		if (Input.GetMouseButtonUp(0))
		{
			StopDragging();
		}
		
#elif UNITY_ANDROID
		
		if (Input.touchCount == 1)
		{
			if (isDragging == false)
			{
				isDragging = true;
				StartDragging();
			}
			else
			{
				ContinueDragging();
			}
		}
		else if (Input.touchCount == 0)
		{
			if (isDragging == true)
			{
				isDragging = false;
				StopDragging();
			}
		}
		
#endif
	}

	private void Fire()
	{		
		distance = endPos - startPos;
		Vector2 dir = (startPos - endPos).normalized; 
		if (dir.x <= 0)
		{
			return;
		}

		if (selectedBird.TryGetComponent<BirdController>(out var component))
		{
			component.canUseAbility = true;
		}
		GameManager.Instance.BirdCntInScene -= 1;
		float power = Mathf.Abs(Vector2.Distance(startPos, endPos));

		power = Mathf.Clamp(power, 0, maxPowerLimit);
		print(power);

		var rigid = selectedBird.GetComponent<Rigidbody2D>();
		rigid.simulated = true;
		rigid.gravityScale = 1;
		rigid.AddForce(power * powerMultiple * dir, ForceMode2D.Impulse);


		selectedBird.GetComponent<AudioSource>().Play();
		selectedBird.transform.SetParent(null);

		ReloadBirdSet();

	}

	private void StartDragging()
	{
		isDragging = true;
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
		startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		// trajTrailRenderer.AddPosition(firePos.transform.position);
#elif UNITY_ANDROID
		startPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
#endif
	}

	private void ContinueDragging()
	{
		trajLineRenderer.positionCount = 0;
		//trajTrailRenderer.Clear();
		if (isDragging)
		{
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
			endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
#elif UNITY_ANDROID
			endPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
#endif
			float force = Mathf.Clamp(Mathf.Abs(Vector2.Distance(endPos, startPos)), 0, maxPowerLimit);
			DrawTrajectory(startPos, endPos, powerMultiple * force);
		}
		
	}

	private void StopDragging()
	{
		isDragging = false;
		trajLineRenderer.positionCount = 0;

		Fire();
	}


	void DrawTrajectory(Vector2 start, Vector2 end, float _force)
	{
		Vector2 trajectory = end - start;
		if (trajectory.x >= 0) return;
		Vector2 force = -trajectory.normalized * _force / selectedBird.GetComponent<Rigidbody2D>().mass;

		float timeInterval = 0.1f;
		int numPoints = Mathf.CeilToInt(1f / timeInterval) * 3 / 2 - 1;
		trajLineRenderer.positionCount = numPoints;
		trajLineRenderer.startWidth = 0.05f; 
		trajLineRenderer.endWidth = 0.05f;

		Vector3[] positions = new Vector3[numPoints];
		for (int i = 0; i < numPoints; i++)
		{
			float time = i * timeInterval / 2;
			Vector2 position = CalculateProjectilePosition(selectedBird.transform.position, force, time);
			positions[i] = position;
		}
		trajLineRenderer.SetPositions(positions);
	}
	Vector2 CalculateProjectilePosition(Vector2 start, Vector2 force, float time)
	{
		float gravity = Physics2D.gravity.y;
		float x = start.x + force.x * time;
		float y = start.y + force.y * time + 0.5f * gravity * time * time;
		return new Vector2(x, y);
	}

}
