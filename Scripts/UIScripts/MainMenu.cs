using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	[SerializeField]
	private Scene gameScene;

	public void PlayGame()
	{
		SceneManager.LoadScene("GameScene");
	}
	public void QuitGame()
	{
		Application.Quit();
	}
}
