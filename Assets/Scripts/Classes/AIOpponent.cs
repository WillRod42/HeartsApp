using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIOpponent
{
	public static int numPlayers;
	public delegate bool checkIfLegalDelegate(GameObject playedCard, GameObject leadCard, List<GameObject> playerHand);
	public List<GameObject> hand;

	protected int playerIndex;

	

	public AIOpponent(int playerIndex)
	{
		this.playerIndex = playerIndex;
	}

	protected List<GameObject> GetValidCards(checkIfLegalDelegate checkIfLegal, List<GameObject> trick)
	{
		List<GameObject> validCards = new List<GameObject>();
		GameObject leadCard = trick.Count > 0 ? trick[0] : null;

		foreach(GameObject card in hand)
		{
			if (checkIfLegal(card, leadCard, hand))
			{
				validCards.Add(card);
			}
		}

		return validCards;
	}


	public abstract GameObject[] GetPassingCards();

	public abstract GameObject PlayCard(checkIfLegalDelegate checkIfLegal, List<GameObject> trick);
}
