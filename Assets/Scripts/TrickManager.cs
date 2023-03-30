using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickManager : MonoBehaviour
{
	[Range(0.1f, 5f)]
	public float aiDelay;

	[Range(0.1f, 5f)]
	public float roundDelay;

	public GameObject trickArea;

	private const int SCORE_QUEEN_OF_SPADES = 13;

	private DeckManager deck;
	private GameManager gameManager;
	private List<GameObject> trick;
	private List<GameObject>[] cardPiles;
	private UIManager ui;
	private int currPlayerTurn;
	private int round;

  void Start()
  {
    deck = GetComponent<DeckManager>();
		gameManager = GetComponent<GameManager>();
		trick = new List<GameObject>();
		ui = GetComponent<UIManager>();
		currPlayerTurn = 0;
		round = 0;

		cardPiles = new List<GameObject>[deck.numPlayers];
		for (int i = 0; i < deck.numPlayers; i++)
		{
			cardPiles[i] = new List<GameObject>();
		}
  }

	public int GetTrickSize()
	{
		return trick.Count;
	}

	public void StartRound()
	{
		round++;
		ui.HidePlayerLabels();
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
					SetCurrPlayer(i);
					return;
				}
			}
		}
	}

	public void SetCurrPlayer(int playerIndex)
	{
		currPlayerTurn = playerIndex;
		ui.setActivePlayerTxt(currPlayerTurn);
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
			else if (deck.HandHasSuit(playerHand, "S") || deck.HandHasSuit(playerHand, "C") || deck.HandHasSuit(playerHand, "D"))
			{
				return GameManager.isHeartsBroken || playedSuit != "H";
			}
			else
			{
				return true;
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
		string winningCard = "";

		Utility.Log("ROUND " + round, "");
		for (int i = 0; i < deck.numPlayers; i++)
		{
			if (currPlayerTurn != 0)
			{
				yield return new WaitForSeconds(aiDelay);
				if (trick.Count < deck.numPlayers)
				{
					SimpleOpponentTest opponent = deck.getOpponent(currPlayerTurn);
					opponent.PlayCard(PlayCard);

					Debug.Log("Player " + (playerTurn + 1) + ": " + trick[^1].name);
				}
			}

			yield return new WaitUntil(() => playerTurn != currPlayerTurn);

			if (checkIfNewCardWinning())
			{
				winningPlayerIndex = playerTurn;
				winningCard = trick[^1].name;
			}

			playerTurn = currPlayerTurn;
		}

		Utility.Log("ROUND OVER", "Winner: Player " + (winningPlayerIndex + 1));
		Debug.Log("Winning Card: " + winningCard);

		cardPiles[winningPlayerIndex].AddRange(trick);

		if (GameManager.addExtraCardsToTrick)
		{
			cardPiles[winningPlayerIndex].AddRange(deck.GetExtraCards());
			GameManager.addExtraCardsToTrick = false;
		}

		yield return new WaitForSeconds(roundDelay);
		
		foreach(GameObject card in trick)
		{
			card.SetActive(false);
		}
		
		SetCurrPlayer(winningPlayerIndex);
		trick.Clear();

		StopCoroutine("PlayRound");
		if (deck.GetHand(0).Count > 0)
		{
			StartRound();
		}
		else
		{
			ScoreHand();
		}
	}

	private void ScoreHand()
	{
		for (int i = 0; i < cardPiles.Length; i++)
		{
			foreach (GameObject card in cardPiles[i])
			{
				GameManager.scores[i] += ScoreCard(card.name);
			}
		}

		ui.UpdateScores(gameManager.GetScores());
		PhaseManager.RunPhase();
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
			if ((trick[0]).name[^1] == (trick[i]).name[^1] && deck.CompareCards(latestCard, trick[i]) <= 0)
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

			playedCard.SetActive(true);

			deck.PlaceCards(trick, deck.numPlayers, trickArea.transform.position, trickArea.GetComponent<SpriteRenderer>().bounds.size);

			ui.UpdatePlayerLabel(playedCard.transform.position, playerIndex);
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
			SetCurrPlayer(0);
		}
		else
		{
			SetCurrPlayer(currPlayerTurn + 1);
		}
	}
}
