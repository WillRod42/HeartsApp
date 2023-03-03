using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
	public const int NUMBER_OF_PASSED_CARDS = 3;

	public static int[] scores;
	public static bool isHeartsBroken;

	[SerializeField]
	private const int SCORE_QUEEN_OF_SPADES = 13;

	private DeckManager deck;
	private List<GameObject> selectedCards;
	private List<GameObject> trick;

  private void Start()
  {
		deck = GetComponent<DeckManager>();
		selectedCards = new List<GameObject>();
		scores = new int[deck.numPlayers];
		isHeartsBroken = false;

		// Subscribe methods to phase events
		PhaseManager.onDealingPhase += deck.Shuffle;
		PhaseManager.onDealingPhase += deck.DealHands;
		PhaseManager.onDealingPhase += deck.PlaceCards;
		PhaseManager.onDealingPhase += deck.LogHands;

		PhaseManager.onPassingPhase += deck.PassCards;
		PhaseManager.onPassingPhase += deck.PlaceCards;
		PhaseManager.onPassingPhase += deck.LogHands;
  }

	public List<GameObject> GetSelectedCards()
	{
		return selectedCards;
	}

	public bool CheckIfLegalPlay(GameObject playedCard, GameObject leadCard, List<GameObject> playerHand)
	{
		string playedSuit = "" + deck.GetCardValue(playedCard)[^1];
		string leadSuit = "" + deck.GetCardValue(leadCard)[^1];
		return playedSuit == leadSuit || !deck.HandHasSuit(playerHand, leadSuit); 
	}

	public bool CheckIfLegalPlay(GameObject playedCard)
	{
		string suit = "" + deck.GetCardValue(playedCard)[^1];
		return isHeartsBroken || suit != "H";
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
					scores[playerIndex] += ScoreCard(deck.GetCardValue(card));
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

	// Decides what tapping/left clicking non-ui elements does depending on the phase
	private void OnTouch()
	{
		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
		Vector2 rayOrigin = new Vector2(mousePos.x, mousePos.y);
		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.zero);
		if (hit.transform)
		{
			GameObject selectedCard = hit.collider.gameObject;
			switch (PhaseManager.GetCurrPhase())
			{
				case Phase.Passing:
					if (selectedCards.Contains(selectedCard))
					{
						Vector3 currPos = selectedCard.transform.position;
						selectedCard.transform.position = new Vector3(currPos.x, currPos.y - 1, 0);
						selectedCards.Remove(selectedCard);
					}
					else if (selectedCards.Count < NUMBER_OF_PASSED_CARDS)
					{
						Vector3 currPos = selectedCard.transform.position;
						selectedCard.transform.position = new Vector3(currPos.x, currPos.y + 1, 0);
						selectedCards.Add(selectedCard);
					}

					break;

				case Phase.Playing:
					PlayCard(selectedCard, 0);
					break;
			}
		}
	}
}
