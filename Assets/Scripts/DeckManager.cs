using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
	public const int DECK_LENGTH = 52;
	public const int CARDS_PER_SUIT = 13;
	public const int NUMBER_PASSING_PHASES = 4;

	[Range(3, 6)]
	public int numPlayers;
	public bool debug;
	public bool hasExtraCards;

	private GameObject[] cards;
	private Sprite[] cardImages;
	private List<GameObject>[] hands; // First hand is player's hand, rest of hands go clockwise around the 'table'
	private List<GameObject> extraCards;
	private SimpleOpponentTest[] opponents;
	private UIManager ui;

	private float cardWidth;
	private float cardHeight;

  private void Start()
  {
		opponents = new SimpleOpponentTest[numPlayers - 1];
		for (int i = 0; i < opponents.Length; i++)
		{
			opponents[i] = new SimpleOpponentTest(i + 1);
		}

		ui = GetComponent<UIManager>();

		// Initialize cards and card images
		cards = new GameObject[DECK_LENGTH];
    cardImages = Resources.LoadAll<Sprite>("English_pattern_playing_cards_deck");//
		for (int i = 0; i < DECK_LENGTH; i++)
		{
			GameObject newCard = new GameObject();
			newCard.AddComponent<SpriteRenderer>();
			newCard.GetComponent<SpriteRenderer>().sprite = cardImages[i];
			newCard.AddComponent<BoxCollider2D>();
			newCard.SetActive(false);
			newCard.transform.SetParent(transform);
			newCard.name = GetCardValue(newCard);

			cards[i] = newCard;
		}

		cardWidth = cardImages[0].bounds.size.x;
		cardHeight = cardImages[0].bounds.size.y;

		PhaseManager.RunPhase();
  }


	public void LogHands()
	{
		if (debug)
		{
			string allHands = "\n";
			foreach(List<GameObject> hand in hands)
			{
				string handStr = "";
				foreach (GameObject card in hand)
				{
					handStr += " " + card.name + " ";
				}
				handStr = "[" + handStr + "]";
				allHands += handStr + "\n";
			}

			Utility.Log("HANDS", allHands);
		}
	}

	public void LogPasses(List<GameObject[]> passedCards)
	{
		if (debug)
		{
			string allPasses = "\n";
			foreach (GameObject[] cards in passedCards)
			{
				string passed = "";
				foreach (GameObject card in cards)
				{
					passed += " " + card.name + " ";
				}
				passed = "[" + passed + "]";
				allPasses += passed + "\n";
			}

			Utility.Log("PASSED", allPasses);
		}
	}

	public List<GameObject> GetHand(int index)
	{
		return hands[index];
	}

	public List<GameObject> GetExtraCards()
	{
		return extraCards;
	}

	public void DealHands()
	{
		// Initialize hands
		hands = new List<GameObject>[numPlayers];
		for (int i = 0; i < numPlayers; i++)
		{
			hands[i] = new List<GameObject>();
		}

		// Add cards to hands
		for (int i = 0; i < DECK_LENGTH; i += numPlayers)
		{
			for (int j = 0; j < numPlayers && j + i < DECK_LENGTH; j++)
			{
				hands[j].Add(cards[i + j]);
			}
		}

		foreach (List<GameObject> hand in hands)
		{
			SortHand(hand);
		}

		// Assign hands to opponents
		for (int i = 0; i < opponents.Length; i++)
		{
			opponents[i].hand = hands[i + 1]; // First hand is player hand, so we add 1
		}


		// Set aside extra cards if there are leftovers after dealing
		if (DECK_LENGTH % numPlayers != 0)
		{
			hasExtraCards = true;
			extraCards = new List<GameObject>();
			int numExtraCards = DECK_LENGTH % numPlayers;
			for (int i = DECK_LENGTH - numExtraCards; i < DECK_LENGTH; i++)
			{
				extraCards.Add(cards[i]);
			}
		}
		else
		{
			hasExtraCards = false;
		}
	}

	public void PlaceCards(List<GameObject> cards, int cardCount, Vector3 placeAreaCenter, Vector3 placeAreaSize)
	{
		List<Transform> transforms = new List<Transform>();
		foreach (GameObject card in cards)
		{
			transforms.Add(card.transform);
		}

		Utility.PlaceObjectsInSpread(transforms, cardCount, cardWidth, cardHeight, placeAreaCenter, placeAreaSize, Vector3.zero);
	}

	public void Shuffle()
	{
		Debug.Log("Shuffling Start");
		for (int i = 0; i < cards.Length; i++)
		{
			int j = Random.Range(0, i + 1);
			GameObject card = cards[i];
			cards[i] = cards[j];
			cards[j] = card;
		}
	}

	public void PassCards()
	{
		// Debug.Log("PASS");
		List<GameObject[]> passedCards = new List<GameObject[]>();
		passedCards.Add(GetComponent<GameManager>().GetSelectedCards().ToArray());

		if (passedCards[0].Length == 3)
		{
			foreach (SimpleOpponentTest opponent in opponents)
			{
				passedCards.Add(opponent.GetPassingCards());
			}

			LogPasses(passedCards);

			switch (PhaseManager.GetCurrRound() % NUMBER_PASSING_PHASES)
			{
				case 1: // Pass left
					for (int i = 0; i < numPlayers - 1; i++)
					{
						PassCards(passedCards[i], i, i + 1);
					}
					PassCards(passedCards[numPlayers - 1], numPlayers - 1, 0);
					break;

				case 2: // Pass right
					for (int i = numPlayers - 1; i > 0 ; i--)
					{
						PassCards(passedCards[i], i, i - 1);
					}
					PassCards(passedCards[0], 0, numPlayers - 1);
					break;

				case 3: // Pass diagonally (pass left but skip one player)
					if (numPlayers > 3)
					{
						for (int i = 0; i < numPlayers - 2; i++)
						{
							PassCards(passedCards[i], i, i + 2);
						}
						PassCards(passedCards[numPlayers - 2], numPlayers - 1, 0);
						PassCards(passedCards[numPlayers - 1], numPlayers - 1, 1);
					}
					break;
			}

			PhaseManager.RunPhase();
			ui.PassBtn.SetActive(false);
		}

	}

	private void PassCards(GameObject[] cards, int senderIndex, int recipientIndex)
	{
		List<GameObject> senderHand = hands[senderIndex];
		List<GameObject> recipientHand = hands[recipientIndex];

		foreach (GameObject card in cards)
		{
			card.SetActive(false);
			senderHand.Remove(card);
			recipientHand.Add(card);
		}

		SortHand(hands[recipientIndex]);
	}

	public bool HandHasSuit(List<GameObject> hand, string suit)
	{
		foreach (GameObject card in hand)
		{
			if (card.name[^1].ToString() == suit.ToUpper())
			{
				return true;
			}
		}

		return false;
	}

	public int CompareCards(GameObject card1, GameObject card2)
	{
		int[] card1Vals = CardValToInts(card1.name);
		int[] card2Vals = CardValToInts(card2.name);

		return (card1Vals[1] * CARDS_PER_SUIT + card1Vals[0]) - (card2Vals[1] * CARDS_PER_SUIT + card2Vals[0]);
	}

	public string GetCardValue(GameObject card)
	{
		string value = "";
		string[] suits = {"S", "H", "D", "C"};

		string imageName = card.GetComponent<SpriteRenderer>().sprite.name;
		int imageIndex = int.Parse(imageName.Split('_')[^1]);

		int cardValue = imageIndex % CARDS_PER_SUIT;

		// Keeps face card values as letters
		if (cardValue >= 10)
		{
			switch (cardValue - 10)
			{
				case 0: value += "J"; break;
				case 1: value += "Q"; break;
				case 2: value += "K"; break;
			}
		}
		else if (cardValue == 0)
		{
			value += "A";
		}
		else
		{
			value += (cardValue + 1);
		}
		
		switch (imageIndex / CARDS_PER_SUIT)
		{
			case 0: value += "S"; break;
			case 1: value += "H"; break;
			case 2: value += "D"; break;
			case 3: value += "C"; break;
		}

		return value;
	}

	private int[] CardValToInts(string cardVal)
	{
		int[] intVals = new int[2]; // 1st is card value, 2nd is suit

		string cardNumVal = cardVal.Substring(0, cardVal.Length - 1);
		switch (cardNumVal)
		{
			case "J": intVals[0] = 10; break;
			case "Q": intVals[0] = 11; break;
			case "K": intVals[0] = 12; break;
			case "A": intVals[0] = 13; break;
			default: intVals[0] = (int.Parse(cardNumVal) - 1); break;
		}

		string suit = "" + cardVal[^1];
		switch (suit)
		{
			case "S": intVals[1] = 0; break;
			case "H": intVals[1] = 1; break;
			case "C": intVals[1] = 2; break;
			case "D": intVals[1] = 3; break;
		}

		return intVals;
	}

	// Sorts by suit first then value
	private void SortHand(List<GameObject> hand)
	{
		hand.Sort(CompareCards);
	}

	// Use player index (i.e. 0 would be the player and not an opponent)
	public SimpleOpponentTest getOpponent(int playerIndex)
	{
		return opponents[playerIndex - 1];
	}

	public void ClearCards()
	{
		foreach (GameObject card in cards)
		{
			card.SetActive(false);
		}
	}
}
