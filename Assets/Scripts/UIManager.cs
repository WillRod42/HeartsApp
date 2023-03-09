using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
	[Range(0, 500)]
	public int playerLabelVertOffset;
	public Canvas canvas;
	public GameObject playerLabelPrefab;

	public TMP_Text activePlayerTxt;
	public GameObject scoreboard;
	public GameObject scores;
	
	private DeckManager deck;
	private List<GameObject> playerScoresUI;
	private List<GameObject> playerPlayLabels;

	private void Start()
	{
		deck = GetComponent<DeckManager>();
		playerScoresUI = new List<GameObject>();
		playerPlayLabels = new List<GameObject>();

		Transform[] children = scores.GetComponentsInChildren<RectTransform>();
		foreach (Transform child in children)
		{
			if (child.parent == scores.transform)
			{
				playerScoresUI.Add(child.gameObject);
			}
		}

		InitUI(deck.numPlayers);
	}

	public void setActivePlayerTxt(int currTurn)
	{
		activePlayerTxt.text = "Active Player: " + (currTurn + 1);
	}

	public void ToggleScoreBoard()
	{
		scoreboard.SetActive(!scoreboard.activeSelf);
	}

	public void UpdateScores(int[] newScores)
	{
		for (int i = 0; i < newScores.Length; i++)
		{
			playerScoresUI[i].GetComponentsInChildren<Transform>()[2].gameObject.GetComponent<TMP_Text>().text = newScores[i].ToString();
		}
	}

	// playerIndex: player 1 is 0, player 2 is 1, etc.
	public void UpdatePlayerLabel(Vector3 cardPos, int playerIndex)
	{
		Vector3 cardUIPos = Camera.main.WorldToScreenPoint(cardPos);
		GameObject label = playerPlayLabels[playerIndex];
		label.SetActive(true);
		label.transform.position = new Vector3(cardUIPos.x, cardUIPos.y - playerLabelVertOffset, 0);
	}

	public void HidePlayerLabels()
	{
		foreach (GameObject label in playerPlayLabels)
		{
			label.SetActive(false);
		}
	}

	private void InitUI(int numPlayers)
	{
		for (int i = playerScoresUI.Count - 1; i >= numPlayers; i--)
		{
			playerScoresUI[i].gameObject.SetActive(false);
		}

		for (int i = 0; i < numPlayers; i++)
		{
			GameObject playerLabel = Instantiate(playerLabelPrefab);
			playerLabel.name = "Player " + (i + 1);
			playerLabel.GetComponent<TMP_Text>().text = playerLabel.name;
			playerLabel.GetComponent<TMP_Text>().fontSize = 36;
			playerLabel.transform.SetParent(canvas.transform);
			playerLabel.SetActive(false);
			
			playerPlayLabels.Add(playerLabel);
		}
	}
}
