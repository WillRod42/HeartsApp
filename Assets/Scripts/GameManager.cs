using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public const int NUMBER_OF_PASSED_CARDS = 3;

	public static int[] scores;
	public static bool isHeartsBroken;
	public static bool addExtraCardsToTrick;
	public static bool gameOver;

	public bool debugSetScore;
	public int setUserScore;
	public int gameEndScore;
	public GameObject playerCardArea;

	private DeckManager deck;
	private TrickManager trickManager;
	private UIManager ui;
	private List<GameObject> selectedCards;
	private bool playedCard;

  private void Start()
  {
		addExtraCardsToTrick = false;
		deck = GetComponent<DeckManager>();
		trickManager = GetComponent<TrickManager>();
		ui = GetComponent<UIManager>();
		selectedCards = new List<GameObject>();
		scores = new int[deck.numPlayers];
		isHeartsBroken = false;
		playedCard = false;
		gameOver = false;

		scores[0] = debugSetScore ? setUserScore : 0;

		// Subscribe methods to phase events
		PhaseManager.onDealingPhase += deck.Shuffle;
		PhaseManager.onDealingPhase += deck.DealHands;
		PhaseManager.onDealingPhase += PlacePlayerCards;
		PhaseManager.onDealingPhase += deck.LogHands;

		PhaseManager.onPassingPhase += PlacePlayerCards;
		PhaseManager.onPassingPhase += ClearSelectedCards;
		PhaseManager.onPassingPhase += trickManager.setFirstPlayer;
		PhaseManager.onPassingPhase += deck.LogHands;
		PhaseManager.onPassingPhase += trickManager.StartRound;

		PhaseManager.onPlayingPhase += PhaseManager.RunPhase;
		
		PhaseManager.onScoringPhase += LogScores;
		PhaseManager.onScoringPhase += CheckIfGameEnd;
  }

	public List<GameObject> GetSelectedCards()
	{
		return selectedCards;
	}

	public static void BreakHearts(bool hasExtraCards)
	{
		isHeartsBroken = true;
		if (hasExtraCards)
		{
			addExtraCardsToTrick = true;
		}
	}

	public void PassCards()
	{
		PhaseManager.RunPhase();
		ui.PassBtn.SetActive(false);
	}

	public void LogScores()
	{
		string scoreStr = "\n";
		foreach (int score in scores)
		{
			scoreStr += score + "\n";
		}
		Utility.Log("SCORE BOARD", scoreStr);
	}

	public int[] GetScores()
	{
		return scores;
	}

	public void ResetGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	private void CheckIfGameEnd()
	{
		foreach (int score in scores)
		{
			if (score >= gameEndScore)
			{
				int winnerIndex = 0;
				for (int i = 1; i < scores.Length; i++)
				{
					if (scores[i] < scores[winnerIndex])
					{
						winnerIndex = i;
					}
				}
				
				ui.SetWinnerText(winnerIndex);
				ui.ToggleUIElement(ui.gameOverCanvas);
				ui.ToggleUIElement(ui.uiCanvas);
				ui.ToggleUIElement(ui.scoreboard);
				gameOver = true;
				break;
			}
		}
	}

	private void PlacePlayerCards()
	{
		deck.PlaceCards(deck.GetHand(0), DeckManager.DECK_LENGTH / deck.numPlayers, playerCardArea.transform.position, playerCardArea.GetComponent<SpriteRenderer>().bounds.size);
	}

	private void ClearSelectedCards()
	{
		selectedCards.Clear();
	}

	// Decides what tapping/left clicking non-ui elements does depending on the phase
	private void OnTouch()
	{
		Vector2 TouchPosition;
		Vector2 mousePos;
		PlayerInput input = GetComponent<PlayerInput>();
		if (input.currentControlScheme == "Touch")
		{
			mousePos = Camera.main.ScreenToWorldPoint(Touchscreen.current.position.ReadValue());
		}
		else
		{
			mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
		}

		TouchPosition = new Vector2(mousePos.x, mousePos.y);

		RaycastHit2D hit = new RaycastHit2D();
		RaycastHit2D[] hits = Physics2D.RaycastAll(TouchPosition, Vector2.zero);
		for (int i = 0; i < hits.Length; i++)
		{
			RaycastHit2D temp = hits[i];
			if (temp.transform)
			{
				if (!hit.transform)
				{
					hit = hits[i];
				}
				else
				{
					SpriteRenderer sprite = temp.collider.gameObject.GetComponent<SpriteRenderer>();
					if (sprite)
					{
						if (sprite.sortingOrder > hit.collider.gameObject.GetComponent<SpriteRenderer>().sortingOrder)
						{
							hit = temp;
						}
					}
				}
			}
		}

		if (hit.transform)
		{
			GameObject selectedCard = hit.collider.gameObject;
			if (deck.GetHand(0).Contains(selectedCard))
			{
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
						if (trickManager.getCurrPlayerTurn() == 0 && !playedCard && trickManager.GetTrickSize() < 4)
						{
							playedCard = true;
							if(trickManager.PlayCard(selectedCard, 0))
							{
								Debug.Log("User: " + selectedCard.name);
							}
							playedCard = false;
						}
						break;
				}
			}
		}
	}
}
