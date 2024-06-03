using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MapSelectManager : MonoBehaviour
{
	[SerializeField] GameObject goBackBtnGO;
	[SerializeField] GameObject buttonSetGO;
    [SerializeField] GameObject levelButtonGO; // level button prefab
    [SerializeField] int lvlCount;


	private void Start()
	{
		for (int i = 1; i <= lvlCount; i++)
		{
			GameObject copyGO = Instantiate(levelButtonGO, buttonSetGO.transform);
			int idxCopy = i;
			copyGO.name = "Button Lvl " + idxCopy.ToString();
			copyGO.GetComponent<Button>().onClick.AddListener(() => { SceneManager.LoadScene("Level" + idxCopy); }) ;
			copyGO.GetComponentInChildren<TextMeshProUGUI>().text = idxCopy.ToString();
		}

		goBackBtnGO.GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("Lobby"));
	}

	
}
