using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
	[SerializeField] private Button btnStartGame;
	[SerializeField] private Button btnStartMultiplayer;

	private void Start()
	{
		btnStartGame.onClick.AddListener(() => {
			Debug.Log("Starting Singleplayer...");
			SceneManager.LoadScene("SampleScene");
		});

		btnStartMultiplayer.onClick.AddListener(() => {
			Debug.Log("Starting Multiplayer...");
			SceneManager.LoadScene("Multiplayer");
		});
	}
}
