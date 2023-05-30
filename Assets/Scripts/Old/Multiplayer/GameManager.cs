using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Multiplayer
{
	public class GameManager : NetworkBehaviour
	{
		[SerializeField] private Deck deck;
		[SerializeField] private int numPlayers;

		
		private void Start()
		{

		}

		private void Deal()
		{
			// List<GameObject>[] dealtCards = new List<GameObject>[numPlayers];
			// for (int i = 0; i < numPlayers; i++)
			// {
			// 	dealtCards[i] = new List<GameObject>();
			// }

			// deck.Shuffle();
			// int handIndex = 0;
			// while (deck.HasCards())
			// {
			// 	dealtCards[handIndex].Add(deck.Draw());

			// 	if (handIndex == numPlayers - 1)
			// 	{
			// 		handIndex = 0;
			// 	}
			// 	else
			// 	{
			// 		handIndex++;
			// 	}
			// }

			// for (int i = 0; i < numPlayers; i++)
			// {
			// 	hands[i] = new Hand(dealtCards[i]);
			// }
		}

	}
}
