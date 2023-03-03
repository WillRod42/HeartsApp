// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class GameManager : MonoBehaviour
// {
// 	public GameObject cardPrefab;
// 	public int numPlayers;

// 	private Sprite[] cardImages;
// 	private HandManager[] hands;
// 	private DeckManager deckManager;
// 	private List<Card> cards;

// 	private int[] scoreboard;

// 	public void StartRound()
// 	{
// 		foreach (HandManager hand in hands)
// 		{
// 			hand.EmptyHand();
// 		}

// 		deckManager.Deal(hands);
// 	}

//   void Start()
//   {
//     hands = new HandManager[numPlayers];
// 		deckManager = GetComponent<DeckManager>();
// 		scoreboard = new int[numPlayers];

// 		cardImages = Resources.LoadAll<Sprite>("English_pattern_playing_cards_deck");
// 		cards = new List<Card>();
// 		for (int i = 1; i <= 52; i++)
// 		{
// 			Card cardObj = new Card();
// 			cardObj.cardImage = cardImages[i];
// 			cardObj.value = i;
			
// 			cards.Add(cardObj);
// 		}

// 		deckManager.FillDeck(cards);
//   }

// 	private void DrawCardFromDeck(int player)
// 	{
// 		hands[player].GetCard(deckManager.DrawCard());
// 	}
// }

// public class Card : ScriptableObject
// {
// 	public Sprite cardImage;
// 	public int value;
// }