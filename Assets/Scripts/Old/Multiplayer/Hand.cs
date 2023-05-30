using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Multiplayer
{
	public class Hand
	{
		private float width, height;
		private List<GameObject> cards;

		public Hand(List<GameObject> dealtCards)
		{
			cards = dealtCards;
			
			Rect cardRect = cards[0].GetComponent<SpriteRenderer>().sprite.rect;
			width = cardRect.width;
			height = cardRect.height;
		}

		public void AddCards(List<GameObject> cards)
		{
			cards.AddRange(cards);
		}

		public GameObject GetCard(int index)
		{
			GameObject card = cards[index];
			cards.Remove(card);

			return card;
		}

		public void PlaceCards(Vector3 placeAreaCenter, Vector3 placeAreaSize)
		{
			List<Transform> transforms = new List<Transform>();
			foreach (GameObject card in cards)
			{
				transforms.Add(card.transform);
			}

			Utility.PlaceObjectsInSpread(transforms, cards.Count, width, height, placeAreaCenter, placeAreaSize, Vector3.zero);
		}
	}
}