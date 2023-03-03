using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
	public int numPlayers;

	private GameObject[] cards;
	private Sprite[] cardImages;
	private List<GameObject>[] hands; // First hand is player's hand, rest of hands go clockwise around the 'table'
	private List<GameObject> extraCards;

	private SimpleOpponentTest[] opponents; //test

	private float cardWidth;
	private float cardHeight;

	private const int DECK_LENGTH = 52;
	private const int CARDS_PER_SUIT = 13;
	private const int NUMBER_PASSING_PHASES = 4;

  private void Start()
  {
		opponents = new SimpleOpponentTest[3];
		for (int i = 0; i < opponents.Length; i++)
		{
			opponents[i] = new SimpleOpponentTest();
		}

		// Initialize Deck and load card images
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

			cards[i] = newCard;
		}

		cardWidth = cardImages[0].bounds.size.x;
		cardHeight = cardImages[0].bounds.size.y;

  }

	public List<GameObject> getHand(int index)
	{
		return hands[index];
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
			extraCards = new List<GameObject>();
			int numExtraCards = DECK_LENGTH % numPlayers;
			for (int i = DECK_LENGTH - numExtraCards; i < DECK_LENGTH; i++)
			{
				extraCards.Add(cards[i]);
			}
		}
	}

	public void PlaceCards()
	{
		List<GameObject> playerHand = hands[0];

		Vector3 bottomLeftScreen = new Vector3(0, 0, 0);
		Vector3 bottomLeftCamera = Camera.main.ScreenToWorldPoint(bottomLeftScreen);

		Vector3 bottomRightScreen = new Vector3(Screen.width, 0, 0);
		Vector3 bottomRightCamera = Camera.main.ScreenToWorldPoint(bottomRightScreen);


		//Calculate offset
		float cardOffset = ((bottomRightCamera.x - cardWidth) - (bottomLeftCamera.x)) / (playerHand.Count - 1);

		// if (cardOffset > cardWidth)
		// {
		// 	// Fan cards centered on bottom of screen
		// 	cardOffset = cardWidth;
			
		// }
		// else
		// {
			// Fan cards from left
			for (int i = 0; i < playerHand.Count; i++)
			{
				GameObject card = playerHand[i];
				card.transform.position = new Vector3(bottomLeftCamera.x + (cardOffset * i) + (cardWidth / 2f), bottomLeftCamera.y + cardHeight / 2, 0);
				card.SetActive(true);
			}
		// }
	}

	public void Shuffle()
	{
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
		List<GameObject[]> passedCards = new List<GameObject[]>();
		passedCards.Add(GetComponent<GameManager>().GetSelectedCards().ToArray());

		foreach (SimpleOpponentTest opponent in opponents)
		{
			passedCards.Add(opponent.GetPassingCards());
		}

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

			case 3: // Pass diagonally
				if (numPlayers > 3)
				{
					
				}
				break;
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

	private string GetCardValue(GameObject card)
	{
		string value = "";
		string[] suits = {"S", "H", "D", "C"};

		string imageName = card.GetComponent<SpriteRenderer>().sprite.name;
		int imageIndex = int.Parse(imageName.Split('_')[^1]);

		int cardValue = imageIndex % CARDS_PER_SUIT;

		// Keeps face card values as letters
		if (cardValue > 10)
		{
			switch (cardValue - 10)
			{
				case 1: value += "J"; break;
				case 2: value += "Q"; break;
				case 3: value += "K"; break;
			}
		}
		else if (cardValue == 0)
		{
			value += "A";
		}
		else
		{
			value += cardValue;
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
		hand.Sort(delegate(GameObject card1, GameObject card2)
		{
			int[] card1Vals = CardValToInts(GetCardValue(card1));
			int[] card2Vals = CardValToInts(GetCardValue(card2));

			return (card1Vals[1] * CARDS_PER_SUIT + card1Vals[0]) - (card2Vals[1] * CARDS_PER_SUIT + card2Vals[0]);
		});
	}

}
