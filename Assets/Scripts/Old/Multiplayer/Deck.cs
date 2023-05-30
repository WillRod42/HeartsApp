using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Multiplayer
{
	public class Deck : MonoBehaviour
	{
		private const int DECK_LENGTH = 52;
		private const int CARDS_PER_SUIT = 13;

		private GameObject[] cards;
		private Stack<GameObject> deck;
		private Sprite[] cardImages;
		private float cardWidth;
		private float cardHeight;

		private void Awake()
		{
			// Initialize cards and card images
			cards = new GameObject[DECK_LENGTH];
			deck = new Stack<GameObject>();
			cardImages = Resources.LoadAll<Sprite>("English_pattern_playing_cards_deck");
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

		public void Shuffle()
		{
			GameObject[] temp = new GameObject[DECK_LENGTH];
			cards.CopyTo(temp, 0);

			for (int i = 0; i < temp.Length; i++)
			{
				int j = UnityEngine.Random.Range(0, i + 1);
				GameObject card = temp[i];
				temp[i] = temp[j];
				temp[j] = card;
			}
			
			deck.Clear();
			foreach (GameObject card in temp)
			{
				deck.Push(card);
			}
		}

		public GameObject Draw()
		{
			return deck.Pop();
		}

		public bool HasCards()
		{
			return deck.Count > 0;
		}
	}
}
