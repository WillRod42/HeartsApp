// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;

// public class DeckManager : MonoBehaviour
// {
// 	private Stack<Card> deck;

// 	public void FillDeck(IEnumerable<Card> cards)
// 	{
// 		deck = new Stack<Card>(cards);
// 	}

// 	public Card DrawCard()
// 	{
// 		return deck.Pop();
// 	}

// 	public void Deal(HandManager[] hands)
// 	{
// 		for (int i = 0; i < 52 / hands.Length; i++)
// 		{
// 			for (int j = 0; j < hands.Length; j++)
// 			{
// 				hands[j].GetCard(DrawCard());
// 			}
// 		}
// 	}

// 	public void Shuffle()
// 	{
// 		Card[] tempDeck = deck.ToArray();
// 		for (int i = 0; i < tempDeck.Length - 1; i++)
// 		{
// 			int j = Random.Range(0, i + 1);
// 			Card temp = tempDeck[i];
// 			tempDeck[i] = tempDeck[j];
// 			tempDeck[j] = temp;
// 		}

// 		deck = new Stack<Card>(tempDeck);
// 	}
// }

