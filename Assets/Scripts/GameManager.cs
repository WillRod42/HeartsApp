using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
	public const int NUMBER_OF_PASSED_CARDS = 3;

	public static int[] scores;
	public static bool isHeartsBroken;
	public static bool addExtraCardsToTrick;

	private DeckManager deck;
	private TrickManager trickManager;
	private List<GameObject> selectedCards;
	

  private void Start()
  {
		addExtraCardsToTrick = false;
		deck = GetComponent<DeckManager>();
		trickManager = GetComponent<TrickManager>();
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
		PhaseManager.onPassingPhase += ClearSelectedCards;
		PhaseManager.onPassingPhase += deck.LogHands;
  }

	public List<GameObject> GetSelectedCards()
	{
		return selectedCards;
	}

	public static void BreakHearts()
	{
		isHeartsBroken = true;
		addExtraCardsToTrick = true;
	}

	private void ClearSelectedCards()
	{
		selectedCards.Clear();
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
					trickManager.PlayCard(selectedCard, 0);
					break;
			}
		}
	}
}
