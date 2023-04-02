using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NormalAI : AIOpponent
{
	public NormalAI(int playerIndex) : base(playerIndex) {}

	public override GameObject[] GetPassingCards()
	{
		// temp
		return hand.GetRange(0, 3).ToArray();
	}

	public override GameObject PlayCard(checkIfLegalDelegate checkIfLegal, List<GameObject> trick)
	{
		List<GameObject> validCards = GetValidCards(checkIfLegal, trick);
		Utility.LogArray<GameObject>(validCards.ToArray());
		if (trick.Count == 0)
		{
			Debug.Log("Leading");
			return DeckManager.GetLowCard(validCards);
		}
		else
		{
			if (validCards[0].name[^1] != trick[0].name[^1])
			{
				Debug.Log("Sloughing high card");
				return DeckManager.GetHighCard(validCards);
			}
			else
			{
				List<GameObject> nonWinningCards = new List<GameObject>();
				for (int i = 0; i < validCards.Count; i++)
				{
					if (!CanWinTrick(validCards[i], trick))
					{
						nonWinningCards.Add(validCards[i]);
					}
				}

				if (nonWinningCards.Count == 0)
				{
					if (trick.Count == numPlayers - 1)
					{
						Debug.Log("Have to take trick, getting rid of high card");
						return DeckManager.GetHighCard(validCards);
					}
					else
					{
						Debug.Log("Have to take trick, hope I won't take it");
						return DeckManager.GetLowCard(validCards);
					}
				}
				else
				{
					Debug.Log("Highest card without taking");
					return DeckManager.GetHighCard(nonWinningCards);
				}
			}
		}
	}

	private bool CanWinTrick(GameObject card, List<GameObject> trick)
	{
		if (trick.Count == 0)
		{
			return true;
		}
		else
		{
			if (trick[0].name[^1] != card.name[^1])
			{
				return false;
			}
			else
			{
				return DeckManager.CompareCards(card, GetWinningCard(trick)) > 0;
			}
		}
	}

	private GameObject GetWinningCard(List<GameObject> trick)
	{
		if (trick.Count == 0)
		{
			return null;
		}
		else if (trick.Count == 1)
		{
			return trick[0];
		}
		else
		{
			char suit = trick[0].name[^1];
			GameObject winningCard = trick[0];
			foreach (GameObject card in trick.Skip(1))
			{
				if (card.name[^1] == suit && DeckManager.CompareCards(card, winningCard) > 0)
				{
					winningCard = card;
				}
			}

			return winningCard;
		}
	}
}

/****************************

- Get all valid cards 																				[X]
- Remove cards that would win trick 													[X]
- Play highest out of those																		[X]
- If leading, play lowest card																[X]
- If they have to take a trick, use lowest card								[X]
- If they can slough a card, play highest card								[X]
- If last and have to take, play highest card									[X]

*****************************/
