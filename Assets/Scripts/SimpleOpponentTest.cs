using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleOpponentTest
{
	public delegate bool playCardDelegate(GameObject playedCard, int playerIndex);
	public List<GameObject> hand;
	
	private int playerIndex;

	public SimpleOpponentTest(int playerIndex)
	{
		this.playerIndex = playerIndex;
	}

	// Just passes first 3 cards in hand
	public GameObject[] GetPassingCards()
	{
		return hand.GetRange(0, 3).ToArray();
	}

	public GameObject PlayCard(playCardDelegate playCard)
	{
		int cardIndex = 0;
		GameObject card = hand[cardIndex];
		while (!playCard(card, playerIndex))
		{
			cardIndex++;
			card = hand[cardIndex];
		}

		return card;
	}
}
