using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickManager : MonoBehaviour
{
	private const int SCORE_QUEEN_OF_SPADES = 13;

	private DeckManager deck;
	private List<GameObject> trick;
	private int currPlayerTurn;

  void Start()
  {
    deck = GetComponent<DeckManager>();
		trick = new List<GameObject>();
		currPlayerTurn = 0;
  }

	public void StartRound()
	{
		StopCoroutine("PlayRound");
		StartCoroutine("PlayRound", PlayRound());
	}

	public void setFirstPlayer()
	{
		for (int i = 0; i < deck.numPlayers; i++)
		{
			List<GameObject> hand = deck.GetHand(i);
			foreach (GameObject card in hand)
			{
				if (card.name == "2C")
				{
					currPlayerTurn = i;
					return;
				}
			}
		}
	}

	public bool CheckIfLegalPlay(GameObject playedCard, GameObject leadCard, List<GameObject> playerHand)
	{
		string playedSuit = "" + playedCard.name[^1];
		if (leadCard == null)
		{
			if (playerHand.Count == DeckManager.DECK_LENGTH / deck.numPlayers)
			{
				return playedCard.name == "2C";
			}
			else
			{
				return GameManager.isHeartsBroken || playedSuit != "H";
			}
		}

		string leadSuit = "" + leadCard.name[^1];
		if (!deck.HandHasSuit(playerHand, leadSuit))
		{
			if (playedSuit == "H")
			{
				GameManager.BreakHearts(deck.hasExtraCards);
			}

			return true;
		}

		return playedSuit == leadSuit;
	}

	public IEnumerator PlayRound()
	{
		int playerTurn = currPlayerTurn;
		int winningPlayerIndex = currPlayerTurn;
		for (int i = 0; i < deck.numPlayers; i++)
		{
			if (currPlayerTurn != 0)
			{
				SimpleOpponentTest opponent = deck.getOpponent(currPlayerTurn);
				opponent.PlayCard(PlayCard);

				Utility.Log("Player " + playerTurn, trick[^1].name);
			}

			yield return new WaitUntil(() => playerTurn != currPlayerTurn);

			if (checkIfNewCardWinning())
			{
				winningPlayerIndex = playerTurn;
			}

			playerTurn = currPlayerTurn;
		}

		Utility.Log("ROUND OVER", "Winner: Player " + winningPlayerIndex);

		foreach (GameObject card in trick)
		{
			GameManager.scores[winningPlayerIndex] += ScoreCard(card.name);
		}

		if (GameManager.addExtraCardsToTrick)
		{
			foreach (GameObject card in deck.GetExtraCards())
			{
				GameManager.scores[winningPlayerIndex] += ScoreCard(card.name);
			}
			GameManager.addExtraCardsToTrick = false;
		}

		trick.Clear();
		currPlayerTurn = winningPlayerIndex;
		
		StopCoroutine("PlayRound");
		if (deck.GetHand(0).Count > 0)
		{
			StartRound();
		}
	}

	private bool checkIfNewCardWinning()
	{
		// Card must be in suit to take the trick
		if (trick.Count > 1 && (trick[0]).name[^1] != (trick[^1]).name[^1])
		{
			return false;
		}

		bool newCardWinning = true;
		GameObject latestCard = trick[^1];
		for (int i = trick.Count - 2; i >= 0; i--)
		{
			if (deck.CompareCards(latestCard, trick[i]) <= 0)
			{
				newCardWinning = false;
			}
		}

		return newCardWinning;
	}

	public bool PlayCard(GameObject playedCard, int playerIndex)
	{
		List<GameObject> hand = deck.GetHand(playerIndex);
		if ((trick.Count == 0 && CheckIfLegalPlay(playedCard, null, hand)) || (trick.Count > 0 && CheckIfLegalPlay(playedCard, trick[0], hand)))
		{
			trick.Add(playedCard);
			hand.Remove(playedCard);
			advancePlayerTurnQueue();
			return true;
		}

		return false;
	}

	private int ScoreCard(string cardVal)
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

	public int getCurrPlayerTurn()
	{
		return currPlayerTurn;
	}

	private void advancePlayerTurnQueue()
	{
		if (currPlayerTurn >= deck.numPlayers - 1)
		{
			currPlayerTurn = 0;
		}
		else
		{
			currPlayerTurn++;
		}
	}
}
