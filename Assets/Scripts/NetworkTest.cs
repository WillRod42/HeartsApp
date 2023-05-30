using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkTest : NetworkBehaviour
{
	private NetworkVariable<int> networkNumber = new NetworkVariable<int>(0);

	private void Awake()
	{
		StartCoroutine("LogNetworkNumber");
	}

	private void OnTouch()
	{
		networkNumber.Value = networkNumber.Value + 1;
	}

	private IEnumerator LogNetworkNumber()
	{
		yield return new WaitForSeconds(1);
		Debug.Log(OwnerClientId + " - number: " + networkNumber.Value);
		StartCoroutine("LogNetworkNumber");
	}
}
