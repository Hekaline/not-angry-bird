using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private Button playBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button infoBtn;
    void Start()
    {
        playBtn.onClick.AddListener(OnClickPlayBtn);
        exitBtn.onClick.AddListener(OnClickExitBtn);
        infoBtn.onClick.AddListener(OnClickInfoBtn);
    }
    void Update()
    {
        
    }

    void OnClickPlayBtn()
    {
        SceneManager.LoadScene("MapSelect");
    }

    void OnClickExitBtn()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

#else
        Application.Quit();
#endif
	}

	void OnClickInfoBtn()
    {

    }
}
