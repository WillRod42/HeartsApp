using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
	public float selectOffset;

	private bool selected;

  private void Start()
  {
    selected = false;
  }

	public void OnSelect()
	{
		switch (PhaseManager.GetCurrPhase())
		{
			case Phase.Passing: 
				Vector3 currPos = transform.position;
				transform.position = new Vector3(currPos.x, currPos.y + (selectOffset * (selected ? -1f : 1f)), 0);
				selected = !selected;
				break;

			case Phase.Playing: break;
		}
		
	}
}
