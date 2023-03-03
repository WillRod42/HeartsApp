using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickManager : MonoBehaviour
{
	private const int SCORE_QUEEN_OF_SPADES = 13;

	private DeckManager deck;
	private List<GameObject> trick;
	

  void Start()
  {
    deck = GetComponent<DeckManager>();
  }

	public bool CheckIfLegalPlay(GameObject playedCard, GameObject leadCard, List<GameObject> playerHand)
	{
		string playedSuit = "" + deck.GetCardValue(playedCard)[^1];
		string leadSuit = "" + deck.GetCardValue(leadCard)[^1];
		if (!deck.HandHasSuit(playerHand, leadSuit))
		{
			if (playedSuit == "H")
			{
				GameManager.BreakHearts();
			}

			return true;
		}

		return playedSuit == leadSuit;
	}

	public bool CheckIfLegalPlay(GameObject playedCard)
	{
		string suit = "" + deck.GetCardValue(playedCard)[^1];
		return GameManager.isHeartsBroken || suit != "H";
	}

	public bool PlayCard(GameObject playedCard, int playerIndex)
	{
		List<GameObject> hand = deck.GetHand(playerIndex);
		if ((trick.Count == 0 && CheckIfLegalPlay(playedCard)) || CheckIfLegalPlay(playedCard, trick[0], hand))
		{
			trick.Add(playedCard);
			hand.Remove(playedCard);

			if (trick.Count == deck.numPlayers)
			{
				foreach (GameObject card in trick)
				{
					GameManager.scores[playerIndex] += ScoreCard(deck.GetCardValue(card));
				}

				if (GameManager.addExtraCardsToTrick)
				{
					foreach (GameObject card in deck.GetExtraCards())
					{
						GameManager.scores[playerIndex] += ScoreCard(deck.GetCardValue(card));
					}
					GameManager.addExtraCardsToTrick = false;
				}

				trick.Clear();
			}

			return true;
		}

		return false;
	}

	public int ScoreCard(string cardVal)
	{
		if (cardVal == "QS")
		{
			return SCORE_QUEEN_OF_SPADES;
		}
		else
		{
			return cardVal[^1] == 'H' ? 1 : 0;
		}
	}
}
