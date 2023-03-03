using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseButton : MonoBehaviour
{
	public TMPro.TMP_Text btnTxt;

  private void Start()
  {
    
  }

	public void UpdateButtonText()
	{
		btnTxt.text = PhaseManager.GetCurrPhase().ToString();
	}
}
