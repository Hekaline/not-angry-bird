using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance => _instance;
    private static GameManager _instance;

	[SerializeField] GameObject onWinGO;
	[SerializeField] GameObject onLoseGO;
    [SerializeField] GameObject onPauseGO;
    [SerializeField] GameObject pauseBtnGO;
    [SerializeField] TextMeshProUGUI countdownText;
    [SerializeField] TextMeshProUGUI currentLevelText;

    bool isWin;
    int currentLevel;
    public bool IsWin
    {
        get => isWin;
        set
        {
            if (value == true && !IsLose)
            {
                stopCountdown = true;
                isWin = true;
                
                OnWinGame();
            } else
            {
                isWin = false;
                onWinGO.SetActive(false);
            }
        }
    }
    bool isLose;
    public bool IsLose
    {
        get => isLose;
        set
        {
            if (value == true && !IsWin)
            {
                isLose = true;
                OnLoseGame();
            } else
            {
                isLose = false;
                onLoseGO.SetActive(false);
            }
        }
    }

    private int _enemyCntInScene;
    public int EnemyCntInScene
    {
        get => _enemyCntInScene;
        set
        {
            _enemyCntInScene = value;
            if (_enemyCntInScene <= 0)
            {
                IsWin = true;
            }
        }
    }


    Coroutine countDownCoroutine;
    private int _birdCntInScene;
    public int BirdCntInScene
    {
        get => _birdCntInScene;
        set
        {
            _birdCntInScene = value;
            if (_birdCntInScene <= 0)
            {
                countDownCoroutine = StartCoroutine(CountdownBeforeLose());
            }
        }
    }

    private float _score;
    public float Score
    {
        get => _score;
        set => _score = value;
    }

	private void Awake()
	{
		if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
	}

    void Initialize()
    {
        IsWin = false;
        IsLose = false;                                                                                                 

        countdownText.gameObject.SetActive(false);
        if (countDownCoroutine != null)
        {
			StopCoroutine(countDownCoroutine);
            elapsedTime = 0f;
            countDownCoroutine = null;
		}
        
        _birdCntInScene = 0;
        _enemyCntInScene = 0;

        OnContinueGame();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		Initialize();
		if (scene.name.Contains("Level"))
        {
            // �� �� ����
            print($"{nameof(OnSceneLoaded)} called");
            pauseBtnGO.SetActive(true);

            currentLevelText.gameObject.SetActive(true);
            currentLevel = Convert.ToInt32(scene.name.Substring(5));
            currentLevelText.text = "Level " + currentLevel;

            
            Transform[] entityTransforms = FindObjectsOfType<Transform>(true);

            foreach (var entityTransform in entityTransforms)
            {
                if (entityTransform.CompareTag("Enemy"))
                {
                    EnemyCntInScene += 1;
                } else if (entityTransform.CompareTag("Bird"))
                {
                    BirdCntInScene += 1;
                }
            }
        } else
        {
            if (pauseBtnGO.activeSelf)
                pauseBtnGO.SetActive(false);
            
            if (currentLevelText.gameObject.activeSelf)
                currentLevelText.gameObject.SetActive(false);
        }
    }

	void OnWinGame()
    {
        if (!isLose)
        {
            stopCountdown = true;
            
            print("you won the level!");
            onWinGO.SetActive(true);
		} else
        {
            return;
        }
    }
    void OnLoseGame()
    {
        if (!isWin)
        {
            onLoseGO.SetActive(true);
        } else
        {
            return;
        }
    }
    
    public void OnGiveUp()
    {
        IsLose = true;
    }
    
    public void LoadChooseLevelScene()
    {
        stopCountdown = true;
        SceneManager.LoadScene("MapSelect");
    }
    
    public void OnPauseGame()
    {
        if (!IsWin && !IsLose)
        {
			Time.timeScale = 0;
			onPauseGO.SetActive(true);
		}
    }
    
    public void OnContinueGame()
    {
        Time.timeScale = 1f;
        onPauseGO.SetActive(false);
    }
    
    public void OnReloadGame()
    {
        var allTransforms = FindObjectsOfType<Transform>();

        // disable IsWin
        _enemyCntInScene = int.MaxValue;
        foreach (var entityTransform in allTransforms)
        {
            if (entityTransform.CompareTag("Bird") || entityTransform.CompareTag("Enemy"))
            {
                Destroy(entityTransform.gameObject);
            }
        }
        
        StartCoroutine(ReloadGameCoroutine());
    }

    private IEnumerator ReloadGameCoroutine()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        stopCountdown = true;

        StopAllCoroutines();
        yield break;
    }
    
    public void OnClickNextLevel()
    {
        LoadLevel(currentLevel + 1);
    }

    public void LoadLevel(int value)
    {
        try
        {
			SceneManager.LoadScene("Level" + value.ToString());
			currentLevel = value;
		} catch
        {
            print("the scene doesn't exist");
        }
		
	}

	[SerializeField] float timeToWaitBeforeLose;
    bool stopCountdown = false;
    bool isCounting = false;
	float elapsedTime = 0f;
    
	IEnumerator CountdownBeforeLose()
    {
        stopCountdown = false;
        isCounting = true;
        countdownText.gameObject.SetActive(true);

        
        while (elapsedTime < timeToWaitBeforeLose)
        {
            if (IsWin || stopCountdown)
            {
                stopCountdown = false;

                elapsedTime = 0f;
				countdownText.gameObject.SetActive(false);
				yield break;
            }

            countdownText.text = "Countdown: " + (int)(timeToWaitBeforeLose - elapsedTime) + "s";

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        countdownText.gameObject.SetActive(false);
        IsLose = true;
        isCounting = false;
    }
}
