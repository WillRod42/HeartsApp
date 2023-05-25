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
			NetworkManager.Singleton.StartHost();
		});
		hostStartBtn.onClick.AddListener(() => {
			NetworkManager.Singleton.StartClient();
		});

		
	}
}
