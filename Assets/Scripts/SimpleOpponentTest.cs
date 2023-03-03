using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleOpponentTest
{
	public List<GameObject> hand;

  private void Start()
  {
    
  }

	// Just passes first 3 cards in hand
	public GameObject[] GetPassingCards()
	{
		return hand.GetRange(0, 3).ToArray();
	}
}
