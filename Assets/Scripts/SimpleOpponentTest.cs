using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleOpponentTest : AIOpponent
{
	public SimpleOpponentTest(int playerIndex) : base(playerIndex) {}
	
	// Passes first 3 cards in hand
	public override GameObject[] GetPassingCards()
	{
		return hand.GetRange(0, 3).ToArray();
	}

	// Plays first legal card in hand
	public override GameObject PlayCard(checkIfLegalDelegate checkIfLegal, List<GameObject> trick)
	{
		int cardIndex = 0;
		GameObject card = hand[cardIndex];
		GameObject leadCard = trick.Count > 0 ? trick[0] : null;
		while (!checkIfLegal(card, leadCard, hand))
		{
			cardIndex++;
			card = hand[cardIndex];
		}

		return card;
	}
}
