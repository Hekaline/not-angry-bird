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
	private GameObject birdSet; // 발사될 수 있는 새들 모음
	public GameObject selectedBird; // 선택된 새
	//public TrailRenderer trajTrailRenderer;
	public LineRenderer trajLineRenderer; // 궤적 라인 렌더러
	[SerializeField] float maxPowerLimit; // 최대 파워 제한
	[SerializeField] float powerMultiple; // 새총 파워 배수
	[SerializeField] GameObject firePos; // 새 발사 위치 게임오브젝트
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
			// 씬 내의 새(bird) 숫자를 자식 개수로 설정
			GameManager.Instance.BirdCntInScene = birdSet.transform.childCount;
			
			// 선택된 새를 birdSet의 첫 번째 자식으로 설정
			selectedBird = birdSet.transform.GetChild(0).gameObject;
			selectedBird.transform.position = firePos.transform.position;

			for (int i = 1; i < birdSet.transform.childCount; i++)
			{
				// i번째 자식의 위치 설정 (i가 클 수록 점점 왼쪽으로 이동)
				Transform otherBird = birdSet.transform.GetChild(i);
				float x = firePos.transform.position.x - (i * 1);
				float y = firePos.transform.position.y - 1;
				otherBird.position = new Vector2(x, y);
			}
		} else
		{
			// 새가 없음
			selectedBird = null;
		}
	}
	
	private void Update()
	{
// 윈도우 에디터 혹은 윈도우 스탠드얼론
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
		// 드래그 시작
		if (Input.GetMouseButtonDown(0))
		{
			StartDragging();
		}
		// 드래그 중
		if (Input.GetMouseButton(0))
		{
			ContinueDragging();
		}
		// 드래그 끝
		if (Input.GetMouseButtonUp(0))
		{
			StopDragging();
		}
		
// 안드로이드
#elif UNITY_ANDROID
		
		// 터치 중일 때
		if (Input.touchCount == 1)
		{
			// 드래그 시작하지 않음(처음 터치)
			if (isDragging == false)
			{
				isDragging = true;
				StartDragging();
			}
			// 드래그 중
			else
			{
				ContinueDragging();
			}
		}
		// 터치 중이 아님
		else if (Input.touchCount == 0)
		{
			// 터치하고 있었을 때 터치 중지
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
		// 놓은 위치 - 클릭 시작한 위치로 파워 계산
		distance = endPos - startPos;
		Vector2 dir = (startPos - endPos).normalized; 
		
		// 적 방향이 아닌, 뒤로 조준했을 때 얼리 리턴
		if (dir.x <= 0)
		{
			return;
		}

		// 선택된 새의 컨트롤러가 있을 때 능력 사용 가능 활성화 (이제 한 번 더 화면 클릭 시 능력 활성화)
		if (selectedBird.TryGetComponent<BirdController>(out var component))
		{
			component.canUseAbility = true;
		}
		
		// 남은 새 개수 -1
		GameManager.Instance.BirdCntInScene -= 1;
		float power = Mathf.Abs(Vector2.Distance(startPos, endPos));

		// 최대 파워 제한
		power = Mathf.Clamp(power, 0, maxPowerLimit);
		print(power);

		// 선택된 새의 리지드바디 시뮬레이션 on 및 중력 활성화
		var rigid = selectedBird.GetComponent<Rigidbody2D>();
		rigid.simulated = true;
		rigid.gravityScale = 1;
		
		// 새에 힘 적용
		rigid.AddForce(power * powerMultiple * dir, ForceMode2D.Impulse);

		// 새의 오디오 활성화 (날아갈 때 내는 울음소리)
		selectedBird.GetComponent<AudioSource>().Play();
		
		// 선택된 새를 부모(birdSet)에서 제거
		selectedBird.transform.SetParent(null);

		// 새를 앞으로 1칸씩 당김
		ReloadBirdSet();
	}

	private void StartDragging()
	{
		isDragging = true;
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
		// 윈도우(마우스 포인터 위치)
		startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
#elif UNITY_ANDROID
		// 안드로이드(터치한 위치)
		startPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
#endif
	}

	private void ContinueDragging()
	{
		// 궤적 점의 개수를 0개로 설정해 보이지 않게 하기
		trajLineRenderer.positionCount = 0;
		//trajTrailRenderer.Clear();
		if (isDragging)
		{
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
			// 윈도우 (마우스 포인터 위치)
			endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
#elif UNITY_ANDROID
			// 안드로이드 (터치한 위치)
			endPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
#endif
			// 클릭 시작한 위치 - 현재 클릭하고 있는 위치로 거리에 따른 힘 계산 후
			float force = Mathf.Clamp(Mathf.Abs(Vector2.Distance(endPos, startPos)), 0, maxPowerLimit);
			// 예상 궤적 그리기 (힘 배수 * 거리에 따른 힘)
			DrawTrajectory(startPos, endPos, powerMultiple * force);
		}
		
	}

	private void StopDragging()
	{
		isDragging = false;
		trajLineRenderer.positionCount = 0;

		// 클릭 놓을 시 발사
		Fire();
	}


	// 궤적 그리기
	void DrawTrajectory(Vector2 start, Vector2 end, float _force)
	{
		Vector2 dist = end - start;
		if (dist.x >= 0) return;
		
		// 당긴 쪽의 반대로 날려야 하므로 dist 뒤집기
		// 새의 질량을 나눠서 정확한 힘 계산
		Vector2 force = -dist.normalized * _force / selectedBird.GetComponent<Rigidbody2D>().mass;

		// 선의 간격이 0.1초 사이의 간격임
		float timeInterval = 0.1f;
		
		// 새가 날아간 지 (1초에 생기는 선 개수 * 1.5) 점의 개수 정도까지의 거리 계산
		int numPoints = Mathf.CeilToInt(1f / timeInterval) * 3 / 2 - 1;
		trajLineRenderer.positionCount = numPoints;
		
		// 궤적 선의 시작하는 굵기
		trajLineRenderer.startWidth = 0.05f; 
		
		// 궤적 선의 끝나는 굵기
		trajLineRenderer.endWidth = 0.05f;

		// 점 위치를 지정하는 배열 초기화
		Vector3[] positions = new Vector3[numPoints];
		
	
		for (int i = 0; i < numPoints; i++)
		{
			// i에 따른 새가 날아간 후의 시간 계산 (나누기 2로 궤적 길이 너프)
			float time = i * timeInterval / 2;
			
			// time에 따른 새 위치 계산
			Vector2 position = CalculateProjectilePosition(selectedBird.transform.position, force, time);
			positions[i] = position;
		}
		
		// positions 배열로 점들 위치 지정
		trajLineRenderer.SetPositions(positions);
	}
	
	// 시간에 따른 날아가는 새의 위치 계산
	Vector2 CalculateProjectilePosition(Vector2 start, Vector2 force, float time)
	{
		// 중력가속도 (보통 9.81f)
		float gravity = Physics2D.gravity.y;
		
		// 시작 위치 + 힘 방향 * 시간
		float x = start.x + force.x * time;
		float y = start.y + force.y * time + 0.5f * gravity * time * time;
		return new Vector2(x, y);
	}
}
