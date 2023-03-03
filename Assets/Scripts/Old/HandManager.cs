// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class HandManager : MonoBehaviour
// {
// 	private List<Card> hand;

// 	void Start()
// 	{
// 		hand = new List<Card>();
// 	}

// 	public void GetCard(Card card)
// 	{
// 		hand.Add(card);
// 	}

// 	public void GetCards(IEnumerable<Card> cards)
// 	{
// 		hand.AddRange(cards);
// 	}

// 	public Card PlayCard(Card card)
// 	{
// 		hand.Remove(card);
// 		return card;
// 	}

// 	public Card PlayCard(int index)
// 	{
// 		return PlayCard(hand[index]);
// 	}

// 	public void EmptyHand()
// 	{

// 		hand.Clear();
// 	}
// }
