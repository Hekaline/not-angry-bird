using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance => _instance;
    private static SoundManager _instance;

	public List<AudioSource> list;

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

		GameObject[] objs = SceneManager.GetActiveScene().GetRootGameObjects();
		foreach (var obj in objs)
		{
			AudioSource[] audios = GetComponentsInChildren<AudioSource>();
			foreach (var audio in audios)
			{
				list.Add(audio);
			}
		}
	}

	public void SetVolumeAll(float volume)
	{
		SetVolumeInList(list, volume);
	}

	public void SetVolumeInList(List<AudioSource> objs, float volume)
	{
		foreach (var obj in objs)
		{
			obj.volume = volume;
		}
	}
}
