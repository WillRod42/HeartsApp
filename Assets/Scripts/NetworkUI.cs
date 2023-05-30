using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkUI : MonoBehaviour
{
	[SerializeField] private Button hostStartBtn;
	[SerializeField] private Button clientStartBtn;

	private void Awake()
	{
		hostStartBtn.onClick.AddListener(() => {
			Debug.Log("Starting Host...");
			NetworkManager.Singleton.StartHost();
		});
		clientStartBtn.onClick.AddListener(() => {
			Debug.Log("Starting Client...");
			NetworkManager.Singleton.StartClient();
		});
	}
}
