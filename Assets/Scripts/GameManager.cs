using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
	private DeckManager deck;

  private void Start()
  {
		deck = GetComponent<DeckManager>();

		// Subscribe methods to phase events
		PhaseManager.onDealingPhase += deck.Shuffle;
		PhaseManager.onDealingPhase += deck.DealHands;
		PhaseManager.onDealingPhase += deck.PlaceCards;
  }

	private void OnTouch()
	{
		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
		Vector2 rayOrigin = new Vector2(mousePos.x, mousePos.y);

		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.zero);
		if (hit.transform)
		{
			hit.collider.gameObject.GetComponent<Card>()?.OnSelect();
		}
	}
}
